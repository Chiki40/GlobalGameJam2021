using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapCreator : MonoBehaviour
{
    private static MapCreator m_instance;
    public MapData mapDataBase;
    [Range(0,10)]
    public int randomRangeDispersion;
    public MapGenerator m_mapGenerator;
    public static MapCreator instance 
    { 
        get
        {
            if (m_instance == null)
            {
                m_instance = (MapCreator)FindObjectOfType(typeof(MapCreator));
                if (m_instance == null)
                {
                    // Need to create a new GameObject to attach the singleton to.
                    var singletonObject = new GameObject();
                    m_instance = singletonObject.AddComponent<MapCreator>();

                    // Make instance persistent.
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return m_instance;
        } 
    }
    // Start is called before the first frame update
    private void Start()
    {
        m_mapGenerator = GetComponent<MapGenerator>();
    }
    public void CreateRandomMap()
    {
        mapDataBase.dispersionPopulation = Random.Range(mapDataBase.dispersionPopulation - randomRangeDispersion, mapDataBase.dispersionPopulation + randomRangeDispersion);
        mapDataBase.population = Random.Range(mapDataBase.population - randomRangeDispersion, mapDataBase.population + randomRangeDispersion);
        mapDataBase.rSeed = Random.Range(0, Int32.MaxValue);
        m_mapGenerator.Clear();
        m_mapGenerator.Generate(mapDataBase);
    }

    public void CreateMap(string data)
    {
        MapData mapData = Serializator.XmlDeserialize<MapData>(data);
        m_mapGenerator.Clear();
        m_mapGenerator.Generate(mapData);
    }

}
