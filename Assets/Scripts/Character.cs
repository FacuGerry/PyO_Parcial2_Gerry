using UnityEngine;

public class Character
{
    private CharacterDataSO _data;
    private GameObject _go;
    private Vector2Int _pos;
    private HealthSystem _hs;

    public Character(CharacterDataSO data, Vector2Int position, GameObject go)
    {
        _data = data;
        _pos = position;
        _go = go;
    }

    public CharacterDataSO GetData() => _data;
    public GameObject GetGO() => _go;
    public Vector2Int GetPosition() => _pos;
    public HealthSystem GetHealthSystem() => _hs;

    public void SetNewGO(GameObject go) => _go = go;
    public void SetNewPosition(Vector2Int newPos) => _pos = newPos;
    public void SetNewHealthSystem(HealthSystem hs) => _hs = hs;
}