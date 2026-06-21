using System;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private MapDataSO _data;

    // Los hago arrays y no listas porque los arrays ocupan la memoria de forma más ordenada
    // Además, no necesito modificar el tamaño durante runtime

    [Header("Arrays Player")]
    [SerializeField] private CharacterDataSO[] _playersData;
    [SerializeField] private GameObject[] _playersGos;

    [Header("Arrays Enemies")]
    [SerializeField] private CharacterDataSO[] _enemiesData;
    [SerializeField] private GameObject[] _enemiesGos;

    // Map cells
    private List<List<GridCell>> _gridCells;

    // Characters separate
    private List<Character> _playersChars;
    private List<Character> _enemiesChars;

    // ALL CHARACTERS
    private List<Character> _charactersList = new();

    // Managers
    private GameInitializer _gameInit;
    private TurnManager _turnMng;

    private void Awake()
    {
        _gameInit = gameObject.AddComponent<GameInitializer>();
    }

    private void OnEnable()
    {
        _gameInit.OnStartUpCompleted += OnStartUpCompleted_LoadListsAndStart;
    }

    private void Start()
    {
        _gameInit.StartGame(_data, _playersData, _enemiesData, _playersGos, _enemiesGos);
    }

    private void OnDisable()
    {
        _gameInit.OnStartUpCompleted -= OnStartUpCompleted_LoadListsAndStart;
    }

    private void OnStartUpCompleted_LoadListsAndStart(List<List<GridCell>> listMap, List<Character> listPlayers, List<Character> listEnemies)
    {
        _gridCells = listMap;
        _playersChars = listPlayers;
        _enemiesChars = listEnemies;

        CreateCharactersList();
        StartTurns();
    }

    private void CreateCharactersList()
    {
        AddToCharactersList(_playersChars);
        AddToCharactersList(_enemiesChars);
    }

    private void AddToCharactersList(List<Character> list)
    {
        foreach (Character character in list)
            _charactersList.Add(character);
    }

    private void StartTurns()
    {
        _turnMng = new(_charactersList);
    }
}