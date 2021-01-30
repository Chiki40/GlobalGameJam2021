using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSelectorManager : MonoBehaviour
{
    public GameObject _baseGameObject;
    public GameObject _scrollBaseGameObject;
    private List<TwitterManager.Tweet> tweets;

    public void Start()
    {
        tweets = new List<TwitterManager.Tweet>();
        Populate();
    }

    public void ClickButton(GameObject go)
    {
        Debug.Log("se ha pulsado el boton => " + go.name);
    }

    public void ClickBack()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Populate()
    {
        TwitterManager.GetInstance().GetTweets(10, callbackGetTweets);
    }

    private void callbackGetTweets(bool success, List<TwitterManager.Tweet> t)
    {
        for(int tweetId = 0; tweetId < t.Count; ++tweetId)
        {
            TwitterManager.Tweet tweet = t[tweetId];
            tweets.Add(tweet);
            //creamos el gameObject
            GameObject go = Instantiate<GameObject>(_baseGameObject);

            go.transform.name = tweetId.ToString();
            go.transform.SetParent(_scrollBaseGameObject.transform);

            if(tweet._imgs.Count > 0)
            {
                //buscamos la primera imagen que no sea qr
                bool someFotoAdded = false;
                for(int idImg =0; idImg < tweet._imgs.Count; ++idImg)
                {
                    Texture2D texture = tweet._imgs[idImg];
                    Sprite sp = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
                    
                    if (!tweet._isQR[idImg])
                    {
                        for(int childId = 0; childId < go.transform.childCount; ++childId)
                        {
                            if(go.transform.GetChild(childId).name == "Foto")
                            {
                                go.transform.GetChild(childId).GetComponent<Image>().sprite = sp;
                                someFotoAdded = true;
                            }
                        }
                    }
                    else
                    {
                        for (int childId = 0; childId < go.transform.childCount; ++childId)
                        {
                            if (go.transform.GetChild(childId).name == "QR")
                            {
                                go.transform.GetChild(childId).GetComponent<Image>().sprite = sp;
                            }
                        }
                    }
                }

                if (!someFotoAdded)
                {
                    for (int childId = 0; childId < go.transform.childCount; ++childId)
                    {
                        if (go.transform.GetChild(childId).name == "QR")
                        {
                            go.transform.GetChild(childId).gameObject.SetActive(false);
                        }
                        else
                        {
                            if (go.transform.GetChild(childId).name == "Foto")
                            {
                                Texture2D texture = tweet._imgs[0];
                                Sprite sp = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
                                go.transform.GetChild(childId).GetComponent<Image>().sprite = sp;
                            }
                        }
                    }
                }

            }
            
            go.GetComponentInChildren<TextMeshProUGUI>().text = t[tweetId]._msg;
        }
        _baseGameObject.SetActive(false);
    }
}
