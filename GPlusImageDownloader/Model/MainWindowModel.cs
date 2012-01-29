using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPlusImageDownloader.Model
{
    class MainWindowModel
    {
        public MainWindowModel()
        {
            Setting = new SettingManager();
            Setting.SavingSetting += Setting_SavingSetting;
            Setting.SavedSetting += Setting_SavedSetting;
            Downloader = new ImageDownloaderContainer(Setting);
        }
        public MainWindowModel(SettingManager setting, ImageDownloaderContainer downloader)
        {
            Setting = setting;
            Setting.SavingSetting += Setting_SavingSetting;
            Setting.SavedSetting += Setting_SavedSetting;
            Downloader = downloader;
        }
        ~MainWindowModel() { Dispose(); }
        System.Net.CookieContainer _cookies;

        public ImageDownloaderContainer Downloader { get; private set; }
        public SettingManager Setting { get; private set; }

        public void StartDownload()
        {
            if (ImageDownloaderContainer.CheckCanAuth(Setting.Cookies))
            {
                Downloader.StartDownload(Setting.Cookies);
                OnNotify(new NotifyEventArgs("ログイン成功。ストリーム監視を開始。"));
            }
            else
                OnNotify(new NotifyEventArgs("ログイン失敗。"));
        }
        public void Dispose()
        {
            System.GC.SuppressFinalize(this);
            if (Downloader != null)
                Downloader.Dispose();
        }

        void Setting_SavingSetting(object sender, SavingSettingEventArgs e)
        {
            if (Setting.EmailAddress == e.EmailAddress
                && Setting.Password == e.Password)
            {
                e.Cancel = true;
                return;
            }

            e.Cancel = !ImageDownloaderContainer.CheckCanAuth(e.EmailAddress, e.Password, out _cookies);
            if (e.Cancel)
                _cookies = null;
        }
        void Setting_SavedSetting(object sender, EventArgs e)
        {
            Setting.Cookies = _cookies ?? Setting.Cookies;
            StartDownload();
        }

        public event NotifyEventHandler Notify;
        protected virtual void OnNotify(NotifyEventArgs e)
        {
            if (Notify != null)
                Notify(this, e);
        }
    }
    delegate void NotifyEventHandler(object sender, NotifyEventArgs e);
    class NotifyEventArgs : EventArgs
    {
        public NotifyEventArgs(string text) { Text = text; }
        public string Text { get; private set; }
    }
}
