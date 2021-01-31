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

    private void Start()
    {
        tweets = new List<TwitterManager.Tweet>();
        Populate();
    }

	private void OnEnable()
	{
        GameManager.MapToLoad = null;
        GameManager.SpritePhoto = null;
    }

	public void ClickButton(GameObject go)
    {
        Debug.Log("se ha pulsado el boton => " + go.name);
        string mapDataStr = tweets[int.Parse(go.name)]._seedsImgs[0];

        Sprite sprite = null;
        for (int childId = 0; childId < go.transform.childCount; ++childId)
        {
            if (go.transform.GetChild(childId).name == "Foto")
            {
                sprite = go.transform.GetChild(childId).GetComponent<Image>().sprite;
            }
        }
        if (sprite != null)
		{
            GameManager.SpritePhoto = sprite;
        }

        // Use PlayerPref to propagate map data to next scene
        GameManager.MapToLoad = mapDataStr;
        GameManager.IdTweet = tweets[int.Parse(go.name)]._id;
        SceneManager.LoadScene("GamePlayMode");
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
            GameObject go = Instantiate(_baseGameObject, _scrollBaseGameObject.transform);

            go.transform.name = tweetId.ToString();
            go.SetActive(true);
            Button button = go.GetComponent<Button>();
            if (button != null)
			{
                button.enabled = true;
			}

            if (tweet._imgs.Count > 0)
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
            }
            int lenght = t[tweetId]._msg.IndexOf(TwitterManager.defaultHashtags);
            string msg = t[tweetId]._msg;
            if(lenght >0)
            {
                msg = t[tweetId]._msg.Substring(0, lenght);
            }
            go.GetComponentInChildren<TextMeshProUGUI>().text = msg;
        }
        _baseGameObject.SetActive(false);
    }
}
