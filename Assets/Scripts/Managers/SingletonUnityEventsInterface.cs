using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonUnityEventsInterface : MonoBehaviour
{
    MapCreator m_mapCreator;
    // Start is called before the first frame update
    void Start()
    {
        m_mapCreator = FindObjectOfType<MapCreator>();
    }

    // Update is called once per frame
    public void GenerateRandomMap()
    {
        m_mapCreator.CreateRandomMap();
    }
}
