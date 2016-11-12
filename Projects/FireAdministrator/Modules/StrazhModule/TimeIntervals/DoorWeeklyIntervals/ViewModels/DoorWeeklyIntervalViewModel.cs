using Infrastructure;
using Infrastructure.Common.Windows;
using Localization.Strazh.ViewModels;
using StrazhAPI.SKD;
using StrazhModule.Intervals.Base.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace StrazhModule.ViewModels
{
    public class DoorWeeklyIntervalViewModel : BaseIntervalViewModel<DoorWeeklyIntervalPartViewModel, SKDDoorWeeklyInterval>
    {
        readonly DoorWeeklyIntervalsViewModel _weeklyIntervalsViewModel;

        public DoorWeeklyIntervalViewModel(int index, SKDDoorWeeklyInterval weeklyInterval, DoorWeeklyIntervalsViewModel weeklyIntervalsViewModel)
            : base(index, weeklyInterval)
        {
            _weeklyIntervalsViewModel = weeklyIntervalsViewModel;
            Initialize();
            Update();
        }

        void Initialize()
        {
            Parts = new ObservableCollection<DoorWeeklyIntervalPartViewModel>();
            if (Model != null)
                foreach (var weeklyIntervalPart in Model.WeeklyIntervalParts)
                {
                    var weeklyIntervalPartViewModel = new DoorWeeklyIntervalPartViewModel(_weeklyIntervalsViewModel, weeklyIntervalPart);
                    Parts.Add(weeklyIntervalPartViewModel);
                }
        }

        public override void Update()
        {
            base.Update();
            Name = IsActive ? Model.Name : string.Format(CommonViewModels.WeekSchedule, Index);
            Description = IsActive ? Model.Description : string.Empty;
        }
        protected override void Activate()
        {
            if (IsActive && Model == null)
            {
                Model = new SKDDoorWeeklyInterval
                {
                    ID = Index,
                    Name = Name,
                    WeeklyIntervalParts = SKDDoorWeeklyInterval.CreateParts()
                };

                var dayIntervalCard = SKDManager.TimeIntervalsConfiguration.DoorDayIntervals.FirstOrDefault(x => x.Name == TimeIntervalsConfiguration.PredefinedIntervalNameCard);
                if (dayIntervalCard != null)
                    Model.WeeklyIntervalParts.ForEach(day => day.DayIntervalUID = dayIntervalCard.UID);

                Initialize();
                SKDManager.TimeIntervalsConfiguration.DoorWeeklyIntervals.Add(Model);
                ServiceFactory.SaveService.SKDChanged = true;
                ServiceFactory.SaveService.TimeIntervalChanged();
            }
            else if (!IsActive && Model != null)
            {
                if (ConfirmDeactivation())
                {
                    // Для замков, которые ссылались на данный недельный график замка,
                    // задаем предустановленный недельный график замка <Карта>
                    foreach (var device in GetLinkedLocks())
                    {
                        var skdDoorWeeklyInterval = SKDManager.TimeIntervalsConfiguration.DoorWeeklyIntervals.FirstOrDefault(
                            x => x.Name == TimeIntervalsConfiguration.PredefinedIntervalNameCard);
                        if (skdDoorWeeklyInterval != null)
                            device.SKDDoorConfiguration.WeeklyIntervalID =
                                skdDoorWeeklyInterval.ID;
                    }

                    SKDManager.TimeIntervalsConfiguration.DoorWeeklyIntervals.Remove(Model);
                    Model = null;
                    Initialize();
                    //SKDManager.TimeIntervalsConfiguration.SlideWeeklyIntervals.ForEach(week => week.InvalidateWeekIntervals());
                    ServiceFactory.SaveService.SKDChanged = true;
                    ServiceFactory.SaveService.TimeIntervalChanged();
                }
                else
                    IsActive = true;
            }
            base.Activate();
        }

        public override void Paste(SKDDoorWeeklyInterval interval)
        {
            IsActive = true;
            Model.Name = GenerateNewNameBeforePaste(interval.Name);
            for (int i = 0; i < interval.WeeklyIntervalParts.Count; i++)
            {
                Model.WeeklyIntervalParts[i].DayIntervalUID = interval.WeeklyIntervalParts[i].DayIntervalUID;
            }
            Initialize();
            ServiceFactory.SaveService.SKDChanged = true;
            ServiceFactory.SaveService.TimeIntervalChanged();
            Update();
        }

        private string GenerateNewNameBeforePaste(string name)
        {
            string newName;
            var i = 1;

            do
                newName = String.Format("{0} ({1})", name, i++);
            while (_weeklyIntervalsViewModel.Intervals.Any(x => x.Name == newName));

            return newName;
        }

        bool ConfirmDeactivation()
        {
            var hasReference = false;
            foreach (var device in SKDManager.Devices.Where(x => x.DriverType == SKDDriverType.Lock))
            {
                if (device.SKDDoorConfiguration == null)
                    continue;
                if (device.SKDDoorConfiguration.WeeklyIntervalID == Model.ID)
                {
                    hasReference = true;
                    break;
                }

            }
            return hasReference
                ? MessageBoxService.ShowQuestion(
                    String.Format(
                        CommonViewModels.WeekLockSchedule_DeleteForLocksConfirm,
                        Name), null, MessageBoxImage.Warning)
                : MessageBoxService.ShowQuestion(String.Format(CommonViewModels.WeekLockSchedule_DeleteConfirm, Name));
        }

        IEnumerable<SKDDevice> GetLinkedLocks()
        {
            return
                SKDManager.Devices.Where(
                    x =>
                        x.DriverType == SKDDriverType.Lock && x.SKDDoorConfiguration != null &&
                        x.SKDDoorConfiguration.WeeklyIntervalID == Model.ID).ToList();
        }

        public override bool IsPredefined
        {
            get
            {
                return Name == TimeIntervalsConfiguration.PredefinedIntervalNameCard
                    || Name == TimeIntervalsConfiguration.PredefinedIntervalNamePassword
                    || Name == TimeIntervalsConfiguration.PredefinedIntervalNameCardAndPassword;
            }
        }
    }
}