using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapCreator))]
public class MapCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        DrawDefaultInspector();
        if (GUILayout.Button("Generar Mapa Aleatorio"))
        {
            MapCreator mapCreator = (MapCreator)target;
            mapCreator.CreateRandomMap();
        }

        if (GUILayout.Button("Generar Pala y tesoro"))
        {
            MapCreator mapCreator = (MapCreator)target;
            mapCreator.m_mapGenerator.InstantitatePalaYTesoro();
        }
    }
}
