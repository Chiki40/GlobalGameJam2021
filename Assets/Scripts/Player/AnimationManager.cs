using UnityEngine;

[RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
public class AnimationManager : MonoBehaviour {

    public enum AnimationsAllowed  {WALK_HORIZONTAL, WALK_VERTICAL, WALK_DIAGONAL_FRONT, WALK_DIAGONAL_BACK, STOP, NONE};

    private SpriteRenderer _renderer;
    private Animator _animationController;

    private static string kWalkHorizontalKey = "WalkHorizontal";
    private static string kWalkVerticalKey = "WalkVertical";
    private static string kWalkDiagonalFrontKey = "WalkDiagonalFront";
    private static string kWalkDiagonalBackKey = "WalkDiagonalBack";
    private static string _resetKey = "Reset";

    private CharacterController.EDirections? actuallyWalkingDirection = null;

    public delegate void  AnimationCallback();

    public bool soundWood;

    void Start() {
        _renderer = GetComponent<SpriteRenderer>();
        _animationController = GetComponent<Animator>();
    }

	private void OnEnable()
	{
        actuallyWalkingDirection = null;
    }

	#region specific Animations
	private void WALK(CharacterController.EDirections direction) {
        switch (direction)
		{
            case CharacterController.EDirections.LEFT:
                _animationController.SetBool(kWalkHorizontalKey, true);
                _animationController.SetBool(kWalkVerticalKey, false);
                _animationController.SetBool(kWalkDiagonalFrontKey, false);
                _animationController.SetBool(kWalkDiagonalBackKey, false);
                _renderer.flipX = true;
                break;
            case CharacterController.EDirections.RIGHT:
                _animationController.SetBool(kWalkHorizontalKey, true);
                _animationController.SetBool(kWalkVerticalKey, false);
                _animationController.SetBool(kWalkDiagonalFrontKey, false);
                _animationController.SetBool(kWalkDiagonalBackKey, false);
                _renderer.flipX = false;
                break;
            case CharacterController.EDirections.UP:
                _animationController.SetBool(kWalkHorizontalKey, false);
                _animationController.SetBool(kWalkVerticalKey, true);
                _animationController.SetBool(kWalkDiagonalFrontKey, false);
                _animationController.SetBool(kWalkDiagonalBackKey, false);
                _renderer.flipX = false;
                break;
            case CharacterController.EDirections.DOWN:
                _animationController.SetBool(kWalkHorizontalKey, false);
                _animationController.SetBool(kWalkVerticalKey, true);
                _animationController.SetBool(kWalkDiagonalFrontKey, false);
                _animationController.SetBool(kWalkDiagonalBackKey, false);
                _renderer.flipX = false;
                break;
            case CharacterController.EDirections.LEFT_UP:
                _animationController.SetBool(kWalkHorizontalKey, false);
                _animationController.SetBool(kWalkVerticalKey, false);
                _animationController.SetBool(kWalkDiagonalFrontKey, false);
                _animationController.SetBool(kWalkDiagonalBackKey, true);
                _renderer.flipX = true;
                break;
            case CharacterController.EDirections.LEFT_DOWN:
                _animationController.SetBool(kWalkHorizontalKey, false);
                _animationController.SetBool(kWalkVerticalKey, false);
                _animationController.SetBool(kWalkDiagonalFrontKey, true);
                _animationController.SetBool(kWalkDiagonalBackKey, false);
                _renderer.flipX = true;
                break;
            case CharacterController.EDirections.RIGHT_UP:
                _animationController.SetBool(kWalkHorizontalKey, false);
                _animationController.SetBool(kWalkVerticalKey, false);
                _animationController.SetBool(kWalkDiagonalFrontKey, false);
                _animationController.SetBool(kWalkDiagonalBackKey, true);
                _renderer.flipX = false;
                break;
            case CharacterController.EDirections.RIGHT_DOWN:
                _animationController.SetBool(kWalkHorizontalKey, false);
                _animationController.SetBool(kWalkVerticalKey, false);
                _animationController.SetBool(kWalkDiagonalFrontKey, true);
                _animationController.SetBool(kWalkDiagonalBackKey, false);
                _renderer.flipX = false;
                break;
        }
    }
    private void STOP()
    {
        _animationController.SetBool(kWalkHorizontalKey, false);
        _animationController.SetBool(kWalkVerticalKey, false);
        _animationController.SetBool(kWalkDiagonalFrontKey, false);
        _animationController.SetBool(kWalkDiagonalBackKey, false);
    }
    #endregion

    #region public actions
    public void playMovementAnimation(CharacterController.EDirections direction)
    {
        if (actuallyWalkingDirection.HasValue && actuallyWalkingDirection.Value == direction)
        {
            return;
        }

        actuallyWalkingDirection = direction;
        WALK(direction);
    }

    public void stopMovement()
	{
        if (!actuallyWalkingDirection.HasValue)
        {
            return;
        }

        actuallyWalkingDirection = null;
        STOP();
    }

    public void RESET()
    {
        actuallyWalkingDirection = null;

        _animationController.SetBool(kWalkHorizontalKey, false);
        _animationController.SetBool(kWalkVerticalKey, false);
        _animationController.SetBool(kWalkDiagonalFrontKey, false);
        _animationController.SetBool(kWalkDiagonalBackKey, false);
        _animationController.SetTrigger(_resetKey);
    }
    #endregion
}
