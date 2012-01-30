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

            //EmailAddress, Passwordが空文字だった場合には設定保存をキャンセルする。
            if (string.IsNullOrEmpty(e.EmailAddress) || string.IsNullOrEmpty(e.Password))
            {
                e.Cancel = true;
                _cookies = null;
                return;
            }
            //EmailAddress, Passwordに変化がある場合、あるいはストリーム監視中でなかった場合
            //には再ログイン処理を行う。
            if (Setting.EmailAddress != e.EmailAddress || Setting.Password != e.Password
                || !Downloader.IsWatchingStream)
            {
                //変更されたログイン情報を元に再ログインする。ログインに失敗した場合には
                //e.Cancel = trueにする。成功した場合には_cookiesにログイン済みクッキーを入れ、
                //SavedSettingで設定変更が確定した時にSetting.Cookiesを更新する。
                e.Cancel = !ImageDownloaderContainer.CheckCanAuth(e.EmailAddress, e.Password, out _cookies);
                if (e.Cancel)
                    _cookies = null;
            }
            //何も変化が無く、ストリーム監視中だった場合には設定保存を許可しつつ、
            //後のSetting_SavedSettingでSetting.Cookiesを更新して再接続しないようにする。
            else
                _cookies = null;
        }
        void Setting_SavedSetting(object sender, EventArgs e)
        {
            if (_cookies == null)
                return;

            Setting.Cookies = _cookies;
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
