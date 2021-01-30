using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CompartirMapaManager : MonoBehaviour
{
    public TMP_InputField _textArea;
    private Texture2D _imageTweet;
    private string _mapaStr;
    public void ShowCompartirMapa(Texture2D imageTweet, string mapDataStr)
    {
        this.gameObject.SetActive(true);
        this._mapaStr = mapDataStr;
        _imageTweet = imageTweet;
    }

    private void Start()
    {
        this.gameObject.SetActive(false);
    }

    public void OnCompartir()
    {
        string userTwitter = _textArea.text;
        string msgTwitter = "Creado anonimamente";
        //si no tiene @, se la añado
        if(userTwitter.Length > 0)
        {
            if (userTwitter[0] != '@')
            {
                userTwitter = "@" + userTwitter;
            }
            msgTwitter = "creado por: " + userTwitter;
        }
        Debug.Log("mensaje twitter => " + msgTwitter);

        TwitterManager tm = TwitterManager.GetInstance();
        Texture2D qr = QrReader.generateQR(_mapaStr);
        tm.SendTweetWithImage(msgTwitter, qr, _imageTweet, callbackSendTweet);
    }

    private void callbackSendTweet(bool success, string idTweet)
    {
        if (success)
        {
            Debug.Log("he publicado el tweet bien => " + idTweet);
            this.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("he publicado el tweet mal");
        }
    }

    public void OnAtras()
    {
        this.gameObject.SetActive(false);
    }

}
