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
            Downloader.RaiseError += Downloader_RaiseError;
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
            Downloader.Dispose();
        }

        void Setting_SavingSetting(object sender, SavingSettingEventArgs e)
        {
            //設定が変更された場合、その変更が有効なものであるかをチェックする
            //そして変更が無効である場合にはe.Cancel = trueにする事で設定変更
            //をキャンセルする。

            if (string.IsNullOrEmpty(e.EmailAddress) || string.IsNullOrEmpty(e.Password) ||
                Setting.EmailAddress == e.EmailAddress && Setting.Password == e.Password
                && Downloader.IsWatchingStream)
            {
                e.Cancel = true;
                return;
            }
            //変更されたログイン情報を元に再ログインする。ログインに失敗した場合には
            //e.Cancel = trueにする。成功した場合には_cookiesにログイン済みクッキーを入れ、
            //SavedSettingで設定変更が確定した時にSetting.Cookiesを更新する。
            e.Cancel = !ImageDownloaderContainer.CheckCanAuth(e.EmailAddress, e.Password, out _cookies);
            if (e.Cancel)
                _cookies = null;
        }
        void Setting_SavedSetting(object sender, EventArgs e)
        {
            Setting.Cookies = _cookies ?? Setting.Cookies;
            StartDownload();
        }
        void Downloader_RaiseError(object sender, RaiseErrorEventArgs e)
        {
            OnNotify(new NotifyEventArgs(
                string.Format("エラーが発生しました。\r\n{0}", e.ThrowedException.Message)));
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
