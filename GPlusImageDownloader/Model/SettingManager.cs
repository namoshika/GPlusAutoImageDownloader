using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GPlusImageDownloader.Model
{
    class SettingManager
    {
        public SettingManager()
        {
            //設定ファイル読み込み
            EmailAddress = GPlusImageDownloader.Properties.Settings.Default.EmailAddress;
            Password = GPlusImageDownloader.Properties.Settings.Default.Password;
            ImageSaveDirectory = new System.IO.DirectoryInfo(
                string.IsNullOrEmpty(GPlusImageDownloader.Properties.Settings.Default.ImageSaveDirectory)
                ? Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\testFolder"
                : GPlusImageDownloader.Properties.Settings.Default.ImageSaveDirectory);
            Cookies = DeserializeCookie();
            ImageHashList = DeserializeHashes();

            if (!ImageSaveDirectory.Exists)
                try
                {
                    ImageSaveDirectory.Create();
                    ImageSaveDirectory.Refresh();
                }
                catch (System.IO.IOException)
                { IsErrorNotFoundImageSaveDirectory = !ImageSaveDirectory.Exists; }
            else
                IsErrorNotFoundImageSaveDirectory = false;
        }
        ~SettingManager()
        {
            SerializeCookie(Cookies);
            SerializeHashes(ImageHashList);
        }

        public string EmailAddress { get; private set; }
        public string Password { get; private set; }
        public bool IsErrorNotFoundImageSaveDirectory { get; private set; }
        public System.IO.DirectoryInfo ImageSaveDirectory { get; private set; }
        public HashSet<string> ImageHashList { get; private set; }
        public System.Net.CookieContainer Cookies { get; set; }

        public System.Threading.Tasks.Task<bool> Save(string mail, string password, System.IO.DirectoryInfo imgDir)
        {
            return System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    var args = new SavingSettingEventArgs(mail, password, imgDir);
                    OnSavingSetting(args);
                    if (args.Cancel)
                        return false;

                    GPlusImageDownloader.Properties.Settings.Default.EmailAddress = mail;
                    GPlusImageDownloader.Properties.Settings.Default.Password = password;
                    GPlusImageDownloader.Properties.Settings.Default.ImageSaveDirectory = imgDir.FullName;
                    GPlusImageDownloader.Properties.Settings.Default.Save();
                    EmailAddress = mail;
                    Password = password;
                    ImageSaveDirectory = imgDir;

                    if (!imgDir.Exists)
                        try
                        {
                            imgDir.Create();
                            IsErrorNotFoundImageSaveDirectory = false;
                        }
                        catch { IsErrorNotFoundImageSaveDirectory = true; }
                    else
                        IsErrorNotFoundImageSaveDirectory = false;

                    OnSavedSetting(new EventArgs());
                    SerializeCookie(Cookies);
                    return true;
                });
        }
        void SerializeCookie(System.Net.CookieContainer cookies)
        {
            var serializer = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            var cookiePath = new System.IO.FileInfo(string.Format("{0}\\{1}\\cookie",
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                System.Reflection.Assembly.GetEntryAssembly().GetName().Name));
            using (var strm = cookiePath.Open(System.IO.FileMode.Create, System.IO.FileAccess.Write))
                serializer.Serialize(strm, cookies);
        }
        void SerializeHashes(HashSet<string> imageHashes)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(HashSet<string>));
            var hashesPath = new System.IO.FileInfo(string.Format("{0}\\{1}\\imageHashes.xml",
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                System.Reflection.Assembly.GetEntryAssembly().GetName().Name));
            using (var strm = hashesPath.Open(System.IO.FileMode.Create, System.IO.FileAccess.Write))
                serializer.Serialize(strm, imageHashes);
        }
        System.Net.CookieContainer DeserializeCookie()
        {
            var serializer = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            var cookiePath = new System.IO.FileInfo(string.Format("{0}\\{1}\\cookie",
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                System.Reflection.Assembly.GetEntryAssembly().GetName().Name));

            if (cookiePath.Exists)
                using (var strm = cookiePath.OpenRead())
                    return (System.Net.CookieContainer)serializer.Deserialize(strm);
            else
                return new System.Net.CookieContainer();
        }
        HashSet<string> DeserializeHashes()
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(HashSet<string>));
            var hashesPath = new System.IO.FileInfo(string.Format("{0}\\{1}\\imageHashes.xml",
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                System.Reflection.Assembly.GetEntryAssembly().GetName().Name));

            if (hashesPath.Exists)
                using (var strm = hashesPath.OpenRead())
                    return (HashSet<string>)serializer.Deserialize(strm);
            else
                return new HashSet<string>();
        }

        public event SavingSettingEventHandler SavingSetting;
        protected virtual void OnSavingSetting(SavingSettingEventArgs e)
        {
            if (SavingSetting != null)
                SavingSetting(this, e);
        }
        public event EventHandler SavedSetting;
        protected virtual void OnSavedSetting(EventArgs e)
        {
            if (SavedSetting != null)
                SavedSetting(this, e);
        }
    }
    delegate void SavingSettingEventHandler(object sender, SavingSettingEventArgs e);
    class SavingSettingEventArgs : System.ComponentModel.CancelEventArgs
    {
        public SavingSettingEventArgs(string mail, string pass, System.IO.DirectoryInfo imgDir)
        {
            EmailAddress = mail;
            Password = pass;
            ImageSaveDirectory = imgDir;
        }
        public string EmailAddress { get; private set; }
        public string Password { get; private set; }
        public System.IO.DirectoryInfo ImageSaveDirectory { get; private set; }
    }
}
