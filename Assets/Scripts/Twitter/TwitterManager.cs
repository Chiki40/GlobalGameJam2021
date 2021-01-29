using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Twity.DataModels.Core;
using Twity.DataModels.Responses;

public class TwitterManager : MonoBehaviour
{
    #region SECRET
    private string beared_token = "AAAAAAAAAAAAAAAAAAAAAH%2FzMAEAAAAARUX5R7l6w0rmR7rAy%2Bj8MSvNZiw%3D9Er7pNltWzk2VmdL8BL5ewJGX1iELAsyyJ0oDIipnseP70rUuR";
    private string access_token = "1354883396510560256-ym71xxqcnzg522LaB9npw8ZjIfhPXd";
    private string access_token_secret = "Eo9nBqryfAqdueRQEWRnFq5AiRenroziiIGI9DmJtpP4f";
    private string user_id = "1354883396510560256";
    private string consumer_key = "tPQ0qItYMBerVZYHoyWth7GNy";
    private string consumer_key_secret = "tVh4G5btcms85bFfSgEUQblrol7zxlwybuXUA6hHTHC6lm3iRQ";
    #endregion

    public delegate void SendTweetCallback(bool success, string idTweet);
    public event SendTweetCallback _callbackSendTweet;
    private string _msgSendImage;

    public struct Tweet
    {
        public string id;
        public List<string> url;
        public string msg;
    }
    public delegate void GetTweetsCallback(bool success, List<Tweet> t);
    public event GetTweetsCallback _callbackGetTweets;

    #region SINGLETTON
    private static TwitterManager _instance = null;
    public static TwitterManager GetInstance()
    {
        return _instance;
    }

    public void Start()
    {
        if (_instance != null)
        {
            Destroy(this.gameObject);
        }
        _instance = this;
        DontDestroyOnLoad(this.gameObject);

        Twity.Oauth.accessToken = access_token;
        Twity.Oauth.accessTokenSecret = access_token_secret;
        Twity.Oauth.bearerToken = beared_token;
        //no se usan
        Twity.Oauth.consumerKey = consumer_key;
        Twity.Oauth.consumerSecret = consumer_key_secret;
    }
    #endregion
    #region Send Tweet
    /// <summary>
    /// public un Tweet
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="_callback"></param>
    public void SendTweet(string msg, SendTweetCallback _callback)
    {
        _callbackSendTweet = _callback;
        _msgSendImage = msg;

        Dictionary<string, string> parameters = new Dictionary<string, string>();
        parameters["status"] = msg;
        StartCoroutine(Twity.Client.Post("statuses/update", parameters, CallbackTweet));
    }
    private void CallbackTweet(bool success, string response)
    {
        if (_callbackSendTweet != null)
        {
            TweetInfo Response = JsonUtility.FromJson<TweetInfo>(response);

            _callbackSendTweet(success, Response.id);
        }
    }

    public void SendTweetWithImage(string msg, byte[] img, SendTweetCallback _callback)
    {
        _callbackSendTweet = _callback;
        _msgSendImage = msg;

        string imgbase64 = System.Convert.ToBase64String(img);
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        parameters["media"] = imgbase64;
        StartCoroutine(Twity.Client.Post("media/upload", parameters, MediaUploadCallback));
    }
    private void MediaUploadCallback(bool success, string response)
    {
        if (success)
        {
            UploadMedia media = JsonUtility.FromJson<UploadMedia>(response);
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters["media_ids"] = media.media_id.ToString();
            parameters["status"] = _msgSendImage;
            StartCoroutine(Twity.Client.Post("statuses/update", parameters, CallbackTweet));
        }
        else
        {
            Debug.Log(response);
        }
    }


    #endregion
    #region Get Tweets
    /// <summary>
    /// Devuelve todos los tweets que ha publicado PalaVerde
    /// </summary>
    /// <param name="totalTweets"></param>
    /// <param name="_callback"></param>
    public void GetTweets(int totalTweets, GetTweetsCallback _callback)
    {
        _callbackGetTweets = _callback;
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        string path = "users/" + user_id + "/tweets";
        parameters["max_results"] = totalTweets.ToString();
        parameters["tweet.fields"] = "attachments";
        parameters["expansions"] = "attachments.media_keys";
        parameters["media.fields"] = "url";
        StartCoroutine(Twity.Client.GetV2(path, parameters, CallbackGetTweets));
    }
    void CallbackGetTweets(bool success, string response)
    {
        List<Tweet> objectResponse = new List<Tweet>();
        if (success)
        {
            StatusesHomeTimelineResponse Response = JsonUtility.FromJson<StatusesHomeTimelineResponse>(response);

            if (Response.data != null)
            {
                for (int tid = 0; tid < Response.data.Length; ++tid)
                {
                    Tweet t;
                    t.id = Response.data[tid].id;
                    t.msg = Response.data[tid].text;
                    t.url = getAllUrls(Response, tid);
                    objectResponse.Add(t);
                }
            }
        }
        if (_callbackGetTweets != null)
        {
            _callbackGetTweets(success, objectResponse);
        }
    }

    private List<string> getAllUrls(StatusesHomeTimelineResponse _response, int id)
    {
        List<string> result = new List<string>();

        if (_response.data[id].attachments.media_keys != null)
        {
            int totalMedia = _response.data[id].attachments.media_keys.Length;
            for (int mediaId = 0; mediaId < totalMedia; ++mediaId)
            {
                string idTweetMedia = _response.data[0].attachments.media_keys[mediaId];
                if (_response.includes != null)
                {
                    for (int i = 0; i < _response.includes.media.Length; ++i)
                    {
                        if (_response.includes.media[i].media_key == idTweetMedia)
                        {
                            result.Add(_response.includes.media[i].url);
                        }
                    }
                }
            }
        }

        return result;
    }

    #endregion
}
