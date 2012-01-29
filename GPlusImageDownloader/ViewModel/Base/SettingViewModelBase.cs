using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace GPlusImageDownloader.ViewModel
{
    class SettingViewModelBase : ViewModelBase
    {
        public SettingViewModelBase()
        {
            SaveConfigCommand = new RelayCommand(obj => { IsExpanded = false; });
            CancelConfigCommand = new RelayCommand(obj => {  });
            NotificationText = "エラーが発生しました。メールアドレスやパスワード、画像保存先に異常がある事が考えられます。";
        }

        bool _isExpanded;
        bool _isModified;
        string _emailAddress;
        string _password;
        string _imageSaveDirectory;
        string _notificationText;
        SettingStatusType _status;

        public SettingStatusType Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Status"));
            }
        }
        public bool IsModified
        {
            get { return _isModified; }
            set
            {
                _isModified = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsModified"));
            }
        }
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (_isExpanded == value)
                    return;
                _isExpanded = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsExpanded"));
            }
        }
        public string EmailAddress
        {
            get { return _emailAddress; }
            set
            {
                _emailAddress = value;
                OnPropertyChanged(
                    new System.ComponentModel.PropertyChangedEventArgs("EmailAddress"));
            }
        }
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged(
                    new System.ComponentModel.PropertyChangedEventArgs("Password"));
            }
        }
        public string ImageSaveDirectory
        {
            get { return _imageSaveDirectory; }
            set
            {
                _imageSaveDirectory = value;
                OnPropertyChanged(
                    new System.ComponentModel.PropertyChangedEventArgs("ImageSaveDirectory"));
            }
        }
        public string NotificationText
        {
            get { return _notificationText; }
            set
            {
                _notificationText = value;
                OnPropertyChanged(
                    new System.ComponentModel.PropertyChangedEventArgs("NotificationText"));
            }
        }
        public ICommand SaveConfigCommand { get; set; }
        public ICommand CancelConfigCommand { get; set; }
    }
    enum SettingStatusType { Normal, Checking, Error }

    class SettingStatusToBooleanConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var status = (SettingStatusType)value;
            return status != SettingStatusType.Checking;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
