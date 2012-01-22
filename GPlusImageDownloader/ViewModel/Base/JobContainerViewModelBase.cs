using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace GPlusImageDownloader.ViewModel
{
    using GPlusImageDownloader.Model;

    class JobContainerViewModelBase : ViewModelBase
    {
        public JobContainerViewModelBase()
        {
            JobActivityGroups = new ObservableCollection<NoticeItemBase>();
        }
        ObservableCollection<NoticeItemBase> _jobActivityGroups;
        public ObservableCollection<NoticeItemBase> JobActivityGroups
        {
            get { return _jobActivityGroups; }
            set
            {
                _jobActivityGroups = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("JobActivityGroups"));
            }
        }
    }
}
