using System;
using System.Collections.Generic;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    public event Action<List<List<GridCell>>, List<Character>, List<Character>> OnStartUpCompleted;

    private MapDataSO _data;

    // Characters data - NO NEED TO SEND BACK
    private CharacterDataSO[] _playersData;
    private CharacterDataSO[] _enemiesData;

    // Characters gameObjects - NO NEED TO SEND BACK
    private GameObject[] _playersGos;
    private GameObject[] _enemiesGos;

    // Map cells
    private List<List<GridCell>> _gridCells;

    // Characters separate
    private List<Character> _playersChars = new();
    private List<Character> _enemiesChars = new();

    public void StartGame(MapDataSO mapData, CharacterDataSO[] playersData, CharacterDataSO[] enemiesData, GameObject[] playersGos, GameObject[] enemiesGos)
    {
        _data = mapData;
        
        _playersData = playersData;
        _enemiesData = enemiesData;
        
        _playersGos = playersGos;
        _enemiesGos = enemiesGos;

        MapBuilder mapBuilder = new();
        _gridCells = mapBuilder.GenerateMap(_data.gridHeight, _data.gridWidth, _data.cellPrefab);

        InitializeMap();

        CreateCharacters(_playersData);
        CreateCharacters(_enemiesData);

        SpawnCharacters(_playersChars, TerrainType.PLAYER);
        SpawnCharacters(_enemiesChars, TerrainType.ENEMY);

        OnStartUpCompleted?.Invoke(_gridCells, _playersChars, _enemiesChars);
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
            if (array == _playersData)
            {
                Character character = new(array[i], Vector2Int.zero, _playersGos[i]);
                _playersChars.Add(character);
            }
            else if (array == _enemiesData)
            {
                Character character = new(array[i], Vector2Int.zero, _enemiesGos[i]);
                _enemiesChars.Add(character);
            }
            else
            {
                Debug.LogError("NO CHARACTER WAS CREATED!");
                return;
            }
        }
    }

    private void SpawnCharacters(List<Character> list, TerrainType type)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Character character = list[i];

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