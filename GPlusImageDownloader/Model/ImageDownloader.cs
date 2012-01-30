using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SunokoLibrary.GooglePlus;

namespace GPlusImageDownloader.Model
{
    class ImageDownloader
    {
        public ImageDownloader(ImageDownloaderContainer container, ImageInfo imageInfo)
        {
            _container = container;
            ImageInfo = imageInfo;
            Status = DownloadStatus.Unloaded;
        }
        static ImageDownloader()
        {
            _hashMaker = new System.Security.Cryptography.SHA256CryptoServiceProvider();
        }
        ImageDownloaderContainer _container;
        static System.Security.Cryptography.SHA256CryptoServiceProvider _hashMaker;

        public string HashText { get; private set; }
        public DownloadStatus Status { get; private set; }
        public ImageInfo ImageInfo { get; private set; }
        public System.IO.FileInfo DownloadedImageFile { get; private set; }
        public System.IO.FileInfo DownloadedTempImageFile { get; private set; }
        public System.Threading.Tasks.Task Download()
        {
            return System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    Status = DownloadStatus.Loading;
                    OnChangedTaskStatus(new EventArgs());
                    var tmpFile = new System.IO.FileInfo(System.IO.Path.GetTempFileName());
                    try
                    {
                        var hashBuilder = new StringBuilder();
                        using (var fileStrm = tmpFile.Open(System.IO.FileMode.Open))
                        using (var imgStrm = ImageInfo.GetStream())
                        {
                            var buff = new byte[1024];
                            var buffLen = 0;
                            while ((buffLen = imgStrm.Read(buff, 0, buff.Length)) > 0)
                                fileStrm.Write(buff, 0, buffLen);

                            fileStrm.Seek(0, System.IO.SeekOrigin.Begin);
                            foreach (var chr in _hashMaker.ComputeHash(fileStrm))
                                hashBuilder.AppendFormat("{0:X2}", chr);
                        }

                        HashText = hashBuilder.ToString();
                        DownloadedTempImageFile = tmpFile;
                        var imgFile = new System.IO.FileInfo(_container.Setting.ImageSaveDirectory.FullName + "\\" + HashText + ".jpg");
                        if (_container.Setting.ImageHashList.Add(HashText) && !imgFile.Exists)
                        {
                            imgFile = tmpFile.CopyTo(imgFile.FullName);
                            Status = DownloadStatus.Loaded;
                            DownloadedImageFile = imgFile;
                            OnChangedTaskStatus(new EventArgs());
                        }
                        else
                        {
                            Status = imgFile.Exists ? DownloadStatus.Loaded : DownloadStatus.Deleted;
                            DownloadedImageFile = imgFile.Exists ? imgFile : null;
                            OnChangedTaskStatus(new EventArgs());
                        }
                    }
                    catch (Exception)
                    {
                        Status = DownloadStatus.Failed;
                        OnChangedTaskStatus(new EventArgs());
                    }
                    finally
                    {
                        if (tmpFile.Exists)
                            tmpFile.Delete();
                    }
                });
        }
        public void RefreshStatus()
        {
            if (Status == DownloadStatus.Loaded)
            {
                DownloadedImageFile.Refresh();
                Status = DownloadedImageFile.Exists ? DownloadStatus.Loaded : DownloadStatus.Deleted;
            }
        }
        public void Open()
        {
            System.Diagnostics.Process.Start(DownloadedImageFile.FullName);
        }

        public event EventHandler ChangedTaskStatus;
        protected virtual void OnChangedTaskStatus(EventArgs e)
        {
            if (ChangedTaskStatus != null)
                ChangedTaskStatus(this, e);
        }
    }
    public enum DownloadStatus
    {
        Unloaded, Loading, Loaded, Failed, Deleted,
    }
}
