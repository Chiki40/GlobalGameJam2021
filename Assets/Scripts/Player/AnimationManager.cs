using UnityEngine;

[RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
public class AnimationManager : MonoBehaviour {

    public enum AnimationsAllowed  {WALK_LEFT, WALK_RIGHT, STOP, NONE};

    private SpriteRenderer _renderer;
    private Animator _animationController;

    private static string _walkKey = "Walk";
    private static string _resetKey = "Reset";

    private bool? actuallyWalkingLeft = null;

    public delegate void  AnimationCallback();

    public bool soundWood;

    void Start() {
        _renderer = GetComponent<SpriteRenderer>();
        _animationController = GetComponent<Animator>();
    }

    #region specific Animations
    private void WALK(bool left) {
        _animationController.SetBool(_walkKey, true);
        _renderer.flipX = left;
    }
    private void STOP()
    {
        _animationController.SetBool(_walkKey, false);
    }
    #endregion

    #region public actions
    public void playAnimation(AnimationsAllowed animation)
    {
        if(actuallyWalkingLeft.HasValue &&
            ((actuallyWalkingLeft.Value && animation == AnimationsAllowed.WALK_LEFT) || (!actuallyWalkingLeft.Value && animation == AnimationsAllowed.WALK_RIGHT)))
        {
            return;
        }

        switch (animation) 
        {
            case AnimationsAllowed.WALK_LEFT:
            {
                actuallyWalkingLeft = true;
                WALK(true);
                break;
            }
            case AnimationsAllowed.WALK_RIGHT:
            {
                actuallyWalkingLeft = false;
                WALK(false);
                break;
            }
            case AnimationsAllowed.STOP:
            {
                actuallyWalkingLeft = null;
                STOP();
                break;
            }
        }
    }
    public void RESET()
    {
        _animationController.SetBool(_walkKey, false);
        _animationController.SetTrigger(_resetKey);
        actuallyWalkingLeft = null;
    }
    #endregion
}
