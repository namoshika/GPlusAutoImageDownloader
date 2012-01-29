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
            NotificationText = string.Empty;

            PropertyChanged += SettingViewModel_PropertyChanged;
            SaveConfigCommand = new RelayCommand(
                async obj =>
                    {
                        NotificationText = string.Empty;
                        Status = SettingStatusType.Checking;
                        Status = (await setting.Save(
                            EmailAddress, Password, new System.IO.DirectoryInfo(ImageSaveDirectory)))
                            ? SettingStatusType.Normal : SettingStatusType.Error;

                        if (Status == SettingStatusType.Error)
                        {
                            NotificationText = "エラーが発生しました。メールアドレスやパスワード、画像保存先に異常がある事が考えられます。";
                            return;
                        }
                        IsModified = false;
                        IsExpanded = false;
                    });
            CancelConfigCommand = new RelayCommand(
                obj =>
                    {
                        EmailAddress = setting.EmailAddress;
                        Password = setting.Password;
                        ImageSaveDirectory = setting.ImageSaveDirectory.FullName;
                        NotificationText = string.Empty;
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
