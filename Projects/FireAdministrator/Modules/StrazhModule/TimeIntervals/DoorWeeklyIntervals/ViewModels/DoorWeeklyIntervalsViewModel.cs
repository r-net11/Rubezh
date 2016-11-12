using Common;
using Infrastructure;
using Infrastructure.Common.Windows;
using StrazhAPI.SKD;
using StrazhModule.Intervals.Base.ViewModels;
using System.Collections.ObjectModel;
using System.Linq;

namespace StrazhModule.ViewModels
{
    public class DoorWeeklyIntervalsViewModel : BaseIntervalsViewModel<DoorWeeklyIntervalViewModel, DoorWeeklyIntervalPartViewModel, SKDDoorWeeklyInterval>
    {
        public ObservableCollection<SKDDoorDayInterval> AvailableDayIntervals { get; private set; }

        public override void Initialize()
        {
            base.Initialize();
            var map = SKDManager.TimeIntervalsConfiguration.DoorWeeklyIntervals.ToDictionary(item => item.ID);
            Intervals = new ObservableCollection<DoorWeeklyIntervalViewModel>();
            for (int i = 0; i <= 127; i++)
                Intervals.Add(new DoorWeeklyIntervalViewModel(i, map.ContainsKey(i) ? map[i] : null, this));
            SelectedInterval = Intervals.FirstOrDefault();
        }

        protected override void OnEdit()
        {
            var weeklyIntervalDetailsViewModel = new DoorWeeklyIntervalDetailsViewModel(SelectedInterval.Model);
            if (DialogService.ShowModalWindow(weeklyIntervalDetailsViewModel))
            {
                SelectedInterval.Update();
                ServiceFactory.SaveService.SKDChanged = true;
                ServiceFactory.SaveService.TimeIntervalChanged();
            }
        }

        protected override SKDDoorWeeklyInterval CopyInterval(SKDDoorWeeklyInterval source)
        {
            var copy = new SKDDoorWeeklyInterval
            {
                Name = source.Name,
                Description = source.Description,
            };
            source.WeeklyIntervalParts.ForEach(part => copy.WeeklyIntervalParts.Add(new SKDDoorWeeklyIntervalPart { DayOfWeek = part.DayOfWeek, DayIntervalUID = part.DayIntervalUID }));
            return copy;
        }

        protected override void BuildIntervals()
        {
            AvailableDayIntervals = new ObservableCollection<SKDDoorDayInterval>(SKDManager.TimeIntervalsConfiguration.DoorDayIntervals);
            OnPropertyChanged(() => AvailableDayIntervals);
            if (SelectedInterval != null)
                SelectedInterval.Parts.ForEach(item => item.Update());
        }
    }
}