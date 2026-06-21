using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    // Current character - used to know if it is a player or an enemy, and therefore knowing HOW to move
    private Character _currentCharacter;

    // Players list - used by enemies to know where to move
    private List<Character> _players;

    // Map grid - used to know where to go
    private List<List<GridCell>> _map;

    // current speed - used for calculating movements left
    private int _currentSpeed = 0;

    private void Update()
    {
        if (_currentSpeed <= 0 || _currentCharacter == null) return;

        if (_currentCharacter.GetData().isPlayer)
            PlayerMovement();
        else
            EnemyMovement();
    }

    public void Initialize(List<List<GridCell>> map) => _map = map;

    public void ChangeCurrentCharacter(Character newChar)
    {
        _currentCharacter = newChar;
        _currentSpeed = _currentCharacter.GetData().speed;
    }

    public void StopCharacterMovement()
    {
        _currentSpeed = 0;
        _currentCharacter = null;
    }

    private void PlayerMovement()
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

    private void EnemyMovement()
    {

    }

    private void MoveCharacter(int movx, int movy)
    {
        Vector2Int posibleNewPosition = GetPosibleNewPosition(movx, movy);
        if (CheckIsValidPosition(posibleNewPosition))
            ChangeCharacterCell(posibleNewPosition);
    }

    private Vector2Int GetCurrentCharacterPosition() => _currentCharacter.GetPosition();
    private Vector2Int GetPosibleNewPosition(int movx, int movy) => new(GetCurrentCharacterPosition().x + movx, GetCurrentCharacterPosition().y + movy);

    private bool CheckIsValidPosition(Vector2Int position)
    {
        if (position.x < 0 || position.x >= _map[position.y].Count) return false;

        if (position.y < 0 || position.y >= _map.Count) return false;

        return true;
    }

    private void ChangeCharacterCell(Vector2Int newPosition)
    {
        GridCell gridCell = _map[GetCurrentCharacterPosition().y][GetCurrentCharacterPosition().x];
        gridCell.SetNewTerrainType(TerrainType.GRASS);

        _currentCharacter.SetNewPosition(newPosition);
        _currentSpeed--;

        GridCell newCell = _map[GetCurrentCharacterPosition().y][GetCurrentCharacterPosition().x];
        TerrainType type = _currentCharacter.GetData().isPlayer ? TerrainType.PLAYER : TerrainType.ENEMY;
        newCell.SetNewTerrainType(type);

        _currentCharacter.GetGO().transform.position = newCell.GetCellGO().transform.position;
    }
}