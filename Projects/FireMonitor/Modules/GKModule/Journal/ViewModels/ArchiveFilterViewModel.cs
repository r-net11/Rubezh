using System;
using Common.GK;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
    public class ArchiveFilterViewModel : DialogViewModel
    {
        public ArchiveFilterViewModel(XArchiveFilter archiveFilter)
        {
            Title = "Настройки фильтра";
            ClearCommand = new RelayCommand(OnClear);
            SaveCommand = new RelayCommand(OnSave);
            CancelCommand = new RelayCommand(OnCancel);

            StartDate = archiveFilter.StartDate;
            EndDate = archiveFilter.EndDate;
            StartTime = archiveFilter.StartDate;
            EndTime = archiveFilter.EndDate;
            UseSystemDate = archiveFilter.UseSystemDate;
        }

        public DateTime ArchiveFirstDate
        {
            get { return ArchiveViewModel.ArchiveFirstDate; }
        }

        public DateTime NowDate
        {
            get { return DateTime.Now; }
        }

        bool _useSystemDate;
        public bool UseSystemDate
        {
            get { return _useSystemDate; }
            set
            {
                _useSystemDate = value;
                OnPropertyChanged("UseSystemDate");
            }
        }

        DateTime _startDate;
        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                OnPropertyChanged("StartDate");
            }
        }
        public DateTime StartTime
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                OnPropertyChanged("StartTime");
            }
        }

        DateTime _endDate;
        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                OnPropertyChanged("EndDate");
            }
        }
        public DateTime EndTime
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                OnPropertyChanged("EndTime");
            }
        }

        public XArchiveFilter GetModel()
        {
            return new XArchiveFilter()
            {
                StartDate = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, StartTime.Hour, StartTime.Minute, StartTime.Second),
                EndDate = new DateTime(EndDate.Year, EndDate.Month, EndDate.Day, EndTime.Hour, EndTime.Minute, EndTime.Second),
                UseSystemDate = UseSystemDate,
            };
        }

        public RelayCommand ClearCommand { get; private set; }
        void OnClear()
        {
            StartDate = StartTime = EndDate = EndTime = DateTime.Now;
            StartDate = StartDate.AddDays(-1);
            UseSystemDate = false;
        }
        public RelayCommand SaveCommand { get; private set; }
        void OnSave()
        {
            if (StartDate > EndDate)
            {
                MessageBoxService.ShowWarning("Начальная дата должна быть меньше конечной");
                return;
            }
            Close(true);
        }
        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }
    }
}