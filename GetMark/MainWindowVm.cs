using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel;

namespace GetMark
{
    class MainWindowVm : ObservableObject
    {
        private MarkFeed _mRssFeed;
        private bool _mShowWaiting;
        public MarkFeed RssFeed { get => _mRssFeed;
            set { _mRssFeed = value; OnPropertyChanged(); }
        }
        public bool ShowWaiting { get => _mShowWaiting; set { _mShowWaiting = value; OnPropertyChanged(); OnPropertiesChanged("IsFeedListEnabled");} }
        public bool IsFeedListEnabled => !ShowWaiting;
    }
}
