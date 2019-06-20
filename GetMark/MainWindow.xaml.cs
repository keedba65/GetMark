using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace GetMark
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowVm _mVm = new MainWindowVm();

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            DataContext = _mVm;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _mVm.ShowWaiting = true;
            using (new WaitCursor())
            {
                _mVm.RssFeed = await MarkPodcast.GetPodcast();
                await DownloadNewPodcasts(_mVm.RssFeed);
            }
            _mVm.ShowWaiting = false;

            MessageBox.Show("Downloaded Mark to Music folder");
        }

        private async Task DownloadNewPodcasts(MarkFeed feed)
        {
            if (feed.Items.Count == 0)
            {
                return;
            }

            var dtLast = Settings.LastDownloadTimestamp;
            var feedList = new List<FeedItem>();
            foreach (var item in feed.Items)
            {
                if (item.PublishDate <= dtLast)
                {
                    break;
                }

                feedList.Add(item);
            }

            if (feedList.Count > 0)
            {
                feedList.Sort((f1, f2) =>
                {
                    var f1d = DateTime.Parse(f1.PublishDate.ToShortDateString());
                    var f2d = DateTime.Parse(f2.PublishDate.ToShortDateString());
                    if (f1d == f2d)
                    {
                        return 0;
                    }

                    if (f1d < f2d)
                    {
                        return -1;
                    }

                    return 1;
                });
                feedList.Reverse();
                var feedStack = new Stack<FeedItem>();
                foreach (var item in feedList)
                {
                    feedStack.Push(item);
                }

                //var sb = new StringBuilder();
                var downloadFailed = false;
                while (feedStack.Count > 0 && !downloadFailed)
                {
                    var item = feedStack.Pop();
                    //sb.AppendLine($"Got {item.Title}");
                    var retries = 3;
                    do
                    {
                        --retries;
                        //AppendToUpdateStatusTextBox(
                        //    $"Downloading {System.IO.Path.GetFileName(System.Net.WebUtility.UrlDecode(item.Enclosure.Url))}");
                        //var targetPath = $"Rush Limbaugh - {item.Title}{Path.GetExtension(item.Enclosure.Filename)}".Replace(",", "");
                        //AppendToUpdateStatusTextBox($"Downloading {targetPath}");
                        var success = await MarkPodcast.DownloadItem(item);
                        if (success)
                        {
                            MarkPodcast.UpdateLastDownloadTimestamp(item);
                            break;
                        }

                        if (retries > 0)
                        {
                            //AppendToUpdateStatusTextBox($"Download failed.  retrying in 60 seconds...");
                            await Task.Delay(60 * 1000);
                        }
                        else
                        {
                            downloadFailed = true;
                        }
                    } while (retries > 0);
                }
            }
        }
    }

    class MainWindowDesignVm : MainWindowVm
    {
        public MainWindowDesignVm()
        {
            RssFeed = new MarkFeed()
            {
                Items = new List<FeedItem>(),
                Title = "Mark Levin Podcast",
                Description = "Mark Levin is one of the hottest properties in Talk radio today. He is also one of the leading authors in the conservative political arena. Mark's radio show on WABC in New York City skyrocketed to Number 1 on the AM dial in his first 18 months on the air in the competitive 6:00 PM - 8:00 PM time slot. Mark's latest book, Plunder and Deceit, debuted at number one on the New York Times Best-Seller list. When your books are endorsed by Rush Limbaugh and Sean Hannity, you know you have a winner on your hands. In a short period of time, Mark has become one of the most listened to local radio Talk show hosts in the nation.",
                ImageUri = new Uri("https://www.omnycontent.com/d/playlist/a7b0bd27-d748-4fbe-ab3b-a6fa0049bcf6/392196e7-87cf-4af5-b31b-a89c01057741/2bab9367-8229-4d22-ad4c-a89c01057758/image.jpg?t=1520474383&amp;size=Large")
            };
            RssFeed.Items.Add(new FeedItem() { Title = "Mark Levin Audio Rewind - 3/4/19", PublishDate = DateTimeOffset.Parse("Tue, 05 Mar 2019 02:30:08 +0000").UtcDateTime, Link = new Uri("https://dts.podtrac.com/redirect.mp3/omnystudio.com/d/clips/a7b0bd27-d748-4fbe-ab3b-a6fa0049bcf6/392196e7-87cf-4af5-b31b-a89c01057741/f540e479-2303-448a-94f3-aa07002859d5/audio.mp3?utm_source=Podcast&amp;in_playlist=2bab9367-8229-4d22-ad4c-a89c01057758&amp;t=1551753066") });
            RssFeed.Items.Add(new FeedItem() { Title = "Mark Levin Audio Rewind - 3/1/19", PublishDate = DateTimeOffset.Parse("Sat, 02 Mar 2019 02:26:00 +0000").UtcDateTime, Link = new Uri("https://dts.podtrac.com/redirect.mp3/omnystudio.com/d/clips/a7b0bd27-d748-4fbe-ab3b-a6fa0049bcf6/392196e7-87cf-4af5-b31b-a89c01057741/ed63f9d3-9182-478c-84de-aa0400274135/audio.mp3?utm_source=Podcast&amp;in_playlist=2bab9367-8229-4d22-ad4c-a89c01057758&amp;t=1551493611") });
        }
    }
}
