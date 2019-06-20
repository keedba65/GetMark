using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetMark
{
    class MarkFeed
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Uri ImageUri { get; set; }
        public List<FeedItem> Items { get; set; }
    }

    class FeedItem
    {
        public string Title { get; set; }
        public string Summary { get; set; }
        public DateTime PublishDate { get; set; }
        public Uri Link { get; set; }
    }
}
