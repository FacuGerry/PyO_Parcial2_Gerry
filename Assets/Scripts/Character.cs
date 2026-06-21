using UnityEngine;

public class Character
{
    private CharacterDataSO _data;
    private Vector2Int _position;
    private GameObject _go;

    public Character(CharacterDataSO data, Vector2Int position, GameObject go)
    {
        _data = data;
        _position = position;
        _go = go;
    }

    public CharacterDataSO GetData() => _data;
    public GameObject GetGO() => _go;
    public Vector2Int GetPosition() => _position;

    public void SetNewGO(GameObject go) => _go = go;
    public void SetNewPosition(Vector2Int newPos) => _position = newPos;
}