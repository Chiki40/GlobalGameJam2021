using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public enum EDirections { LEFT, RIGHT, BACK, FRONT, LEFT_BACK, RIGHT_BACK, LEFT_FRONT, RIGHT_FRONT };

    [SerializeField]
    float _speed = 1.0f;
    [SerializeField]
    float _basicDirdectionThreshold = 0.1f;
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
        if (GetPlayerInput(out Vector3 targetPos))
        {
            Vector3 toTargetVector = targetPos - transform.position;
            float distance = toTargetVector.magnitude;
            // Skip if distance less than _moveDistanceThreshold
            if (distance > _moveDistanceThreshold)
            {
                // Distance to move is clamped by distance to target
                float distanceToMove = Mathf.Min(_speed * Time.deltaTime, distance);
                Vector2 disp = toTargetVector.normalized * distanceToMove;
                transform.Translate(disp);
                direction = GetDirectionFromDisp(disp.normalized);
                movingAnimation = true;
            }
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

    private bool GetPlayerInput(out Vector3 destPos)
	{
        bool input = false;
        destPos = transform.position;
        if (Input.GetMouseButton(0))
        {
            float previousZ = destPos.z;
            destPos = Input.mousePosition;
            destPos.z = previousZ;
            destPos = Camera.main.ScreenToWorldPoint(destPos);
            destPos.z = previousZ;
            input = true;
        }
        else
		{
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
			{
                destPos.y += 10.0f;
                input = true;
            }
            else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                destPos.y -= 10.0f;
                input = true;
            }

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                destPos.x += 10.0f;
                input = true;
            }
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                destPos.x -= 10.0f;
                input = true;
            }
        }
        return input;
	}

    private EDirections GetDirectionFromDisp(Vector2 disp)
	{
        float horizontalContribution = disp.x;
        float verticalContribution = disp.y;
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
        else if (horizontalContribution > _basicDirdectionThreshold)
		{
            return EDirections.RIGHT;
		}
        else if (horizontalContribution < -_basicDirdectionThreshold)
        {
            return EDirections.LEFT;
        }
        else if (verticalContribution > _basicDirdectionThreshold)
        {
            return EDirections.BACK;
        }
        else if (verticalContribution < -_basicDirdectionThreshold)
        {
            return EDirections.FRONT;
        }
        return EDirections.LEFT;
	}
}
