using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace GPlusImageDownloader.ViewModel
{
    using GPlusImageDownloader.Model;

    class JobContainerViewModel : JobContainerViewModelBase
    {
        public JobContainerViewModel(ImageDownloaderContainer downloader)
        {
            ThumbDir = new System.IO.DirectoryInfo(string.Format("{0}\\{1}", System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName()));
            ThumbDir.Create();
            _downloader = downloader;
            _downloader.AddedDownloadingImage += _downloader_DownloadingImageEvent;
        }
        ImageDownloaderContainer _downloader;
        public System.IO.DirectoryInfo ThumbDir { get; protected set; }

        void _downloader_DownloadingImageEvent(object sender, DownloadingImageEventArgs e)
        {
            App.Current.Dispatcher.BeginInvoke(((Action)(() =>
                JobActivityGroups.Insert(0, new JobActivityGroupViewModel(this, e.ParentActivity, e.Downloader)))));
        }

        ~JobContainerViewModel()
        {
            if (ThumbDir.Exists)
                ThumbDir.Delete(true);
        }
    }
}
