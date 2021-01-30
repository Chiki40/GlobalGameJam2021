using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public enum EDirections { LEFT, RIGHT, BACK, FRONT, LEFT_BACK, RIGHT_BACK, LEFT_FRONT, RIGHT_FRONT };

    [SerializeField]
    float _speed = 1.0f;
    [SerializeField]
    float _diagonalDirectionThreshold = 0.3f;
    [SerializeField]
    float _moveDistanceThreshold = 1.0f;

    private AnimationManager _animationManager = null;

    private void Awake()
    {
        _animationManager = this.GetComponent<AnimationManager>();
    }

	private void Update()
    {
        EDirections direction = EDirections.LEFT;
        bool movingAnimation = false;
        if (GetPlayerInput(out Vector3 targetDir))
        {
            float distanceToMove = _speed * Time.deltaTime;
            Vector3 disp = targetDir.normalized * distanceToMove;
            // Double check Y component is 0
            disp.y = 0.0f;
            transform.Translate(disp);
            direction = GetDirectionFromDisp(disp.normalized);
            movingAnimation = true;
        }

        if (_animationManager != null)
        {
            if (movingAnimation)
            {
                _animationManager.playMovementAnimation(direction);
            }
            else
			{
                _animationManager.stopMovement();
            }
        }
    }

	private bool GetPlayerInput(out Vector3 destDirection)
	{
        bool input = false;
        destDirection = Vector3.zero;
        if (Input.GetMouseButton(0))
        {
            Vector2 mouseNormalizedScreenPos = Input.mousePosition / new Vector2(Screen.width, Screen.height);
            Vector2 playerNormalizedScreenPos = Camera.main.WorldToScreenPoint(transform.position) / new Vector2(Screen.width, Screen.height);
            Vector2 dir = mouseNormalizedScreenPos - playerNormalizedScreenPos;
            Vector3 dir3D = new Vector3(dir.x, 0.0f, dir.y);
            // Skip if distance less than _moveDistanceThreshold
            if (dir3D.magnitude >= _moveDistanceThreshold)
            {
                destDirection = dir3D.normalized;
                input = true;
            }
        }
        else
		{
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
			{
                destDirection.z = 1.0f;
                input = true;
            }
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                destDirection.z = -1.0f;
                input = true;
            }

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                destDirection.x = 1.0f;
                input = true;
            }
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                destDirection.x = -1.0f;
                input = true;
            }
        }
        return input;
	}

    private EDirections GetDirectionFromDisp(Vector3 disp)
	{
        float horizontalContribution = disp.x;
        float verticalContribution = disp.z;
        // First, check diagonal cases
        if (horizontalContribution > _diagonalDirectionThreshold && verticalContribution > _diagonalDirectionThreshold)
		{
            return EDirections.RIGHT_BACK;
		}
        else if (horizontalContribution > _diagonalDirectionThreshold && verticalContribution < -_diagonalDirectionThreshold)
		{
            return EDirections.RIGHT_FRONT;
		}
        else if (horizontalContribution < -_diagonalDirectionThreshold && verticalContribution > _diagonalDirectionThreshold)
        {
            return EDirections.LEFT_BACK;
        }
        else if (horizontalContribution < -_diagonalDirectionThreshold && verticalContribution < -_diagonalDirectionThreshold)
        {
            return EDirections.LEFT_FRONT;
        }
        else
		{
            // Not diagonal, find greater axis
            bool horizontalBigger = Mathf.Abs(horizontalContribution) > Mathf.Abs(verticalContribution);
            if (horizontalBigger)
			{
                return horizontalContribution > 0.0f ? EDirections.RIGHT : EDirections.LEFT;
			}
            else
			{
                return verticalContribution > 0.0f ? EDirections.BACK : EDirections.FRONT;
            }
		}
	}
}
