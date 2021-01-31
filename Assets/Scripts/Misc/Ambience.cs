using UnityEngine;

public class Ambience : MonoBehaviour
{
    private void Start()
    {
        UtilSound.instance.PlaySound("WATER_Sea_Waves_Small_15sec_loop_stereo", 0.2f, loop: true);
    }

	private void OnDisable()
	{
        if (UtilSound.instance != null)
        {
            UtilSound.instance.StopSound("WATER_Sea_Waves_Small_15sec_loop_stereo");
        }
    }
}
