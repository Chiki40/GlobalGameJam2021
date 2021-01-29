using System;
using Twity.DataModels.Core;

namespace Twity.DataModels.Responses
{
    [Serializable]
    public class Attachments
    {
        public string[] media_keys;
    }

    [Serializable]
    public class NewTweet
    {
        public string id;
        public string text;
        public Attachments attachments;
    }

    [Serializable]
    public class MediaData
    {
        public string media_key;
        public string type;
        public string url;
    }

    [Serializable]
    public class Include
    {
        public MediaData[] media;
    }

    [Serializable]
    public class StatusesHomeTimelineResponse
    {
        public NewTweet[] data;
        public Include includes;
    }

    public class TweetInfo
    {
        public string id;

    }
}
