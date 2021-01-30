using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VerTesoroManager : MonoBehaviour
{
    public TMP_Text _textoTesoro;
    public TMP_InputField _userTwitter;
    private string _idTweet;

    public void Start()
    {
        this.gameObject.SetActive(false);
    }

    public void ShowTesoro(string txt, string idTweet)
    {
        this.gameObject.SetActive(true);
        _textoTesoro.text = txt;
        _idTweet = idTweet;
    }

    public void PublicarTweet()
    {
        string user = _userTwitter.text;
        if(user.Length > 0)
        {
            TwitterManager.GetInstance().ResponseToTweet(user, _idTweet, callbackSendTweet);
        }
        Salir();
    }

    public void Salir()
    {
        this.gameObject.SetActive(false);
        SceneManager.LoadScene("MainMenu");
    }

    private void callbackSendTweet(bool success, string idTweet)
    {
        if (success)
        {
            Debug.Log("he publicado el tweet bien => " + idTweet);
        }
        else
        {
            Debug.Log("he publicado el tweet mal");
        }
    }

}
