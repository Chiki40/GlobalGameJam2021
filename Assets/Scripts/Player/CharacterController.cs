using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField]
    float _speed = 1.0f;
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
        bool left = false;
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
                Vector3 disp = toTargetVector.normalized * distanceToMove;
                transform.Translate(disp);
                // Skip animation is distance less than _useAnimationDistanceThreshold
                if (distance > _useAnimationDistanceThreshold)
                {
                    left = disp.x < 0.0f;
                    movingAnimation = true;
                }
            }
        }

        if (_animationManager != null)
        {
            if (movingAnimation)
            {
                _animationManager.playAnimation(left ? AnimationManager.AnimationsAllowed.WALK_LEFT : AnimationManager.AnimationsAllowed.WALK_RIGHT);
            }
            else
			{
                _animationManager.playAnimation(AnimationManager.AnimationsAllowed.STOP);
            }
        }
    }
}
