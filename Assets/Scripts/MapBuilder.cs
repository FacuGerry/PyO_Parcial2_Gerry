using System.Collections.Generic;
using UnityEngine;

public class MapBuilder
{
    public List<List<GridCell>> GenerateMap(int gridHeight, int gridWidth, GameObject cellPrefab)
    {
        List<List<GridCell>> map = new();

        for (int width = 0; width < gridHeight; width++)
        {
            List<GridCell> row = new();
            for (int height = 0; height < gridWidth; height++)
            {
                GridCell cell = new(cellPrefab, TerrainType.GRASS);
                row.Add(cell);
            }
            map.Add(row);
        }

        return map;
    }
}