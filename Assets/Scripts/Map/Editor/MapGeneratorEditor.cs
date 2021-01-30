using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        DrawDefaultInspector();
        if (GUILayout.Button("Generar Mapa"))
        {
            MapGenerator mapGenerator = (MapGenerator)target;
            mapGenerator.Clear();
            MapData data = new MapData();
            string ser = Serializator.XmlSerialize<MapData>(data);
            Debug.Log(ser);
            mapGenerator.Generate();
        }

        if (GUILayout.Button("Limpiar Mapa"))
        {
            MapGenerator mapGenerator = (MapGenerator)target;
            mapGenerator.Clear();
        }
    }
}