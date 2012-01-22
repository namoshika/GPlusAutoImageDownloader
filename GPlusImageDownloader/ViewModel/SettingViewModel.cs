using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace GPlusImageDownloader.ViewModel
{
    using GPlusImageDownloader.Model;

    class SettingViewModel : SettingViewModelBase
    {
        public SettingViewModel(ImageDownloaderContainer jobContainer, SettingManager setting)
        {
            EmailAddress = setting.EmailAddress;
            Password = setting.Password;
            ImageSaveDirectory = setting.ImageSaveDirectory.FullName;

            PropertyChanged += SettingViewModel_PropertyChanged;
            SaveConfigCommand = new RelayCommand(async obj =>
                {
                    IsError = !(await setting.Save(
                        EmailAddress, Password, new System.IO.DirectoryInfo(ImageSaveDirectory)));

                    if (IsError)
                        return; 
                    IsModified = false;
                    IsExpanded = false;
                });
            CancelConfigCommand = new RelayCommand(obj =>
                {
                    EmailAddress = setting.EmailAddress;
                    Password = setting.Password;
                    ImageSaveDirectory = setting.ImageSaveDirectory.FullName;
                    IsModified = false;
                    IsExpanded = false;
                });
        }
        void SettingViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsModified")
                IsModified = true;
        }
    }
}
