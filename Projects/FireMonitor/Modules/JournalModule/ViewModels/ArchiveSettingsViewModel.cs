using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure;
using Infrastructure.Common;
using JournalModule.Events;

namespace JournalModule.ViewModels
{
    public class ArchiveSettingsViewModel : DialogContent
    {
        public ArchiveSettingsViewModel(ArchiveDefaultState archiveDefaultState)
        {
            Initialize();

            ArchiveDefaultStates.First(x => x.ArchiveDefaultStateType == archiveDefaultState.ArchiveDefaultStateType).IsActive = true;
            switch (archiveDefaultState.ArchiveDefaultStateType)
            {
                case ArchiveDefaultStateType.LastHours:
                    if (archiveDefaultState.Count.HasValue)
                        HoursCount = archiveDefaultState.Count.Value;
                    break;

                case ArchiveDefaultStateType.LastDays:
                    if (archiveDefaultState.Count.HasValue)
                        DaysCount = archiveDefaultState.Count.Value;
                    break;

                case ArchiveDefaultStateType.FromDate:
                    if (archiveDefaultState.StartDate.HasValue)
                        StartDate = archiveDefaultState.StartDate.Value;
                    break;

                case ArchiveDefaultStateType.RangeDate:
                    if (archiveDefaultState.StartDate.HasValue)
                        StartDate = archiveDefaultState.StartDate.Value;
                    if (archiveDefaultState.EndDate.HasValue)
                        StartDate = archiveDefaultState.EndDate.Value;
                    break;

                case ArchiveDefaultStateType.All:
                default:
                    break;
            }
        }

        void Initialize()
        {
            Title = "Настройки";

            ArchiveDefaultStates = new ObservableCollection<ArchiveDefaultStateViewModel>();
            foreach (ArchiveDefaultStateType item in Enum.GetValues(typeof(ArchiveDefaultStateType)))
            {
                ArchiveDefaultStates.Add(new ArchiveDefaultStateViewModel(item));
            }

            HoursCount = 1;
            DaysCount = 1;
            StartDate = ArchiveFirstDate;
            EndDate = NowDate;

            ServiceFactory.Events.GetEvent<ArchiveDefaultStateCheckedEvent>().Subscribe(OnArchiveDefaultStateCheckedEvent);
            SaveCommand = new RelayCommand(OnSave);
            CancelCommand = new RelayCommand(OnCancel);
        }

        public ObservableCollection<ArchiveDefaultStateViewModel> ArchiveDefaultStates { get; private set; }

        ArchiveDefaultStateType _checkedArchiveDefaultStateType;
        public ArchiveDefaultStateType CheckedArchiveDefaultStateType
        {
            get { return _checkedArchiveDefaultStateType; }
            set
            {
                _checkedArchiveDefaultStateType = value;
                OnPropertyChanged("CheckedArchiveDefaultStateType");
            }
        }

        public int HoursCount { get; set; }
        public int DaysCount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public DateTime ArchiveFirstDate
        {
            get { return ArchiveViewModel.ArchiveFirstDate; }
        }

        public DateTime NowDate
        {
            get { return DateTime.Now; }
        }

        public ArchiveDefaultState GetModel()
        {
            var archiveDefaultState = new ArchiveDefaultState();
            archiveDefaultState.ArchiveDefaultStateType = ArchiveDefaultStates.First(x => x.IsActive).ArchiveDefaultStateType;
            switch (archiveDefaultState.ArchiveDefaultStateType)
            {
                case ArchiveDefaultStateType.LastHours:
                    archiveDefaultState.Count = HoursCount;
                    break;

                case ArchiveDefaultStateType.LastDays:
                    archiveDefaultState.Count = DaysCount;
                    break;

                case ArchiveDefaultStateType.FromDate:
                    archiveDefaultState.StartDate = StartDate;
                    break;

                case ArchiveDefaultStateType.RangeDate:
                    archiveDefaultState.StartDate = StartDate;
                    archiveDefaultState.EndDate = EndDate;
                    break;

                case ArchiveDefaultStateType.All:
                default:
                    break;
            }

            return archiveDefaultState;
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

        void OnArchiveDefaultStateCheckedEvent(ArchiveDefaultStateViewModel archiveDefaultState)
        {
            foreach (var defaultState in ArchiveDefaultStates.Where(x => x != archiveDefaultState))
            {
                defaultState.IsActive = false;
            }

            CheckedArchiveDefaultStateType = archiveDefaultState.ArchiveDefaultStateType;
        }
    }
}