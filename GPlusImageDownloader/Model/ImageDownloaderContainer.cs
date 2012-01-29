using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reactive;
using System.Reactive.Linq;

namespace GPlusImageDownloader.Model
{
    using SunokoLibrary.GooglePlus;

    class ImageDownloaderContainer : IDisposable
    {
        public ImageDownloaderContainer(SettingManager setting)
        {
            DownloadJobs = new List<ImageDownloader>();
            Setting = setting;
        }
        ~ImageDownloaderContainer() { Dispose(); }
        PlatformClient _client;
        IDisposable _stream;

        public bool IsWatchingStream { get; private set; }
        public List<ImageDownloader> DownloadJobs { get; private set; }
        public SettingManager Setting { get; private set; }

        public void StartDownload(System.Net.CookieContainer cookies)
        {
            IsWatchingStream = true;
            _client = new PlatformClient(cookies);
            if (_stream != null)
                _stream.Dispose();

            _stream = _client.GetStream()
                .OfType<CommentInfo>()
                //.Where(cmmInf => cmmInf.Html.Contains("ふぅ") || cmmInf.Owner.Name.Result.Contains("lome"))
                .Distinct(commentInfo => commentInfo.ParentActivity.Id)
                .Select(commentInfo => commentInfo.ParentActivity.UpdateActivityAsync(false).Result)
                .Where(activityInfo => activityInfo.PostStatus != PostStatusType.Removed && activityInfo.AttachedContentType == ContentType.Image)
                .Select(activityInfo => new
                {
                    ActivityInfo = activityInfo,
                    ImageInfos = ((AttachedAlbum)activityInfo.AttachedContent).Pictures
                })
                .Subscribe(item =>
                    {
                        var imgs = item.ImageInfos.Select(inf => new ImageDownloader(this, inf)).ToArray();
                        DownloadJobs.AddRange(imgs);
                        OnAddedDownloadingImage(new DownloadingImageEventArgs(item.ActivityInfo, imgs));
                        foreach (var job in imgs)
                            job.Download();
                    },
                    exp => OnRaiseError(new RaiseErrorEventArgs(exp)));
        }
        public void Dispose()
        {
            System.GC.SuppressFinalize(this);
            if (_stream != null)
                _stream.Dispose();
        }

        public event DownloadingImageEventHandler AddedDownloadingImage;
        protected virtual void OnAddedDownloadingImage(DownloadingImageEventArgs e)
        {
            if (AddedDownloadingImage != null)
                AddedDownloadingImage(this, e);
        }
        public event RaiseErrorEventHandler RaiseError;
        protected virtual void OnRaiseError(RaiseErrorEventArgs e)
        {
            IsWatchingStream = false;
            if (RaiseError != null)
                RaiseError(this, e);
        }

        public static bool CheckCanAuth(string mail, string pass, out System.Net.CookieContainer cookies)
        {
            cookies = new System.Net.CookieContainer();
            return SunokoLibrary.GooglePlus.Primitive.ApiWrapper
                .ConnectToServiceLoginAuth(mail, pass, cookies);
        }
        public static bool CheckCanAuth(System.Net.CookieContainer cookies)
        {
            return cookies.GetCookies(new Uri("https://plus.google.com"))["SSID"] != null;
        }
    }

    delegate void DownloadingImageEventHandler(object sender, DownloadingImageEventArgs e);
    class DownloadingImageEventArgs : EventArgs
    {
        public DownloadingImageEventArgs(ActivityInfo parentActivity, ImageDownloader[] downloader)
        {
            ParentActivity = parentActivity;
            Downloader = downloader;
        }
        public ActivityInfo ParentActivity { get; private set; }
        public ImageDownloader[] Downloader { get; private set; }
    }
    delegate void RaiseErrorEventHandler(object sender, RaiseErrorEventArgs e);
    class RaiseErrorEventArgs : EventArgs
    {
        public RaiseErrorEventArgs(Exception e) { ThrowedException = e; }
        public Exception ThrowedException { get; private set; }
    }
}
