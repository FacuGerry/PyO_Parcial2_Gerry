using System;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public event Action OnLose;
    public event Action<Character> OnWin;

    [Header("Data")]
    [SerializeField] private MapDataSO _data;

    [Header("Character's Data")]
    [SerializeField] private CharacterDataSO[] _charsData;

    [Header("Character's Game Objects")]
    [SerializeField] private GameObject[] _charsGos;

    // Los hago arrays y no listas porque los arrays ocupan la memoria de forma más ordenada
    // Además, no necesito modificar el tamaño durante runtime

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
        _movementMng.OnMovementEnd += OnMovementEnd_ChangeTurns;

        foreach (HealthSystem hs in _hs)
            hs.OnCharacterDie += OnCharacterDie_RemoveFromList;
    }

    private void Start()
    {
        _gameInit.StartGame(_data, _charsData, _charsGos);
        AddHealthSytstemToCharacters();
    }

    private void OnDisable()
    {
        _gameInit.OnStartUpCompleted -= OnStartUpCompleted_LoadListsAndStartTurns;
        _movementMng.OnMovementEnd -= OnMovementEnd_ChangeTurns;
        foreach (HealthSystem hs in _hs)
            hs.OnCharacterDie -= OnCharacterDie_RemoveFromList;
    }

    private void OnDestroy()
    {
        if (_turnMng != null)
            _turnMng.OnTurnChanged -= OnTurnChanged_ChangeActiveCharacter;
    }

    private void OnStartUpCompleted_LoadListsAndStartTurns(List<List<GridCell>> map, List<Character> chars)
    {
        _gridCells = map;
        _characters = chars;

        _movementMng.Initialize(_gridCells, _players);

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
            {
                // Start PvP combat
            }
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
            OnLose?.Invoke();
            Debug.Log("A player died with enemies alive!");
            _movementMng.StopCharacterMovement();
        }
        else
        {
            _activeCharacter = index;
            _movementMng.ChangeCurrentCharacter(_characters[_activeCharacter]);
            Debug.Log(_characters[_activeCharacter].GetData().characterName + "'s turn");
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

    private void OnMovementEnd_ChangeTurns()
    {
        HealthSystem currentHealth = _characters[_activeCharacter].GetHealthSystem();

        if (currentHealth != null)
            currentHealth.TakeDamage(5);

        _turnMng.ChangeTurn();
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
}