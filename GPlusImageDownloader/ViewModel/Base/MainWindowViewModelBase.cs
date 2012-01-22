using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPlusImageDownloader.ViewModel
{
    class MainWindowViewModelBase : ViewModelBase
    {
        JobContainerViewModelBase _jobContainer;
        SettingViewModelBase _setting;

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
        public SettingViewModelBase Setting
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
