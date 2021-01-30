using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private GameObject _player = null;
    [SerializeField]
    private Vector2 _minBounds = new Vector2(0.0f, 0.0f);
    [SerializeField]
    private Vector2 _maxBounds = new Vector2(0.0f, 0.0f);

    private void Update()
    {
        Vector3 playerPos = _player.transform.position;
        Vector3 newPos = transform.position;
        if (playerPos.x > transform.position.x) // Player is to the right of the camera
        {
            float thresholdX = transform.position.x + _maxBounds.x;
            if (playerPos.x > thresholdX)
            {
                newPos.x += playerPos.x - thresholdX;
            }
        }
        else if (playerPos.x < transform.position.x) // Player is to the left of the camera
        {
            float thresholdX = transform.position.x - _minBounds.x;
            if (playerPos.x < thresholdX)
            {
                newPos.x -= thresholdX - playerPos.x;
            }
        }

        float thresholdZ = transform.position.z - _minBounds.y;
        if (playerPos.z < thresholdZ)
        {
            newPos.z -= thresholdZ - playerPos.z;
        }
        else
		{
            thresholdZ = transform.position.z + _maxBounds.y;
            if (playerPos.z > thresholdZ)
            {
                newPos.z += playerPos.z - thresholdZ;
            }
        }
        transform.position = newPos;
    }
}
