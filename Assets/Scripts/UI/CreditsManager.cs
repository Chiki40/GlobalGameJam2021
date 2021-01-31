using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

public class CreditsManager : MonoBehaviour
{
    [SerializeField]
    private UnityEvent _onFinishEvent = null;

    VideoPlayer _videoPlayer = null;

    void Awake()
    {
        _videoPlayer = GetComponent<VideoPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_videoPlayer != null && !_videoPlayer.isPlaying)
		{
            _onFinishEvent?.Invoke();
		}
    }
}
