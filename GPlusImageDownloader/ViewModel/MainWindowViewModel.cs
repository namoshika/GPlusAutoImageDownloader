using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPlusImageDownloader.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(Model.MainWindowModel model)
        {
            JobContainer = new JobContainerViewModel(model.Downloader);
            Setting = new SettingViewModel(model.Downloader, model.Setting);
        }

        JobContainerViewModelBase _jobContainer;
        SettingViewModel _setting;

        public JobContainerViewModelBase JobContainer
        {
            get { return _jobContainer; }
            set
            {
                _jobContainer = value;
                OnPropertyChanged(
                    new System.ComponentModel.PropertyChangedEventArgs("JobContainer"));
            }
        }
        public SettingViewModel Setting
        {
            get { return _setting; }
            set
            {
                _setting = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("Setting"));
            }
        }
    }
}
