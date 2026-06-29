using System;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public event Action<List<Character>> OnGameInitialized; // _characters

    public event Action<int> OnTurnChanged; // _activeCharacter

    public event Action<List<Character>, List<Character>, List<Character>> OnDistanceCalculated; // _charsToMelee, _charsToRange, _charsToHeal

    public event Action OnPlayersLose;

    public event Action<Character> OnWin; // winner

    [Header("Data")]
    [SerializeField] private MapDataSO _data;

    [Header("Character's Data")]
    [SerializeField] private CharacterDataSO[] _charsData;

    [Header("Character's Game Objects")]
    [SerializeField] private GameObject[] _charsGos;

    [Header("Actions")]
    [SerializeField] private UiCharacterActions _charActions;

    // Map cells
    private List<List<GridCell>> _gridCells;

    // Characters - used for turns
    private List<Character> _characters = new();
    private int _activeCharacter = 0;

    // Separated characters - used for checking PvE & PvP instances
    private List<Character> _players = new();
    private List<Character> _enemies = new();

    // Managers
    private GameInitializer _gameInit;
    private TurnManager _turnMng;
    private MovementManager _movementMng;
    private ActionsMananger _actionsMng = new();

    private List<HealthSystem> _hs = new();

    private void Awake()
    {
        _gameInit = gameObject.AddComponent<GameInitializer>();
        _movementMng = gameObject.AddComponent<MovementManager>();

        foreach (GameObject character in _charsGos)
        {
            HealthSystem hs = character.GetComponent<HealthSystem>();
            _hs.Add(hs);
        }
    }

    private void OnEnable()
    {
        _gameInit.OnStartUpCompleted += OnStartUpCompleted_LoadListsAndStartTurns;

        _movementMng.OnCharacterMove += OnCharacterMove_ReloadActions;
        _movementMng.OnMovementEnd += OnMovementEnd_ChangeTurns;

        foreach (HealthSystem hs in _hs)
            hs.OnCharacterDie += OnCharacterDie_RemoveFromList;

        _charActions.OnHealClicked += OnHealClicked_Heal;
        _charActions.OnMeleeClicked += OnMeleeClicked_AttackMelee;
        _charActions.OnRangedClicked += OnRangedClicked_AttackRanged;
    }

    private void Start()
    {
        _gameInit.StartGame(_data, _charsData, _charsGos);
    }

    private void OnDisable()
    {
        _gameInit.OnStartUpCompleted -= OnStartUpCompleted_LoadListsAndStartTurns;

        _movementMng.OnCharacterMove -= OnCharacterMove_ReloadActions;
        _movementMng.OnMovementEnd -= OnMovementEnd_ChangeTurns;

        foreach (HealthSystem hs in _hs)
            hs.OnCharacterDie -= OnCharacterDie_RemoveFromList;

        _charActions.OnHealClicked -= OnHealClicked_Heal;
        _charActions.OnMeleeClicked -= OnMeleeClicked_AttackMelee;
        _charActions.OnRangedClicked -= OnRangedClicked_AttackRanged;
    }

    private void OnDestroy()
    {
        if (_actionsMng != null)
        {
            _actionsMng.OnDistanceCalculated -= OnDistanceCalculated_ToogleButtonsUi;
            _actionsMng.OnEnemyAttacked -= OnEnemyAttacked_ChangeTurn;
        }

        if (_turnMng != null)
            _turnMng.OnTurnChanged -= OnTurnChanged_ChangeActiveCharacter;
    }

    private void OnStartUpCompleted_LoadListsAndStartTurns(List<List<GridCell>> map, List<Character> chars)
    {
        _gridCells = map;
        _characters = chars;

        AddHealthSytstemToCharacters();

        OnGameInitialized?.Invoke(_characters);

        _movementMng.Initialize(_gridCells, _players);

        _actionsMng.InitializeActions(_characters, _hs);
        _actionsMng.ToogleEnemiesDead(false);
        _actionsMng.OnDistanceCalculated += OnDistanceCalculated_ToogleButtonsUi;
        _actionsMng.OnEnemyAttacked += OnEnemyAttacked_ChangeTurn;

        SeparateCharactersList(_characters);
        StartTurns();
    }

    private void OnTurnChanged_ChangeActiveCharacter(int index)
    {
        if (!_characters[index].IsAlive())
        {
            _turnMng.ChangeTurn();
            return;
        }

        CheckForNextMove(index);
        _actionsMng.ChangeActiveCharacter(_characters[index]);
        OnTurnChanged?.Invoke(_activeCharacter);
    }

    private void OnCharacterDie_RemoveFromList(CharacterDataSO data)
    {
        ResetTerrainInPosition(data);

        if (KillCharacter(_players, data))
        {
            KillCharacter(_characters, data);
        }
        else if (KillCharacter(_enemies, data))
        {
            KillCharacter(_characters, data);

            if (AllEnemiesDead())
                _actionsMng.ToogleEnemiesDead(true);
        }
        else
        {
            Debug.LogError("A CHARACTER DIED BUT WAS NOT FOUND IN ANY LIST");
            return;
        }
    }

    private void CheckForNextMove(int index)
    {
        if (OnePlayerAlive())
        {
            Character winner = GetAlivePlayer();
            OnWin?.Invoke(winner);
            _movementMng.StopCharacterMovement();
        }
        else if (!AllEnemiesDead() && IsPlayerDead())
        {
            OnPlayersLose?.Invoke();
            Debug.Log("A player died with enemies alive!");
            _movementMng.StopCharacterMovement();
        }
        else
        {
            _activeCharacter = index;
            _movementMng.ChangeCurrentCharacter(_characters[_activeCharacter]);
            Debug.Log($"{_characters[_activeCharacter].GetData().characterName}'s turn");
        }
    }

    private void ResetTerrainInPosition(CharacterDataSO data)
    {
        foreach (Character character in _characters)
        {
            if (character.GetData().characterName == data.characterName)
            {
                _movementMng.ResetTerrainTypeInPosition(character);
                return;
            }
        }
    }

    private void AddHealthSytstemToCharacters()
    {
        for (int i = 0; i < _characters.Count; i++)
        {
            _characters[i].SetNewHealthSystem(_hs[i]);
        }
    }

    private void OnCharacterMove_ReloadActions() => ReloadActions();

    private void OnMovementEnd_ChangeTurns()
    {
        _turnMng.ChangeTurn();
        ReloadActions();
    }

    private void ReloadActions()
    {
        if (_characters[_activeCharacter].GetData().isPlayer)
        {
            _actionsMng.StartPlayerActions();
            return;
        }

        _actionsMng.StartEnemyActions();
    }

    private bool KillCharacter(List<Character> list, CharacterDataSO data)
    {
        int index = list.FindIndex(item => item.GetData().characterName == data.characterName);
        if (index != -1)
        {
            list[index].KillCharacter();
            Debug.Log("CHARACTER " + data.characterName + " WAS KILLED");
            return true;
        }
        return false;
    }

    private bool AllEnemiesDead()
    {
        int deaths = 0;
        foreach (Character enemy in _enemies)
            if (!enemy.IsAlive())
                deaths++;

        if (deaths == _enemies.Count)
        {
            Debug.Log("All enemies are dead!");
            return true;
        }

        return false;
    }

    private bool OnePlayerAlive()
    {
        int deaths = 0;
        foreach (Character player in _players)
            if (!player.IsAlive())
                deaths++;

        if ((_players.Count - deaths) == 1)
        {
            Debug.Log("Only one player left alive!");
            return true;
        }

        return false;
    }

    private bool IsPlayerDead()
    {
        int deaths = 0;
        foreach (Character player in _players)
            if (!player.IsAlive())
                deaths++;

        if (deaths >= 1)
            return true;

        return false;
    }

    private Character GetAlivePlayer()
    {
        foreach (Character player in _players)
        {
            if (player.IsAlive())
            {
                Debug.Log($"Last player alive is {player.GetData().characterName}");
                return player;
            }
        }

        return null;
    }

    private void SeparateCharactersList(List<Character> list)
    {
        foreach (Character character in list)
        {
            if (character.GetData().isPlayer)
                _players.Add(character);
            else
                _enemies.Add(character);
        }
    }

    private void StartTurns()
    {
        _turnMng = new(_characters.Count);
        _turnMng.OnTurnChanged += OnTurnChanged_ChangeActiveCharacter;
        _turnMng.StartTurns();
    }

    private void OnHealClicked_Heal(int index)
    {
        _actionsMng.Heal(_characters[_activeCharacter], _characters[index]);
        _movementMng.StopCharacterMovement();
        _turnMng.ChangeTurn();
    }

    private void OnMeleeClicked_AttackMelee(int index)
    {
        _actionsMng.AttackMelee(_characters[_activeCharacter], _characters[index]);
        _movementMng.StopCharacterMovement();
        _turnMng.ChangeTurn();
    }

    private void OnRangedClicked_AttackRanged(int index)
    {
        _actionsMng.AttackRanged(_characters[_activeCharacter], _characters[index]);
        _movementMng.StopCharacterMovement();
        _turnMng.ChangeTurn();
    }

    private void OnDistanceCalculated_ToogleButtonsUi(List<Character> melee, List<Character> ranged, List<Character> heal)
    {
        if (!_characters[_activeCharacter].GetData().isPlayer)
        {
            OnDistanceCalculated?.Invoke(null, null, null);
            return;
        }

        OnDistanceCalculated?.Invoke(melee, ranged, heal);
    }

    private void OnEnemyAttacked_ChangeTurn(bool hasAttacked)
    {
        if (hasAttacked)
        {
            _movementMng.StopCharacterMovement();
            _turnMng.ChangeTurn();
        }
    }
}