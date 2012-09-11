using System;
using System.Collections.ObjectModel;
using System.Linq;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Models;

namespace GKModule.ViewModels
{
    public class ArchiveSettingsViewModel : SaveCancelDialogViewModel
    {
        public ArchiveSettingsViewModel(ArchiveDefaultState archiveDefaultState)
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
                        EndDate = archiveDefaultState.EndDate.Value;
                    break;

                case ArchiveDefaultStateType.All:
                default:
                    break;
            }
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

        protected override bool Save()
        {
            return base.Save();
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