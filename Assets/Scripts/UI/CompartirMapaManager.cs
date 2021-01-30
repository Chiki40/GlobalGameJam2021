using System;
using TMPro;
using UnityEngine;

public class CompartirMapaManager : MonoBehaviour
{
    public TMP_InputField _creador;
    public TMP_InputField _mensaje;
    private Texture2D _imageTweet;
    private MapData _mapData;
    Action _onFinishCallback = null;
    public void ShowCompartirMapa(Texture2D imageTweet, MapData mapData, Action onFinishCallback=null)
    {
        this.gameObject.SetActive(true);
        this._mapData = mapData;
        _imageTweet = imageTweet;
        _onFinishCallback = onFinishCallback;
    }

    private void Start()
    {
        this.gameObject.SetActive(false);
    }

    public void OnCompartir()
    {
        string userTwitter = _creador.text;
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
        msgTwitter += "\n" + "#GlobalGameJam #GlobalJamUCM21";

        TwitterManager tm = TwitterManager.GetInstance();
        _mapData.message = _mensaje.text;
        string mapDataStr = Serializator.XmlSerialize<MapData>(_mapData);
        Texture2D qr = QrReader.generateQR(mapDataStr);
        tm.SendTweetWithImage(msgTwitter, qr, _imageTweet, callbackSendTweet);
    }

    private void callbackSendTweet(bool success, string idTweet)
    {
        if (success)
        {
            Debug.Log("he publicado el tweet bien => " + idTweet);
            this.gameObject.SetActive(false);
            _onFinishCallback.Invoke();
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
