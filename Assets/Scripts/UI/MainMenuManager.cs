using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.MapToLoad = null;
        GameManager.SpritePhoto = null;
    }
}
