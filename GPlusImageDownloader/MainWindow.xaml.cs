using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GPlusImageDownloader
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

#if ENABLED_VMTEST_MODE
            DataContext = FindResource("testVm");
#else
            _downloader = new Model.ImageDownloaderContainer();
            var downloaderVm = new ViewModel.JobContainerViewModel(_downloader);
            DataContext = downloaderVm;
            _downloader.StartDownload();
#endif
            Closed += new EventHandler(MainWindow_Closed);
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            if (_downloader != null)
                _downloader.Dispose();
        }
        Model.ImageDownloaderContainer _downloader;
    }
}
