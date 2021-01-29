using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTwitter : MonoBehaviour
{
    public Texture2D _texture;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            TwitterManager.GetInstance().SendTweet("hola desde twitter" + Random.Range(0, 1000), callbackSendTweet);
        }

        if(Input.GetKeyDown(KeyCode.H))
        {
            TwitterManager.GetInstance().GetTweets(100, callbackGetTweets);
        }

        if(Input.GetKeyDown(KeyCode.I))
        {
            byte[] b = _texture.EncodeToPNG();
            TwitterManager.GetInstance().SendTweetWithImage("hola con imagen " + Random.Range(0, 1000), b,callbackSendTweet);
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
            Debug.Log("he recibido bien todos los tweets");

            for(int i = 0; i < t.Count; ++i)
            {
                Debug.Log("ID =>" + t[i].id);
            }
        }
        else
        {
            Debug.Log("No he recibido bien todos los tweets");
        }
    }
}
