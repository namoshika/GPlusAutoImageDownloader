using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace GPlusImageDownloader.ViewModel
{
    using Model;

    class JobViewModel : JobViewModelBase
    {
        public JobViewModel(JobContainerViewModel container, ImageDownloader model)
        {
            _model = model;
            _model.ChangedTaskStatus += _model_ChangedTaskStatus;
            _container = container;
            DownloadStatus = model.Status;
            ImageUrl = _model.ImageInfo.ImageUrl;
            OpenImageCommand = new RelayCommand(
                obj => _model.Open(),
                obj =>
                    {
                        _model.DownloadedImageFile.Refresh();
                        return _model.DownloadedImageFile != null && model.DownloadedImageFile.Exists;
                    });

            _model_ChangedTaskStatus(null, null);
        }
        ImageDownloader _model;
        JobContainerViewModel _container;

        void _model_ChangedTaskStatus(object sender, EventArgs e)
        {
            switch (_model.Status)
            {
                case DownloadStatus.Loading:
                    ImageUrl = _model.ImageInfo.ImageUrl;
                    break;
                case DownloadStatus.Loaded:
                case DownloadStatus.Deleted:
                    var file = new System.IO.FileInfo(string.Format("{0}\\{1}.jpg", _container.ThumbDir.FullName, _model.HashText));
                    if (!file.Exists)
                    {
                        using (var reader = _model.DownloadedTempImageFile.OpenRead())
                        {
                            var dec = System.Windows.Media.Imaging.BitmapDecoder.Create(
                                reader, System.Windows.Media.Imaging.BitmapCreateOptions.None,
                                System.Windows.Media.Imaging.BitmapCacheOption.None);
                            var baseImg = dec.Frames.First();

                            var width = 250;
                            var height = 80;
                            var rate = Math.Max((double)width / baseImg.PixelWidth, (double)height / baseImg.PixelHeight);
                            var transform = new ScaleTransform(rate, rate);
                            var resizeImg = new System.Windows.Media.Imaging.TransformedBitmap(baseImg, transform);
                            var trimmingImg = new System.Windows.Media.Imaging.CroppedBitmap(
                                resizeImg, new System.Windows.Int32Rect(0, 0, width, height));
                            var enc = new System.Windows.Media.Imaging.JpegBitmapEncoder();
                            enc.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(trimmingImg));
                            using (var writer = file.Create())
                                enc.Save(writer);
                        }
                    }
                    ImageThumbPath = new Uri(file.FullName);
                    break;
            }
            DownloadStatus = _model.Status;
        }
    }
}
