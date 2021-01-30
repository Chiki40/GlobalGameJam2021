using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterController : MonoBehaviour
{
    private const string kDigAnimationTag = "Dig";
    public enum EDirections { LEFT, RIGHT, BACK, FRONT, LEFT_BACK, RIGHT_BACK, LEFT_FRONT, RIGHT_FRONT };

    [SerializeField]
    float _speed = 1.0f;
    [SerializeField]
    float _diagonalDirectionThreshold = 0.3f;
    [SerializeField]
    float _moveDistanceThreshold = 1.0f;

    private bool _inputEnabled = true;

    private PointerEventData _cachedPointerData = null;

    private AnimationManager _animationManager = null;
    private Animator _animator = null;

    private void Awake()
    {
        _animationManager = this.GetComponent<AnimationManager>();
        _animator = this.GetComponent<Animator>();
    }

	private void OnEnable()
	{
        _inputEnabled = true;
        _cachedPointerData = new PointerEventData(EventSystem.current);
    }

	private void Update()
    {
        EDirections direction = EDirections.LEFT;
        bool movingAnimation = false;
        if (GetPlayerInput(out bool dig, out Vector3 targetDir))
        {
            if (dig)
            {
                _animationManager.PerformDig();
                return;
            }
            else
            {
                float distanceToMove = _speed * Time.deltaTime;
                Vector3 disp = targetDir.normalized * distanceToMove;
                // Double check Y component is 0
                disp.y = 0.0f;
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

	private bool GetPlayerInput(out bool dig, out Vector3 destDirection)
	{
        dig = false;
        destDirection = Vector3.zero;
        bool input = false;

        // Input must be enabled and character is not digging
        if (_inputEnabled && !_animator.GetCurrentAnimatorStateInfo(0).IsTag(kDigAnimationTag))
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E))
            {
                dig = true;
                input = true;
            }
            else if (Input.GetMouseButton(0))
            {
                // Ignore if mouse is over UI
                _cachedPointerData.position = Input.mousePosition;
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(_cachedPointerData, results);
                if (results.Count == 0)
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

    public void EnableInput(bool active)
	{
        _inputEnabled = active;
	}
}
