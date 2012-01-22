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
            CancelConfigCommand = new RelayCommand(obj => { IsError = !IsError; });
        }

        bool _isError;
        bool _isExpanded;
        bool _isModified;
        string _emailAddress;
        string _password;
        string _imageSaveDirectory;

        public bool IsError
        {
            get { return _isError; }
            set
            {
                _isError = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("IsError"));
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
        public ICommand SaveConfigCommand { get; set; }
        public ICommand CancelConfigCommand { get; set; }
    }
}
