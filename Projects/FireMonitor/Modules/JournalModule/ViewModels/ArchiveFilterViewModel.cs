using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI.Models;
using Infrastructure.Common;

namespace JournalModule.ViewModels
{
    public class ArchiveFilterViewModel : DialogContent
    {
        public ArchiveFilterViewModel(ArchiveFilter archiveFilter)
        {
            Initialize();

            _startDate = archiveFilter.StartDate;
            _endDate = archiveFilter.EndDate;
            UseSystemDate = archiveFilter.UseSystemDate;

            if (archiveFilter.Descriptions.IsNotNullOrEmpty())
            {
                JournalEvents.Where(x => archiveFilter.Descriptions.Any(description => description == x.Name)).
                              AsParallel().ForAll(x => x.IsEnable = true);
                JournalTypes.Where(x => JournalEvents.Any(journalEvent => journalEvent.ClassId == x.Id && journalEvent.IsEnable)).
                             AsParallel().ForAll(x => x.IsEnable = true);
            }

            if (archiveFilter.DeviceDatabaseIds.IsNotNullOrEmpty())
            {
                Devices.Where(x => archiveFilter.DeviceDatabaseIds.Any(id => id == x.DatabaseId) == false).
                           AsParallel().ForAll(x => x.IsChecked = false);
            }
            IsDeviceFilterOn = archiveFilter.IsDeviceFilterOn;

            if (archiveFilter.Subsystems.IsNotNullOrEmpty())
            {
                Subsystems.Where(x => archiveFilter.Subsystems.Any(subsystem => subsystem == x.Subsystem)).
                           AsParallel().ForAll(x => x.IsEnable = true);
            }
        }

        void Initialize()
        {
            Title = "Настройки фильтра";

            JournalEvents = new List<EventViewModel>(
                FiresecClient.FiresecManager.GetDistinctRecords().
                Select(journalRecord => new EventViewModel(journalRecord.StateType, journalRecord.Description))
            );

            JournalTypes = new List<ClassViewModel>(
                JournalEvents.Select(x => x.ClassId).Distinct().
                Select(x => new ClassViewModel((StateType) x))
            );

            Devices = new List<DeviceViewModel>(
                FiresecClient.FiresecManager.DeviceConfiguration.Devices.Where(
                x => x.Driver.Category == DeviceCategoryType.Device).Select(x => new DeviceViewModel(x))
            );

            Subsystems = new List<SubsystemViewModel>();
            foreach (SubsystemType item in Enum.GetValues(typeof(SubsystemType)))
            {
                Subsystems.Add(new SubsystemViewModel(item));
            }

            ClearCommand = new RelayCommand(OnClear);
            SaveCommand = new RelayCommand(OnSave);
            CancelCommand = new RelayCommand(OnCancel);
        }

        public DateTime ArchiveFirstDate
        {
            get { return ArchiveViewModel.ArchiveFirstDate; }
        }

        public DateTime NowDate
        {
            get { return DateTime.Now; }
        }

        public List<ClassViewModel> JournalTypes { get; private set; }
        public List<EventViewModel> JournalEvents { get; private set; }
        public List<SubsystemViewModel> Subsystems { get; private set; }

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
                _startDate = new DateTime(value.Year, value.Month, value.Day,
                                          _startDate.Hour, _startDate.Minute, _startDate.Second);
                OnPropertyChanged("StartDate");
            }
        }
        public DateTime StartTime
        {
            get { return _startDate; }
            set
            {
                _startDate = new DateTime(_startDate.Year, _startDate.Month, _startDate.Day,
                                            value.Hour, value.Minute, value.Second);
                OnPropertyChanged("StartTime");
            }
        }

        DateTime _endDate;
        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = new DateTime(value.Year, value.Month, value.Day,
                                        _endDate.Hour, _endDate.Minute, _endDate.Second); ;
                OnPropertyChanged("EndDate");
            }
        }
        public DateTime EndTime
        {
            get { return _endDate; }
            set
            {
                _endDate = new DateTime(_endDate.Year, _endDate.Month, _endDate.Day,
                                          value.Hour, value.Minute, value.Second);
                OnPropertyChanged("EndTime");
            }
        }

        public List<DeviceViewModel> Devices { get; private set; }

        bool _isDeviceFilterOn;
        public bool IsDeviceFilterOn
        {
            get { return _isDeviceFilterOn; }
            set
            {
                _isDeviceFilterOn = value;
                OnPropertyChanged("IsDeviceFilterOn");
            }
        }

        public ArchiveFilter GetModel()
        {
            return new ArchiveFilter()
            {
                StartDate = StartDate,
                EndDate = EndDate,
                UseSystemDate = UseSystemDate,
                Descriptions = new List<string>(
                    JournalEvents.Where(x => x.IsEnable).Select(x => x.Name)
                ),
                DeviceDatabaseIds = new List<string>(
                    Devices.Where(x => x.IsChecked).Select(x => x.DatabaseId)
                ),
                IsDeviceFilterOn = IsDeviceFilterOn,
                Subsystems = new List<SubsystemType>(
                    Subsystems.Where(x => x.IsEnable).Select(x => x.Subsystem)
                ),
            };
        }

        public RelayCommand ClearCommand { get; private set; }
        void OnClear()
        {
            StartDate = StartTime = EndDate = EndTime = DateTime.Now;
            UseSystemDate = false;

            JournalTypes.ForEach(x => x.IsEnable = false);
            JournalEvents.ForEach(x => x.IsEnable = false);
            IsDeviceFilterOn = false;
            Devices.ForEach(x => x.IsChecked = true);
            Subsystems.ForEach(x => x.IsEnable = false);
        }

        public RelayCommand SaveCommand { get; private set; }
        void OnSave()
        {
            Close(true);
        }

        public RelayCommand CancelCommand { get; private set; }
        void OnCancel()
        {
            Close(false);
        }
    }
}