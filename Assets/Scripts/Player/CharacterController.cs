using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public enum EDirections { LEFT, RIGHT, UP, DOWN, LEFT_UP, RIGHT_UP, LEFT_DOWN, RIGHT_DOWN };

    [SerializeField]
    float _speed = 1.0f;
    [SerializeField]
    float _basicDirdectionThreshold = 0.1f;
    [SerializeField]
    float _diagonalDirectionThreshold = 0.3f;
    [SerializeField]
    float _useAnimationDistanceThreshold = 1.0f;
    [SerializeField]
    float _moveDistanceThreshold = 1.0f;

    private AnimationManager _animationManager = null;

    private void Awake()
    {
        _animationManager = this.GetComponent<AnimationManager>();
        if (_moveDistanceThreshold > _useAnimationDistanceThreshold)
		{
            Debug.LogError("[CharacterController.Awake] ERROR. _moveDistanceThreshold > _useAnimationDistanceThreshold makes no sense");
		}
    }

	private void Update()
    {
        EDirections direction = EDirections.LEFT;
        bool movingAnimation = false;
        if (Input.GetMouseButton(0))
        {
            float previousZ = transform.position.z;
            var pos = Input.mousePosition;
            pos.z = previousZ;
            pos = Camera.main.ScreenToWorldPoint(pos);
            pos.z = previousZ;

            Vector3 toTargetVector = pos - transform.position;
            float distance = toTargetVector.magnitude;
            // Skip if distance less than _moveDistanceThreshold
            if (distance > _moveDistanceThreshold)
            {
                // Distance to move is clamped by distance to target
                float distanceToMove = Mathf.Min(_speed * Time.deltaTime, distance);
                Vector2 disp = toTargetVector.normalized * distanceToMove;
                transform.Translate(disp);
                // Skip animation is distance less than _useAnimationDistanceThreshold
                if (distance > _useAnimationDistanceThreshold)
                {

                    direction = GetDirectionFromDisp(disp.normalized);
                    movingAnimation = true;
                }
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

    private EDirections GetDirectionFromDisp(Vector2 disp)
	{
        float horizontalContribution = disp.x;
        float verticalContribution = disp.y;
        if (horizontalContribution > _diagonalDirectionThreshold && verticalContribution > _diagonalDirectionThreshold)
		{
            return EDirections.RIGHT_UP;
		}
        else if (horizontalContribution > _diagonalDirectionThreshold && verticalContribution < -_diagonalDirectionThreshold)
		{
            return EDirections.RIGHT_DOWN;
		}
        else if (horizontalContribution < -_diagonalDirectionThreshold && verticalContribution > _diagonalDirectionThreshold)
        {
            return EDirections.LEFT_UP;
        }
        else if (horizontalContribution < -_diagonalDirectionThreshold && verticalContribution < -_diagonalDirectionThreshold)
        {
            return EDirections.LEFT_DOWN;
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
            return EDirections.UP;
        }
        else if (verticalContribution < -_basicDirdectionThreshold)
        {
            return EDirections.DOWN;
        }
        return EDirections.LEFT;
	}
}
