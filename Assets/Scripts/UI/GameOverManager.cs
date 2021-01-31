using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    [SerializeField]
    private GamePlayModeController _gameplayModeController = null;

    public TMPro.TextMeshProUGUI _text;

    public void Start()
    {
        this.gameObject.SetActive(false);
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void Salir()
    {
        UtilSound.instance.PlaySound("BUTTON_Click_Compressor_stereo");
        this.gameObject.SetActive(false);
        if (_gameplayModeController != null)
        {
            _gameplayModeController.StartCoroutine(_gameplayModeController.ResetCoroutine());
        }
    }
}
