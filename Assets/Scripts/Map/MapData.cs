using System;
using UnityEngine;

[Serializable]
public struct MapData
{
    [Serializable]
    public struct HintData
    {
        public Vector2Int gridPos { get; set; }
        public int[] symbols;
    }
    public Vector2Int shovelGridPos;
    public Vector2Int tresureGridPos;
    public HintData[] hintsGridPos;
    public string message;
    public int rSeed;
    public Vector2Int mapSize;
    [Range(0, 1000)]
    public int population;
    [Range(1, 1000)]
    public int dispersionPopulation;
}
