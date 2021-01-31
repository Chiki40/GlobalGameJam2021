using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI _text = null;

	private GamePlayModeController _gamePlaymanager = null;

	private float _currentTime = 0.0f;

	private void Start()
	{
		_gamePlaymanager = FindObjectOfType<GamePlayModeController>();
	}

	private void OnEnable()
	{
		_currentTime = 0.0f;
	}

	private void Update()
	{
		if (_gamePlaymanager == null || !_gamePlaymanager.GameInProgress)
		{
			return;
		}
		_currentTime += Time.deltaTime;
		if (_text)
		{
			_text.text = string.Format("{0:0.0}", _currentTime);
		}
	}
}
