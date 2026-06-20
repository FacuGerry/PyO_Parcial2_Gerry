using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "GameData/MapData")]
public class MapDataSO : ScriptableObject
{
    public int gridHeight;
    public int gridWidth;
    public int gridCellSize;
    public GameObject cellPrefab;
}