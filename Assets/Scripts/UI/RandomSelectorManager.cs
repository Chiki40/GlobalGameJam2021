using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RandomSelectorManager : MonoBehaviour
{
    private void Start()
    {
        Populate();
    }

    public void Populate()
    {
        TwitterManager.GetInstance().GetTweets(10, callbackGetTweets);
    }

    private void callbackGetTweets(bool success, List<TwitterManager.Tweet> t)
    {
        bool findSomething = false;
        int totalCount = 0;
        while (!findSomething && totalCount < 10)
        {
            totalCount++;
            int tweetID = Random.Range(0, t.Count);

            TwitterManager.Tweet tweet = t[tweetID];

            string mapDataStr = tweet._seedsImgs[0];

            Sprite sprite = null;

            if (tweet._imgs.Count > 0)
            {
                //buscamos la primera imagen que no sea qr
                for (int idImg = 0; idImg < tweet._imgs.Count; ++idImg)
                {
                    Texture2D texture = tweet._imgs[idImg];
                    Sprite sp = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);

                    if (!tweet._isQR[idImg])
                    {
                        GameManager.SpritePhoto = sp;
                        findSomething = true;
                    }
                }
            }

            // Use PlayerPref to propagate map data to next scene
            GameManager.MapToLoad = mapDataStr;
            GameManager.IdTweet = t[tweetID]._id;
            SceneManager.LoadScene("GamePlayMode");
        }
    }
}
