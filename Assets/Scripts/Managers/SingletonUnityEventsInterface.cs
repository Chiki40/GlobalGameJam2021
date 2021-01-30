using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonUnityEventsInterface : MonoBehaviour
{
    MapCreator m_mapCreator;
    CharacterController m_CharacterController = null;
    // Start is called before the first frame update
    void Start()
    {
        m_mapCreator = FindObjectOfType<MapCreator>();
        m_CharacterController = FindObjectOfType<CharacterController>();
    }

    // Update is called once per frame
    public void GenerateRandomMap()
    {
        m_mapCreator.CreateRandomMap();
    }

    public void EnablePlayer(bool active)
	{
        m_CharacterController.enabled = active;
	}
}
