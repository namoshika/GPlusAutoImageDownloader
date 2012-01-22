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
        public ImageDownloaderContainer()
        {
            DownloadJobs = new List<ImageDownloader>();
            ImageDirectory = new System.IO.DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\testFolder");
            ImageHashsFile = new System.IO.FileInfo(ImageDirectory.FullName + "\\imgHashs.xml");
            _formatter = new System.Xml.Serialization.XmlSerializer(typeof(HashSet<string>));
        }
        PlatformClient _client;
        IDisposable _stream;
        System.Xml.Serialization.XmlSerializer _formatter;
        public System.IO.FileInfo ImageHashsFile { get; private set; }
        public System.IO.DirectoryInfo ImageDirectory { get; private set; }
        public HashSet<string> ImageHashs { get; private set; }
        public List<ImageDownloader> DownloadJobs { get; private set; }

        public async void StartDownload()
        {
            await Initialize();

            _stream = _client.GetStream()
                .OfType<CommentInfo>()
                //.Where(cmmInf => cmmInf.Html.Contains("ふぅ") || cmmInf.Owner.Name.Result.Contains("lome"))
                .Distinct(commentInfo => commentInfo.ParentActivity.Id)
                .Select(commentInfo => commentInfo.ParentActivity.UpdateActivityAsync(false).Result)
                .Where(activityInfo => activityInfo.PostStatus != PostStatusType.Removed && activityInfo.AttachedContentType == ContentType.Image)
                .Select(activityInfo => new {
                    ActivityInfo = activityInfo,
                    ImageInfos = ((AttachedAlbum)activityInfo.AttachedContent).Pictures })
                .Subscribe(item =>
                    {
                        var imgs = item.ImageInfos.Select(inf => new ImageDownloader(this, inf)).ToArray();
                        DownloadJobs.AddRange(imgs);
                        OnAddedDownloadingImage(new DownloadingImageEventArgs(item.ActivityInfo, imgs));
                        foreach (var job in imgs)
                            job.Download();
                    },
                    exp => Console.WriteLine("Excp: {0}", exp.Message));
        }
        async System.Threading.Tasks.Task Initialize()
        {
            var vals = System.Xml.Linq.XDocument.Load("IGNORE_DATAS.xml").Root;
            var mail = vals.Element("mail").Value;
            var pass = vals.Element("password").Value;
            _client = await PlatformClient.Login(mail, pass);

            if (ImageHashsFile.Exists)
                using (var reader = ImageHashsFile.OpenRead())
                    ImageHashs = (HashSet<string>)_formatter.Deserialize(reader);
            else
                ImageHashs = new HashSet<string>();
        }

        public event DownloadingImageEventHandler AddedDownloadingImage;
        protected virtual void OnAddedDownloadingImage(DownloadingImageEventArgs e)
        {
            if (AddedDownloadingImage != null)
                AddedDownloadingImage(this, e);
        }

        public void Dispose()
        {
            System.GC.SuppressFinalize(this);
            if (_stream != null)
                _stream.Dispose();
            using (var writter = ImageHashsFile.OpenWrite())
                _formatter.Serialize(writter, ImageHashs);
        }
        ~ImageDownloaderContainer()
        { Dispose(); }
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
}
