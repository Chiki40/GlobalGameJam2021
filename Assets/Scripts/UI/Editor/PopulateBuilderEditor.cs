using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PopulateBuilder))]
public class PopulateBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Populate"))
        {
            PopulateBuilder autoLoad = (PopulateBuilder)target;
            autoLoad.Populate();
        }

        if(GUILayout.Button("Remove"))
        {
            PopulateBuilder autoLoad = (PopulateBuilder)target;
            autoLoad.Remove();
        }
    }
}
