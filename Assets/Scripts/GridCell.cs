using UnityEngine;

public class GridCell
{
    private GameObject _prefab;
    private TerrainType _terrainType;

    public GridCell(GameObject go, TerrainType type)
    {
        SetNewGO(go);
        SetNewTerrainType(type);
    }

    public GameObject GetCellGO() => _prefab;
    public TerrainType GetTerrainType() => _terrainType;

    public void SetNewGO(GameObject go) => _prefab = go;
    public void SetNewTerrainType(TerrainType type) => _terrainType = type;
}