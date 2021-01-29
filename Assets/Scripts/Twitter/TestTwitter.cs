using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTwitter : MonoBehaviour
{
    public Texture2D _texture;
    public GameObject _plane;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            TwitterManager.GetInstance().SendTweet("hola desde twitter " + Random.Range(0, 1000), callbackSendTweet);
        }

        if(Input.GetKeyDown(KeyCode.H))
        {
            TwitterManager.GetInstance().GetTweets(100, callbackGetTweets);
        }

        if(Input.GetKeyDown(KeyCode.I))
        {
            Texture2D t = QrReader.generateQR("bloste para ti y todas tus amigas y amigos");
            TwitterManager.GetInstance().SendTweetWithImage("hola con imagen " + Random.Range(0, 1000), t, callbackSendTweet);
        }
    }

    private void callbackSendTweet(bool success, string idTweet)
    {
        if(success)
        {
            Debug.Log("he publicado el tweet bien => "+ idTweet);
        }
        else
        {
            Debug.Log("he publicado el tweet mal");
        }
    }

    private void callbackGetTweets(bool success, List<TwitterManager.Tweet> t)
    {
        if(success)
        {
            _plane.GetComponent<Renderer>().material.mainTexture = t[0].imgs[0];
            string texto = QrReader.readQR(t[0].imgs[0]);
            Debug.Log("he leido el texto =>" +  texto);
        }
        else
        {
            Debug.Log("No he recibido bien todos los tweets");
        }
    }
}
