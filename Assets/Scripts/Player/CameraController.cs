using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private GameObject _player = null;
    [SerializeField]
    private Vector2 _minThresholdsPerc = new Vector2(0.0f, 0.0f);
    [SerializeField]
    private Vector2 _maxThresholdsPerc = new Vector2(0.0f, 0.0f);

    private Vector2 screenLimits = Vector2.zero;

    void Start()
    {
        screenLimits = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        screenLimits.x = Mathf.Abs(screenLimits.x);
        screenLimits.y = Mathf.Abs(screenLimits.y);
    }

    void Update()
    {
        Vector2 playerPos = _player.transform.position;
        Vector3 newPos = transform.position;
        if (playerPos.x > transform.position.x) // Player is to the right of the camera
        {
            float threshold = (transform.position.x - screenLimits.x / 2.0f) + screenLimits.x * _maxThresholdsPerc.x;
            if (playerPos.x > threshold)
            {
                newPos.x += playerPos.x - threshold;
            }
        }
        else if (playerPos.x < transform.position.x) // Player is to the left of the camera
        {
            float threshold = (transform.position.x - screenLimits.x / 2.0f) + screenLimits.x * _minThresholdsPerc.x;
            if (playerPos.x < threshold)
            {
                newPos.x -= threshold - playerPos.x;
            }
        }

        if (playerPos.y > transform.position.y) // Player is above the camera
        {
            float threshold = (transform.position.y - screenLimits.y / 2.0f) + screenLimits.y * _maxThresholdsPerc.y;
            if (playerPos.y > threshold)
            {
                newPos.y += playerPos.y - threshold;
            }
        }
        else if (playerPos.y < transform.position.y) // Player is below the camera
        {
            float threshold = (transform.position.y - screenLimits.y / 2.0f) + screenLimits.y * _minThresholdsPerc.y;
            if (playerPos.y < threshold)
            {
                newPos.y -= threshold - playerPos.y;
            }
        }
        transform.position = newPos;
    }
}
