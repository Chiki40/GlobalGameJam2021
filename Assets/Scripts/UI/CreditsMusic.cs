using UnityEngine;

public class CreditsMusic : MonoBehaviour
{
    private void Start()
    {
        UtilSound.instance.PlaySound("MusicPiano", 1.0f, loop: true);
    }

    private void OnDisable()
    {
        UtilSound.instance.StopSound("MusicPiano");
    }
}
