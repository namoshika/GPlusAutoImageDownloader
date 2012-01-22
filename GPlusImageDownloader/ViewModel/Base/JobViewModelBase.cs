using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace GPlusImageDownloader.ViewModel
{
    using Model;

    class JobViewModelBase : ViewModelBase
    {
        public JobViewModelBase()
        {
            OpenImageCommand = new RelayCommand(
                obj => System.Windows.MessageBox.Show("called OpenImageCommand"));
        }

        DownloadStatus _imageStatus;
        Uri _imageUrl;
        Uri _image;

        public Uri ImageUrl
        {
            get { return _imageUrl; }
            set
            {
                _imageUrl = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("ImageUrl"));
            }
        }
        public Uri ImageThumbPath
        {
            get { return _image; }
            set
            {
                _image = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("ImageThumbPath"));
            }
        }
        public DownloadStatus DownloadStatus
        {
            get { return _imageStatus; }
            set
            {
                _imageStatus = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("DownloadStatus"));
            }
        }
        public System.Windows.Input.ICommand OpenImageCommand { get; protected set; }
    }
}
