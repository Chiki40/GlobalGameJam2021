using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
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
        if (_videoPlayer != null)
		{
            if (!_videoPlayer.isPlaying || Input.GetKeyDown(KeyCode.Escape))
            {
                _onFinishEvent?.Invoke();
            }
		}
    }
}
