using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SunokoLibrary.GooglePlus;

namespace GPlusImageDownloader.ViewModel
{
    class JobActivityGroupViewModel : JobActivityGroupViewModelBase
    {
        public JobActivityGroupViewModel(JobContainerViewModel container, ActivityInfo activityInfo, Model.ImageDownloader[] imageDownloader)
        {
            NoticeText = activityInfo.PostUser.Name;
            NoticeIcon = new System.Windows.Media.Imaging.BitmapImage();
            NoticeIcon.BeginInit();
            NoticeIcon.DecodePixelWidth = 25;
            NoticeIcon.UriSource = activityInfo.PostUser.IconImageUrl;
            NoticeIcon.EndInit();
            ActivityUrl = activityInfo.PostUrl;
            DownloadImageJobs = imageDownloader.Select(
                downloader => (JobViewModelBase)new JobViewModel(container, downloader)).ToList();
        }
    }
}
