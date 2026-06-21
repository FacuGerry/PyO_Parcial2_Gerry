using System;
using System.Collections.Generic;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    public event Action<List<List<GridCell>>, List<Character>> OnStartUpCompleted; // _gridCells, _chars

    // Map Data - RECIEVE
    private MapDataSO _data;

    // Map cells - SEND BACK
    private List<List<GridCell>> _gridCells;

    // Characters - SEND BACK
    private List<Character> _chars = new();

    public void StartGame(MapDataSO mapData, CharacterDataSO[] charsData, GameObject[] charsGos)
    {
        _data = mapData;

        MapBuilder mapBuilder = new();
        _gridCells = mapBuilder.GenerateMap(_data.gridHeight, _data.gridWidth, _data.cellPrefab);

        InitializeMap();

        CreateCharacters(charsData, charsGos);

        SpawnCharacters(_chars);

        OnStartUpCompleted?.Invoke(_gridCells, _chars);
        Debug.Log("Game initialized!");
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

    private void CreateCharacters(CharacterDataSO[] dataArray, GameObject[] objectsArray)
    {
        for (int i = 0; i < dataArray.Length; i++)
        {
            Character character = new(dataArray[i], Vector2Int.zero, objectsArray[i]);
            _chars.Add(character);
        }
    }

    private void SpawnCharacters(List<Character> list)
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

            TerrainType type = list[i].GetData().isPlayer ? TerrainType.PLAYER : TerrainType.ENEMY;
            _gridCells[randomY][randomX].SetNewTerrainType(type);

            character.GetGO().transform.position = _gridCells[randomY][randomX].GetCellGO().transform.position;
            character.SetNewPosition(new(randomX, randomY));
        }
    }
}