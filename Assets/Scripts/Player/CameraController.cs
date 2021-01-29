using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private GameObject _player = null;
    [SerializeField]
    private Vector2 _minBounds = new Vector2(0.0f, 0.0f);
    [SerializeField]
    private Vector2 _maxBounds = new Vector2(0.0f, 0.0f);

    void Update()
    {
        Vector2 playerPos = _player.transform.position;
        Vector3 newPos = transform.position;
        if (playerPos.x > transform.position.x) // Player is to the right of the camera
        {
            float threshold = transform.position.x + _maxBounds.x;
            if (playerPos.x > threshold)
            {
                newPos.x += playerPos.x - threshold;
            }
        }
        else if (playerPos.x < transform.position.x) // Player is to the left of the camera
        {
            float threshold = transform.position.x - _minBounds.x;
            if (playerPos.x < threshold)
            {
                newPos.x -= threshold - playerPos.x;
            }
        }

        if (playerPos.y > transform.position.y) // Player is above the camera
        {
            float threshold = transform.position.y + _maxBounds.y;
            if (playerPos.y > threshold)
            {
                newPos.y += playerPos.y - threshold;
            }
        }
        else if (playerPos.y < transform.position.y) // Player is below the camera
        {
            float threshold = transform.position.y - _minBounds.y;
            if (playerPos.y < threshold)
            {
                newPos.y -= threshold - playerPos.y;
            }
        }
        transform.position = newPos;
    }
}
