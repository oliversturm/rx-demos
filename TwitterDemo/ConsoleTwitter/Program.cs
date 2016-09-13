using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Events;
using Tweetinvi.Streaming;

namespace ConsoleTwitter
{
    class Program
    {
        static void Main(string[] args)
        {
            Doit();
            Console.ReadLine();
        }

        static async void Doit() {
            //Auth.SetUserCredentials("CONSUMER_KEY", "CONSUMER_SECRET", "ACCESS_TOKEN", "ACCESS_TOKEN_SECRET");

            //            var stream = Stream.CreateUserStream();

            var stream = Stream.CreateFilteredStream();
            stream.AddTrack("@olivers");

            var tweetsReceived = Observable.FromEventPattern<MatchedTweetReceivedEventArgs>(
                handler => stream.MatchingTweetReceived += handler,
                handler => stream.MatchingTweetReceived -= handler);
            //handler => stream.TweetCreatedByFriend += handler,
            //    handler => stream.TweetCreatedByFriend -= handler);

            tweetsReceived.Subscribe(tr =>
            {
                Console.WriteLine(tr.EventArgs.Tweet.Text);
            });

            await stream.StartStreamMatchingAllConditionsAsync();
            //await stream.StartStreamAsync();
        }
    }
}
