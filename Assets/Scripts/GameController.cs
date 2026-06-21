using System;
using System.Collections.Generic;
using UnityEngine;

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

    // Characters
    private List<Character> _characters = new();
    private int _activeCharacter = 0;

    // Separated characters - used for checking PvE & PvP instances
    private List<Character> _players = new();
    private List<Character> _enemies = new();

    // Managers
    private GameInitializer _gameInit;
    private TurnManager _turnMng;
    private MovementManager _movementMng;

    private void Awake()
    {
        _gameInit = gameObject.AddComponent<GameInitializer>();
        _movementMng = gameObject.AddComponent<MovementManager>();
    }

    private void OnEnable()
    {
        _gameInit.OnStartUpCompleted += OnStartUpCompleted_LoadListsAndStartTurns;
    }

    private void Start()
    {
        _gameInit.StartGame(_data, _charsData, _charsGos);
    }

    private void OnDisable()
    {
        _gameInit.OnStartUpCompleted -= OnStartUpCompleted_LoadListsAndStartTurns;
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

        CreateSeparateCharactersList();
        StartTurns();
    }

    private void OnTurnChanged_ChangeActiveCharacter(int index)
    {
        _activeCharacter = index;
        Debug.Log("'s turn");
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
    }
}