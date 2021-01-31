﻿using UnityEngine;

public class Ambience : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        UtilSound.instance.PlaySound("WATER_Sea_Waves_Small_15sec_loop_stereo", 0.2f, loop: true);
    }

	private void OnDestroy()
	{
        UtilSound.instance.StopSound("WATER_Sea_Waves_Small_15sec_loop_stereo");
    }
}
