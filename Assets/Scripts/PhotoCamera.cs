using UnityEngine;

public class PhotoCamera : MonoBehaviour
{
    [SerializeField]
    private float _distance = 10.0f;
    [SerializeField]
    private float _height = 50.0f;
    [SerializeField]
    private float _sphereCollisionTestDistanceChecked = 8.0f;
    [SerializeField]
    private float _sphereCollisionTestRadius = 1.0f;

    [SerializeField]
    RenderTexture _renderTexture = null;

    private Camera _camera = null;

	private void Awake()
	{
        _camera = GetComponent<Camera>();
        _camera.targetTexture = _renderTexture;
    }

    /*
    private void Update()
    {
    if (Input.GetKeyDown(KeyCode.K))
    {
        TakePictureOfArea(new Vector3(2.3f, 2.4f, 7.5f));
    }

    }
    */

    public void TakePictureOfArea(Vector3 pos)
	{
        // First, place camera above the point, depending on _height
        Vector3 originPos = pos;
        originPos.y += _height;
        // Choose a random angle
        float randomAngleToTakePicture = Random.Range(0, 360);
        // Move the camera in that direction, depending on distance
        originPos += Quaternion.Euler(0, randomAngleToTakePicture, 0) * Vector3.forward * _distance;
        transform.position = originPos;
        // Look at the position we want to take the picture of
        transform.LookAt(pos);

        // Take the picture
        TakePicture();
    }

	private void TakePicture()
	{
        // Disable annoying objects
        RaycastHit[] result = Physics.SphereCastAll(transform.position, _sphereCollisionTestRadius, transform.forward, _sphereCollisionTestDistanceChecked);
        for (int i = 0; i < result.Length; ++i)
        {
            Debug.LogError(result[i].collider.gameObject.name);
            result[i].collider.gameObject.SetActive(false);
        }

        _camera.enabled = true;
        _camera.Render();
        _camera.enabled = false;

        // Reenable annoying objects
        for (int i = 0; i < result.Length; ++i)
        {
            result[i].collider.gameObject.SetActive(true);
        }
    }
}
