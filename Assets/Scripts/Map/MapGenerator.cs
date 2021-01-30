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
        public bool limitNumber = false;
        [Range(1, 5)]
        public int maxObjectsInTheScene = 100;
    }
    public MapData mapData;
    private float gridSize = 1;
    public RandomObject[] randomObjects;
    public GameObject pala;
    public GameObject tesoro;

    private bool[,] usedMatrix;
    private int totalPopulation;
    private Dictionary<RandomObject, int> maxObjectsInTheSceneDic;
    // Start is called before the first frame update
    void Start()
    {
        //Clear();
        //Generate();
    }

    public void Clear()
    {
        maxObjectsInTheSceneDic = new Dictionary<RandomObject, int>();
        DestroyAllChildren(gameObject);
        usedMatrix = new bool[mapData.mapSize.x, mapData.mapSize.y];
        totalPopulation = 0;
        Vector2Int pos = new Vector2Int();
        GetGridPos(new Vector3(42, 0, 0),ref pos);
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
        foreach (RandomObject obj in randomObjects)
        {
            maxObjectsInTheSceneDic.Add(obj, obj.maxObjectsInTheScene);
        }

        mapData = data;
        Random.InitState(mapData.rSeed);
        totalPopulation = 0;
        for (int i = 0; i < randomObjects.Length; ++i)
        {
            totalPopulation += randomObjects[i].dispersion * randomObjects[i].dispersion;
        }

        var pop = mapData.population;
        GenerateRandoMap(pop);
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

                int floorOrObject = Random.Range(0, 1000 + totalPopulation);
                if (floorOrObject < population)
                {
                    // Se comprueba si hay suficientes instancias permitidas
                    int objectToInstantiate = Random.Range(0, randomObjects.Length);
                    RandomObject obj = randomObjects[objectToInstantiate];
                    if (obj.limitNumber && maxObjectsInTheSceneDic[obj] == 0)
                        continue;
                    maxObjectsInTheSceneDic[obj] = maxObjectsInTheSceneDic[obj] - 1;
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
                if (coordinates.x < mapData.mapSize.x && coordinates.y < mapData.mapSize.y)
                {
                    if ((x % obj.gridUnitsSize.x == 0) && (y % obj.gridUnitsSize.y == 0))
                    {
                        // Si es el primer objeto de la dispersión siempre se pone
                        int randomDispersion = (x == 0 && y == 0) ? 0 : Random.Range(0, 1000 + totalPopulation);
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

    public bool GetGridPos(Vector3 pos, ref Vector2Int gridPosOut)
    {
        // Si se sale de la cuadrícula
        if (pos.x < transform.position.x || (pos.x > transform.position.x + mapData.mapSize.x)
            || pos.z < transform.position.z || (pos.z > transform.position.z + mapData.mapSize.y))
            return true;

        Vector3 localPos = transform.InverseTransformPoint(pos);
        gridPosOut.x = (int)localPos.x;
        gridPosOut.y = (int)localPos.z;
        return true;
    }

    void InstantitatePalaYTesoro()
    {
        Vector3 pos = transform.position;
        // Se posiciona en el centro de la cuadrícula desplazando según la posición del
        // grid que tengamos. El pivote está en el centro así que se posiciona en el centro
        // de lo que ocupe
        pos.x += mapData.shovelGridPos.x * gridSize + gridSize * 0.5f;
        pos.z += mapData.shovelGridPos.y * gridSize + gridSize * 0.5f;
        Instantiate(pala, pos, Quaternion.AngleAxis(Random.Range(0, 360), new Vector3(0, 1, 0)), transform);

        pos = transform.position;
        pos.x += mapData.tresureGridPos.x * gridSize + gridSize * 0.5f;
        pos.z += mapData.tresureGridPos.y * gridSize + gridSize * 0.5f;
        var obj = Instantiate(tesoro, pos, Quaternion.AngleAxis(Random.Range(0, 360), new Vector3(0, 1, 0)), transform);
        obj.SetActive(false);

    }
}
