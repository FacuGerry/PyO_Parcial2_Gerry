using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class GameController : MonoBehaviour
{
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
    }

    private void OnEnable()
    {
        _gameInit.OnStartUpCompleted += OnStartUpCompleted_LoadListsAndStartTurns;

        foreach (HealthSystem hs in _hs)
            hs.OnCharacterDie += OnCharacterDie_RemoveFromList;
    }

    private void Start()
    {
        _gameInit.StartGame(_data, _charsData, _charsGos);

        foreach (GameObject character in _charsGos)
            _hs.Add(character.GetComponent<HealthSystem>());
    }

    private void OnDisable()
    {
        _gameInit.OnStartUpCompleted -= OnStartUpCompleted_LoadListsAndStartTurns;
    }

    private void OnDestroy()
    {
        foreach (HealthSystem hs in _hs)
            hs.OnCharacterDie -= OnCharacterDie_RemoveFromList;

        if (_turnMng != null)
            _turnMng.OnTurnChanged -= OnTurnChanged_ChangeActiveCharacter;
    }

    private void OnStartUpCompleted_LoadListsAndStartTurns(List<List<GridCell>> map, List<Character> chars)
    {
        _gridCells = map;
        _characters = chars;

        _movementMng.Initialize(_gridCells);

        CreateSeparateCharactersList();
        StartTurns();
    }

    private void OnTurnChanged_ChangeActiveCharacter(int index)
    {
        _activeCharacter = index;
        _movementMng.ChangeCurrentCharacter(_characters[_activeCharacter]);
        Debug.Log(_characters[_activeCharacter].GetData().characterName + "'s turn");
    }

    private void OnCharacterDie_RemoveFromList(CharacterDataSO data)
    {
        if (RemoveCharacterFromList(_players, data) || RemoveCharacterFromList(_enemies, data))
            RemoveCharacterFromList(_characters, data);
        else
            Debug.LogError("A CHARACTER DIED BUT WAS NOT FOUND IN ANY LIST");

        if (AllEnemiesDead())
        {
            // Start PvP combat
        }
    }

    private bool RemoveCharacterFromList(List<Character> list, CharacterDataSO data)
    {
        foreach (Character item in list)
        {
            if (item.GetData() == data)
            {
                list.Remove(item);
                Debug.Log("CHARACTER " + data.characterName + " WAS REMOVED FROM " + list);
                return true;
            }
        }
        return false;
    }

    private bool AllEnemiesDead()
    {
        if (_enemies.Count > 0)
            return false;
        else
            return true;
    }

    private void CreateSeparateCharactersList()
    {
        SeparateCharactersList(_characters);
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