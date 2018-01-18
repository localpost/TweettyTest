using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TweetSharp;
using System.Net;

namespace TwittyTest
{
    class TwitService
    {
        private TwitterService tService;

        public TwitService(string ConsumerKey, string ConsumerSecret, string TokenKey, string TokenSecret)
        {
            tService = new TwitterService(ConsumerKey, ConsumerSecret, TokenKey, TokenSecret);
            //getTweetsHandler = new GetStats(GetTweets);
        }        

        //Отправить статус в Твиттер
        public bool SendTweet(string _sendTo, string _message)
        {
            bool sendStatus = false;

            if (_sendTo[0] != '@')
            {
                _sendTo = "@" + _sendTo.Replace(" ", "");
            }

            tService.SendTweet(new SendTweetOptions { Status = _sendTo + ", статистика для последних 5 твитов:" + _message }, (tweet, response) =>
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    sendStatus = true;
                }
            });

            return sendStatus;
        }

        //Получить последние N-число статусов твиттера _userName
        public List<string> GetTweets(string _userName, int _count)
        {            
            if (_count > 0)
            {
                try
                {
                    List<string> result = new List<string>();
                    //Поиск Id пользователя по имени _userName
                    long userId = tService.GetUserProfileFor(new GetUserProfileForOptions { ScreenName = _userName }).Id;

                    //Поиск N-число твиттов пользователя c UserID
                    IEnumerable<TwitterStatus> ListTweets = tService.ListTweetsOnUserTimeline(new ListTweetsOnUserTimelineOptions
                    {
                        UserId = userId,
                        Count = _count
                    });

                    foreach (var item in ListTweets)
                    {
                        result.Add(item.Text);
                    }
                    return result;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
            
        }

        //Получить статистку по частотности символов в строке
        public Dictionary<char, double> GetStatystics(char[] _dictionary, List<string> _listOfTweets)
        {
            Dictionary<char, double> result = new Dictionary<char, double>();

            for(int i=0; i<_dictionary.Length;i++)
            {
                char item = Char.ToLower(_dictionary[i]);
                double count = 0;
                double allTweetsLetters = 0;
                double frequency = 0;

                foreach (string tweet in _listOfTweets)
                {
                    count += tweet.ToLower().Count(letter => letter == item);
                    allTweetsLetters += tweet.Count(char.IsLetter);
                }

                frequency = count / allTweetsLetters;

                result.Add(item, Math.Round(frequency,2));
            }

            return result;
        }

        //Преобразование Dictionary в JSON
        public string DictionaryToJSON(Dictionary<char, double> _dictionary)
        {
            string result = "{";
            foreach(var item in _dictionary)
            {
                result += item.Key.ToString() + ":'" + item.Value.ToString().Replace(",", ".") + "',";
            }
            if(result.Length>1)
            {
                result = result.Substring(0, result.Length - 1);
            }
            result +="}";
            return result;
        }
    }
}
