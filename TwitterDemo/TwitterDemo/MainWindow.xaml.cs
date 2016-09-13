using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tweetinvi;
using Tweetinvi.Events;

namespace TwitterDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Auth.SetUserCredentials("CONSUMER_KEY", "CONSUMER_SECRET", "ACCESS_TOKEN", "ACCESS_TOKEN_SECRET");

            // This filters for tweets mentioning @olivers

            //var stream = Stream.CreateFilteredStream();
            //stream.AddTrack("@olivers");

            //var tweetsReceived = Observable.FromEventPattern<MatchedTweetReceivedEventArgs>(
            //    handler => stream.MatchingTweetReceived += handler,
            //    handler => stream.MatchingTweetReceived -= handler);

            // This goes for the complete user stream
            var stream = Stream.CreateUserStream();

            var tweetsReceived = Observable.FromEventPattern<TweetReceivedEventArgs>(
                handler => stream.TweetCreatedByFriend += handler,
                handler => stream.TweetCreatedByFriend -= handler);

            tweetsReceived.ObserveOnDispatcher().Subscribe(tr =>
            {
                list.Items.Add(tr.EventArgs.Tweet);
            });

            // Filtered stream
            //            await stream.StartStreamMatchingAllConditionsAsync();
               
            // Complete user stream
            await stream.StartStreamAsync();
        }
    }
}
