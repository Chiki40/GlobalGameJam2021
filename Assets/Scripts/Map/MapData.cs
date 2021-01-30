using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using UnityEngine;

[Serializable]
public class MapData
{
    [Serializable]
    public struct HintData
    {
        public Vector2Int gridPos { get; set; }
        public int[] symbols;
    }
    public Vector2Int tresureGridPos;
    public HintData[] hintsGridPos;
    public int rSeed;
    public Vector2Int mapSize;
    [Range(0, 100)]
    public int population;
    [Range(1, 100)]
    public int dispersionPopulation;
}
