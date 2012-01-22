using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SunokoLibrary.GooglePlus;

namespace GPlusImageDownloader.ViewModel
{
    class JobActivityGroupViewModelBase : NoticeItemBase
    {
        public JobActivityGroupViewModelBase()
        {
            DownloadImageJobs = new List<JobViewModelBase>();
        }
        public System.Windows.Media.Imaging.BitmapImage NoticeIcon { get; set; }
        public Uri ActivityUrl { get; set; }
        public List<JobViewModelBase> DownloadImageJobs { get; set; }
    }
}
