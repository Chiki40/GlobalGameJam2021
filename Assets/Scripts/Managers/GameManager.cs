using UnityEngine;

public class GameManager : MonoBehaviour
{
    public const int kMaxCluesZones = 3;

    public static GameManager instance = null;

    public static string MapToLoad = null;
    public static Sprite SpritePhoto = null;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
