using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Twity.DataModels.Core;
using Twity.DataModels.Responses;
using UnityEngine.Networking;
using System.Text;

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
    private List<string> _imagesToUpload;
    private List<string> _responsesIDsImages;

    public static List<string> defaultHashtags = new List<string>() { "#GlobalGameJam", "#GlobalJamUCM21", "#GGJ21" };
    public struct Tweet
    {
        public string _id;
        public List<Texture2D> _imgs;
        public string _msg;
        public List<string> _seedsImgs;
        public List<string> _urls;
        public List<bool> _isQR;
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

        Twity.Oauth.accessToken = access_token;
        Twity.Oauth.accessTokenSecret = access_token_secret;
        Twity.Oauth.bearerToken = beared_token;
        Twity.Oauth.consumerKey = consumer_key;
        Twity.Oauth.consumerSecret = consumer_key_secret;

        _imagesToUpload = new List<string>();
        _responsesIDsImages = new List<string>();
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
        parameters["lat"] = "20.060471";
        parameters["long"] = "-72.764193";
        parameters["in_reply_to_status_id"] = "1355280482079006721";
        StartCoroutine(Twity.Client.Post("statuses/update", parameters, CallbackTweet));
    }
    /// <summary>
    /// Envia un tweet de respuesta a otro tweet
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="idTweet"></param>
    /// <param name="_callback"></param>
    public void ResponseToTweet(string msg, string idTweet, SendTweetCallback _callback)
    {
        _callbackSendTweet = _callback;
        _msgSendImage = msg + "@PirataPalaverde";

        Dictionary<string, string> parameters = new Dictionary<string, string>();
        parameters["status"] = msg;
        parameters["lat"] = "20.060471";
        parameters["long"] = "-72.764193";
        parameters["in_reply_to_status_id"] = idTweet;
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
    #endregion
    #region Send Tweet With Image
    /// <summary>
    /// Publica un tweet con una imagen
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="_texture"></param>
    /// <param name="_callback"></param>
    public void SendTweetWithImage(string msg, Texture2D _texture, SendTweetCallback _callback)
    {
        _callbackSendTweet = _callback;
        _msgSendImage = msg;

        SendTweetWithImage(msg, _texture, null, _callback);
    }

    public void SendTweetWithImage(string msg, Texture2D _qr, Texture2D _foto, SendTweetCallback _callback)
    {
        _callbackSendTweet = _callback;
        _msgSendImage = msg;

        _responsesIDsImages.Clear();
        _imagesToUpload.Clear();

        List<Texture2D> _textures = new List<Texture2D>();
        if(_qr != null)
        {
            _textures.Add(_qr);
        }
        if(_foto != null)
        {
            _textures.Add(_foto);
        }

        for (int i = 0; i < _textures.Count; ++i)
        {
            _imagesToUpload.Add(System.Convert.ToBase64String(_textures[i].EncodeToPNG()));
        }

        for(int i = 0; i < _imagesToUpload.Count; ++i)
        {
            SendTweetWithImage(_imagesToUpload[i]);
        }
    }

    private void SendTweetWithImage(string txt)
    {
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        parameters["media"] = txt;
        StartCoroutine(Twity.Client.Post("media/upload", parameters, MediaUploadCallback));
    }

    private void MediaUploadCallback(bool success, string response)
    {
        if (success)
        {
            UploadMedia media = JsonUtility.FromJson<UploadMedia>(response);
            _responsesIDsImages.Add(media.media_id.ToString());

            if(_imagesToUpload.Count == _responsesIDsImages.Count)
            {
                StringBuilder sb = new StringBuilder();
                for(int i = 0; i < _responsesIDsImages.Count; ++i)
                {
                    sb.Append(_responsesIDsImages[i] + ",");
                }
                sb.Length--;
                //tenemos ya los ids de todas las imagenes
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                parameters["media_ids"] =sb.ToString();
                parameters["status"] = _msgSendImage;
                StartCoroutine(Twity.Client.Post("statuses/update", parameters, CallbackTweet));
            }
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
        parameters["exclude"] = "replies";
        StartCoroutine(Twity.Client.GetV2(path, parameters, CallbackGetTweets));
    }
    void CallbackGetTweets(bool success, string response)
    {
        StartCoroutine(CallbackGetTweets_Coroutine(success, response));
    }
    IEnumerator CallbackGetTweets_Coroutine(bool success, string response)
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
                    t._id = Response.data[tid].id;
                    t._msg = Response.data[tid].text;
                    t._imgs = new List<Texture2D>();
                    t._seedsImgs = new List<string>();
                    t._urls = getAllUrls(Response, tid);
                    t._isQR = new List<bool>();//es una lista para que lo rellene la coroutine
                    yield return StartCoroutine(DownloadAllImages(t._urls, t._imgs, t._seedsImgs, t._isQR));
                    if(t._seedsImgs.Count > 0)
                    {
                        objectResponse.Add(t);
                    }
                }
            }
        }
        else
        {
            Debug.LogError(response);
        }
        if (_callbackGetTweets != null)
        {
            _callbackGetTweets(success, objectResponse);
        }
    }
    IEnumerator DownloadAllImages(List<string> urls,  List<Texture2D> list, List<string> seedTxt, List<bool> idQR)
    {
        for (int i = 0; i < urls.Count; ++i)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(urls[i]);
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
                Debug.Log(request.error);
            else
            {
                Texture2D t = ((DownloadHandlerTexture)request.downloadHandler).texture;
                list.Add(t);
                try
                {
                    seedTxt.Add(QrReader.readQR(t));
                    idQR.Add(true);
                }
                catch (System.Exception e)
                {
                    idQR.Add(false);
                }
            }
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
                string idTweetMedia = _response.data[id].attachments.media_keys[mediaId];
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
