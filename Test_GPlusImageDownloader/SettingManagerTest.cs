using GPlusImageDownloader.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Test_GPlusImageDownloader
{
    
    
    /// <summary>
    ///SettingManagerTest のテスト クラスです。すべての
    ///SettingManagerTest 単体テストをここに含めます
    ///</summary>
    [TestClass()]
    public class SettingManagerTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///現在のテストの実行についての情報および機能を
        ///提供するテスト コンテキストを取得または設定します。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 追加のテスト属性
        // 
        //テストを作成するときに、次の追加属性を使用することができます:
        //
        //クラスの最初のテストを実行する前にコードを実行するには、ClassInitialize を使用
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //クラスのすべてのテストを実行した後にコードを実行するには、ClassCleanup を使用
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //各テストを実行する前にコードを実行するには、TestInitialize を使用
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //各テストを実行した後にコードを実行するには、TestCleanup を使用
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///Save のテスト
        ///</summary>
        [TestMethod()]
        public void SaveTest()
        {
            GPlusImageDownloader.Properties.Settings.Default.Reset();

            SettingManager target = new SettingManager();
            var savingFlg = false;
            target.SavingSetting += (sender, e) => { savingFlg = true; };
            var savedFlg = false;
            target.SavedSetting += (sender, e) => { savedFlg = true; };
            var actualA = target.Save("hoge", "pwd", new DirectoryInfo("dir")).Result;

            Assert.IsTrue(actualA);
            Assert.IsTrue(savedFlg);
            Assert.IsTrue(savingFlg);
            Assert.AreEqual(target.EmailAddress, "hoge");
            Assert.AreEqual(target.Password, "pwd");
            Assert.AreEqual(target.ImageSaveDirectory.Name, "dir");

            savedFlg = false;
            target = new SettingManager();
            target.SavingSetting += (sender, e) => { e.Cancel = true; };
            target.SavedSetting += (sender, e) => { savedFlg = true; };
            Assert.AreEqual(target.EmailAddress, "hoge");
            Assert.AreEqual(target.Password, "pwd");
            Assert.AreEqual(target.ImageSaveDirectory.Name, "dir");
            var actualB = target.Save("hoge", "pwd", new DirectoryInfo("dir"));
            Assert.IsFalse(savedFlg);
        }
    }
}
