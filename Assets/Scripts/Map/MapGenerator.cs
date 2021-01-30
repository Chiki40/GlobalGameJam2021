using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    [Serializable]
    public class RandomObject
    {
        [Range(1,20)]
        public int dispersion = 1;
        public GameObject[] gObject;
        public Vector2Int gridUnitsSize = new Vector2Int(1,1);
        [Range(0, 1)]
        public float randomScaleRange = 0;
    }
    public MapData mapData;
    private float gridSize = 1;
    public RandomObject[] randomObjects;

    private bool[,] usedMatrix;
    private int totalPopulation;
    // Start is called before the first frame update
    void Start()
    {
        Clear();
        Generate();
    }

    public void Clear()
    {
        DestroyAllChildren(gameObject);
        usedMatrix = new bool[mapData.mapSize.x, mapData.mapSize.y];
        totalPopulation = 0;
    }

    private void DestroyAllChildren(GameObject gameObject)
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

    public void Generate(MapData data)
    {
        mapData = data;
        Random.InitState(mapData.rSeed);
        totalPopulation = 0;
        for (int i = 0; i < randomObjects.Length; ++i)
        {
            totalPopulation += randomObjects[i].dispersion * randomObjects[i].dispersion;
        }
        float reduction = (float)randomObjects.Length / totalPopulation * 100;
        //var pop = totalPopulation * mapData.population / 100;
        var pop = mapData.population * reduction;
        GenerateRandoMap(pop);

        // Log estadísticas
        int used = 0;
        for (int i = 0; i< mapData.mapSize.x; ++i)
        {
            for (int j = 0; j < mapData.mapSize.y; ++j)
            {
                if (usedMatrix[i, j])
                    ++used;
            }
        }
        Debug.Log("Usados: " + used + "/" + mapData.mapSize.x * mapData.mapSize.y);
    }
    public void Generate()
    {
        Generate(mapData);
    }
    private void GenerateRandoMap(float population)
    {
        // Iterate map y
        for (int i = 0; i < mapData.mapSize.y; ++i)
        {
            // Iterate map x
            for (int j = 0; j < mapData.mapSize.x; ++j)
            {
                // Si ya se ha rellenado previamente pasamos
                if (usedMatrix[j,i]) 
                    continue;

                int objectToInstantiate = Random.Range(0, randomObjects.Length);
                int floorOrObject = Random.Range(0, totalPopulation);
                if (floorOrObject < population)
                {
                    RandomObject obj = randomObjects[objectToInstantiate];
                    GenerateObject(obj, j, i);
                }
            }
        }
    }

    // Update is called once per frame
    void GenerateObject(RandomObject obj,int gridX, int gridY)
    {
        // Iterate dispersion para situar varios objetos del mismo tipo juntos
        Vector2Int dispersion = new Vector2Int(Random.Range(1, obj.dispersion), Random.Range(1, obj.dispersion));
        // Se utiliza el tamaño por si hay objetos que ocupen más de una cuadrícula
        for (int x = 0; x < dispersion.x * obj.gridUnitsSize.x; ++x)
        {
            for (int y = 0; y < dispersion.y * obj.gridUnitsSize.y; ++y)
            {
                Vector2Int coordinates = new Vector2Int(gridX + x, gridY + y);
                // Se puede salir de la cuadrícula
                if (coordinates.x < usedMatrix.GetLength(0) && coordinates.y < usedMatrix.GetLength(1))
                {
                    if (x % obj.gridUnitsSize.x == 0 && y % obj.gridUnitsSize.y == 0)
                    {
                        int randomDispersion = Random.Range(0, totalPopulation);
                        if (randomDispersion < mapData.dispersionPopulation)
                        {
                            // Se parte de la posición del MapGenerator
                            Vector3 pos = transform.position;
                            // Se posiciona en el centro de la cuadrícula desplazando según la posición del
                            // grid que tengamos. El pivote está en el centro así que se posiciona en el centro
                            // de lo que ocupe
                            pos.x += coordinates.x * gridSize + obj.gridUnitsSize.x * 0.5f;
                            pos.z += coordinates.y * gridSize + obj.gridUnitsSize.y * 0.5f;
                            int randomObjet = Random.Range(0, obj.gObject.Length);
                            GameObject objCreated = Instantiate(obj.gObject[randomObjet], pos, Quaternion.AngleAxis(Random.Range(0, 360), new Vector3(0, 1, 0)), transform);
                            if (obj.randomScaleRange > 0)
                            {
                                objCreated.transform.localScale = new Vector3(1, Random.Range(1 - obj.randomScaleRange, 1 + obj.randomScaleRange), 1);
                            }
                        }
                        // Se marca la casilla como usada
                        usedMatrix[coordinates.x, coordinates.y] = true;
                    }
                    else
                    {
                        // Como el objeto tiene diferente tamaño, se marcan como usadas el resto de
                        // casillas que ocupa el objeto
                        usedMatrix[coordinates.x, coordinates.y] = true;
                    }
                }
            }
        }
    }
}
