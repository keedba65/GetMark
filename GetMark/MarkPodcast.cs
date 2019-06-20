using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GetMark
{
    class MarkPodcast
    {
        public static async Task<MarkFeed> GetPodcast()
        {
            var markFeed = new MarkFeed {Items = new List<FeedItem>()};
            var xml = await GetPodcastXml();
            using (var reader = new StringReader(xml))
            {
                using (XmlReader xmlReader = new XmlTextReader(reader))
                {
                    var feed = SyndicationFeed.Load(xmlReader);
                    markFeed.Title = feed.Title.Text;
                    markFeed.Description = feed.Description.Text;
                    markFeed.ImageUri = feed.ImageUrl;
                    foreach (var item in feed.Items)
                    {
                        var feedItem = new FeedItem();
                        feedItem.Title = item.Title.Text;
                        feedItem.Summary = item.Summary.Text;
                        feedItem.PublishDate = item.PublishDate.UtcDateTime;
                        feedItem.Link = item.Links[0].Uri;
                        markFeed.Items.Add(feedItem);
                    }
                }
            }

            return markFeed;
        }

        public static async Task<bool> DownloadItem(FeedItem item)
        {
            var targetDir = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            var targetPath = Path.Combine(targetDir, $"{item.Title}.mp3").Replace("/", "_");
            var uri = new Uri($"{item.Link.OriginalString}&download=true");
            //_mLogger.Info($"Downloading from {item.Enclosure.Url} to {targetPath}");
            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        using (var reader = await response.Content.ReadAsStreamAsync())
                        {
                            using (var writer = File.Open(targetPath, FileMode.Create))
                            {
                                var buffer = new byte[64 * 1024];
                                var read = 0;
                                do
                                {
                                    read = await reader.ReadAsync(buffer, 0, buffer.Length);
                                    if (read > 0)
                                    {
                                        await writer.WriteAsync(buffer, 0, read);
                                    }
                                } while (read > 0);
                            }
                        }
                        //UpdateLastDownloadTimestamp(item);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    //_mLogger.Error(ex);
                }
                return false;
            }
        }

        public static void UpdateLastDownloadTimestamp(FeedItem item)
        {
            if (item.PublishDate > Settings.LastDownloadTimestamp)
            {
                Settings.LastDownloadTimestamp = item.PublishDate;
            }
        }

        private static async Task<string> GetPodcastXml()
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync("https://omny.fm/shows/mark-levin-audio-rewind/playlists/podcast.rss");
                var content = response.Content;

                // ... Read the string.
                string result = await content.ReadAsStringAsync();

                //// ... Display the result.
                //if (result != null && result.Length >= 50)
                //{
                //    Debug.WriteLine(result.Substring(0, 50) + "...");
                //}
                return result;
            }
        }

    }

}
