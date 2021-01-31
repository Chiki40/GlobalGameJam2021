using UnityEngine;

public class CreditsMusic : MonoBehaviour
{
    private void Start()
    {
        UtilSound.instance.PlaySound("MusicPiano", 1.0f, loop: false);
    }

    private void OnDestroy()
    {
        UtilSound.instance.StopSound("MusicPiano");
    }
}
