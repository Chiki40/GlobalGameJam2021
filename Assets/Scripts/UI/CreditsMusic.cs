using UnityEngine;

public class CreditsMusic : MonoBehaviour
{
    private void Start()
    {
        UtilSound.instance.PlaySound("MusicPiano", 1.0f, loop: false);
    }

    private void OnDisable()
    {
        if (UtilSound.instance != null)
        {
            UtilSound.instance.StopSound("MusicPiano");
        }
    }
}
