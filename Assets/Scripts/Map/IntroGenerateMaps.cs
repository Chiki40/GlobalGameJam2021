using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroGenerateMaps : MonoBehaviour
{
    public GameObject[] maps;
    private GameObject currenMap;

    // Update is called once per frame
    void Start()
    {
        foreach(GameObject obj in maps)
        {
            obj.SetActive(false);
        }
        currenMap = maps[0];
        StartCoroutine(showMap());
    }

    IEnumerator showMap()
    {
        while (enabled)
        {
            int ran = 0;
            do
            {
                ran = Random.Range(0, maps.Length);
            } while (maps[ran] == currenMap);
            
            currenMap = maps[ran];
            currenMap.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            currenMap.SetActive(false);
        }
        
    }
}
