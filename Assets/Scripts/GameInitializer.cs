using System;
using System.Collections.Generic;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    public event Action<List<Character>, List<Character>, List<List<GridCell>>> OnStartUpCompleted;

    [Header("Data")]
    [SerializeField] private MapDataSO _data;

    [Header("Arrays Data")]
    [SerializeField] private CharacterDataSO[] _players; // Los hago arrays y no listas porque los arrays ocupan la memoria de forma más ordenada
    [SerializeField] private CharacterDataSO[] _enemies; // Además, no necesito modificar el tamaño durante runtime

    [Header("Arrays GOs")]
    [SerializeField] private GameObject[] _playersGos; // Los hago arrays y no listas porque los arrays ocupan la memoria de forma más ordenada
    [SerializeField] private GameObject[] _enemiesGos; // Además, no necesito modificar el tamaño durante runtime

    private List<List<GridCell>> _gridCells;
    private List<Character> _playersChars;
    private List<Character> _enemiesChars;

    private void Start()
    {
        MapBuilder mapBuilder = new();
        _gridCells = mapBuilder.GenerateMap(_data.gridHeight, _data.gridWidth, _data.cellPrefab);

        InitializeMap();

        CreateCharacters(_players);
        CreateCharacters(_enemies);

        SpawnCharacters(_players, TerrainType.PLAYER);
        SpawnCharacters(_enemies, TerrainType.ENEMY);

        OnStartUpCompleted?.Invoke(_playersChars, _enemiesChars, _gridCells);
    }

    private void InitializeMap()
    {
        for (int row = 0; row < _gridCells.Count; row++)
        {
            for (int column = 0; column < _gridCells[row].Count; column++)
            {
                GameObject gridCell = Instantiate(_gridCells[row][column].GetCellGO(), transform);
                _gridCells[row][column].SetNewGO(gridCell);

                float positionX = (column * _data.gridCellSize) - _data.gridCellSize;
                float positionY = (row * _data.gridCellSize) - _data.gridCellSize;

                gridCell.transform.localPosition = new(positionX, positionY, 0f);
            }
        }
    }

    private void CreateCharacters(CharacterDataSO[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            Character character = null;

            if (array == _players)
            {
                character = new(array[i], Vector2Int.zero, _playersGos[i]);
                _playersChars.Add(character);
            }
            else if (array == _enemies)
            {
                character = new(array[i], Vector2Int.zero, _enemiesGos[i]);
                _enemiesChars.Add(character);
            }

            if (character == null)
            {
                Debug.LogError("NO CHARACTER WAS CREATED!");
                return;
            }
        }
    }

    private void SpawnCharacters(CharacterDataSO[] array, TerrainType type)
    {
        for (int i = 0; i < array.Length; i++)
        {
            Character character = null;

            int randomX;
            int randomY;
            do
            {
                randomX = UnityEngine.Random.Range(0, _data.gridWidth);
                randomY = UnityEngine.Random.Range(0, _data.gridHeight);
            } while (_gridCells[randomY][randomX].GetTerrainType() != TerrainType.GRASS);

            _gridCells[randomY][randomX].SetNewTerrainType(type);
            character.GetGO().transform.position = _gridCells[randomY][randomX].GetCellGO().transform.position;
        }
    }
}