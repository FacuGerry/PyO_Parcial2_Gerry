using System;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    public event Action OnCharacterMove;
    public event Action OnMovementEnd;

    // Current character - used to know if it is a player or an enemy, and therefore knowing HOW to move
    private Character _currentCharacter;

    // Players list - used by enemies to know where to move
    private List<Character> _players = new();

    // Map grid - used to know where to go
    private List<List<GridCell>> _map;

    // Current speed - used for calculating movements left
    private int _currentSpeed = 0;

    private void OnEnable()
    {
        UiButtonsMovement.OnMovementClicked += OnMovementClicked_MovePlayer;
    }

    private void Update()
    {
        if (_currentSpeed <= 0 || _currentCharacter == null || _currentCharacter.GetData().isPlayer)
            return;

        EnemyMovement();
    }

    private void OnDisable()
    {
        UiButtonsMovement.OnMovementClicked -= OnMovementClicked_MovePlayer;
    }

    private void OnMovementClicked_MovePlayer(MovementTypes type)
    {
        if (_currentSpeed <= 0 || _currentCharacter == null || !_currentCharacter.GetData().isPlayer)
            return;

        switch (type)
        {
            case MovementTypes.UP:
                MoveCharacter(0, 1);
                break;

            case MovementTypes.DOWN:
                MoveCharacter(0, -1);
                break;

            case MovementTypes.LEFT:
                MoveCharacter(-1, 0);
                break;

            case MovementTypes.RIGHT:
                MoveCharacter(1, 0);
                break;
        }
    }

    public void Initialize(List<List<GridCell>> map, List<Character> players)
    {
        _map = map;
        _players = players;
    }

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

    private void EnemyMovement()
    {
        List<Vector2Int> playersPositions = GetPlayersPositions();
        if (playersPositions == null) return;

        Vector2Int closestPlayer = CalculateClosestPlayer(playersPositions);
        Vector2Int currentPos = GetCurrentCharacterPosition();

        int moveX = 0;
        int moveY = 0;

        int distX = Mathf.Abs(closestPlayer.x - currentPos.x);
        int distY = Mathf.Abs(closestPlayer.y - currentPos.y);

        if (distX > distY)
            moveX = closestPlayer.x > currentPos.x ? 1 : -1;
        else
            moveY = closestPlayer.y > currentPos.y ? 1 : -1;

        Vector2Int posibleNewPos = GetPosibleNewPosition(moveX, moveY);

        if (!CheckIsValidPosition(posibleNewPos))
        {
            StopCharacterMovement();
            OnMovementEnd?.Invoke();
            return;
        }

        ChangeCharacterCell(posibleNewPos);
    }

    private List<Vector2Int> GetPlayersPositions()
    {
        List<Vector2Int> playerPos = new();
        foreach (Character player in _players)
            playerPos.Add(player.GetPosition());

        if (playerPos.Count == 0)
        {
            Debug.LogError("NO CHARACTER POSITION WAS ADDED TO THE LIST");
            return null;
        }

        return playerPos;
    }

    private Vector2Int CalculateClosestPlayer(List<Vector2Int> playersPos)
    {
        Vector2Int charPos = GetCurrentCharacterPosition();
        int distance = 999;
        int playerIndex = 0;

        for (int i = 0; i < _players.Count; i++)
        {
            int possibleDistance = Mathf.Abs(charPos.x - playersPos[i].x) + Mathf.Abs(charPos.y - playersPos[i].y);
            if (possibleDistance < distance)
            {
                distance = possibleDistance;
                playerIndex = i;
            }
        }

        return _players[playerIndex].GetPosition();
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
        if (position.y < 0 || position.y >= _map.Count) return false;

        if (position.x < 0 || position.x >= _map[position.y].Count) return false;

        GridCell cell = _map[position.y][position.x];
        if (cell.GetTerrainType() == TerrainType.PLAYER || cell.GetTerrainType() == TerrainType.ENEMY) return false;

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

        OnCharacterMove?.Invoke();

        if (_currentSpeed <= 0)
            OnMovementEnd?.Invoke();
    }

    public void ResetTerrainTypeInPosition(Character character) => _map[character.GetPosition().y][character.GetPosition().x].SetNewTerrainType(TerrainType.GRASS);
}