using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI _text = null;

	private float _currentTime = 0.0f;

	private void OnEnable()
	{
		_currentTime = 0.0f;
	}

	private void Update()
	{
		_currentTime += Time.deltaTime;
		if (_text)
		{
			_text.text = string.Format("{0:0.0}", _currentTime);
		}
	}
}
