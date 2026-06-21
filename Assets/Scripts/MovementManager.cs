using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    private List<Character> _characters;
    private List<List<GridCell>> _map;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            MoveCharacter(-1, 0);
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            MoveCharacter(1, 0);
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            MoveCharacter(0, 1);
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            MoveCharacter(0, -1);
    }

    private void Initialize(List<Character> charactertsList, List<List<GridCell>> map)
    {
        _characters = charactertsList;
        _map = map;
    }

    private void MoveCharacter(int movx, int movy)
    {
        Vector2Int posibleNewPosition = GetPosibleNewPosition(movx, movy);
        if (CheckIsValidPosition(posibleNewPosition))
            ChangeCharacterCell(posibleNewPosition);
    }

    private Vector2Int GetCurrentCharacterPosition() => Vector2Int.zero; // new((int)GetCurrentCharacter().transform.position.x, (int)GetCurrentCharacter().transform.position.y);
    private Vector2Int GetPosibleNewPosition(int movx, int movy) => new(GetCurrentCharacterPosition().x + movx, GetCurrentCharacterPosition().y + movy);

    private bool CheckIsValidPosition(Vector2Int position)
    {
        return position.x >= 0 &&
            position.x < _map[position.y].Count &&
            position.y < _map.Count &&
            position.y >= 0;
    }

    private void ChangeCharacterCell(Vector2Int newPosition)
    {
        GridCell gridCell = _map[GetCurrentCharacterPosition().y][GetCurrentCharacterPosition().x];
        gridCell.SetNewTerrainType(TerrainType.GRASS);

        // move player with newPosition

        gridCell = _map[GetCurrentCharacterPosition().y][GetCurrentCharacterPosition().x];
        gridCell.SetNewTerrainType(TerrainType.PLAYER);
    }
}