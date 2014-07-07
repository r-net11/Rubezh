using System.Collections.Generic;
using System.Collections.ObjectModel;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Intervals.Base;
using Infrastructure;

namespace SKDModule.ViewModels
{
	public class WeeklyIntervalViewModel : BaseIntervalViewModel
	{
		private WeeklyIntervalsViewModel _weeklyIntervalsViewModel;
		public SKDWeeklyInterval WeeklyInterval { get; private set; }

		public WeeklyIntervalViewModel(int index, SKDWeeklyInterval weeklyInterval, WeeklyIntervalsViewModel weeklyIntervalsViewModel)
			: base(index, weeklyInterval != null)
		{
			_weeklyIntervalsViewModel = weeklyIntervalsViewModel;
			WeeklyInterval = weeklyInterval;
			Initialize();
			Update();
		}

		private void Initialize()
		{
			TimeIntervals = new ObservableCollection<WeeklyIntervalPartViewModel>();
			if (WeeklyInterval != null)
				foreach (var weeklyIntervalPart in WeeklyInterval.WeeklyIntervalParts)
				{
					var weeklyIntervalPartViewModel = new WeeklyIntervalPartViewModel(_weeklyIntervalsViewModel, weeklyIntervalPart);
					TimeIntervals.Add(weeklyIntervalPartViewModel);
				}
		}

		public ObservableCollection<WeeklyIntervalPartViewModel> TimeIntervals { get; private set; }

		private WeeklyIntervalPartViewModel _selectedTimeInterval;
		public WeeklyIntervalPartViewModel SelectedTimeInterval
		{
			get { return _selectedTimeInterval; }
			set
			{
				_selectedTimeInterval = value;
				OnPropertyChanged(() => SelectedTimeInterval);
			}
		}

		private string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		private string _description;
		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				OnPropertyChanged(() => Description);
			}
		}

		public override void Update()
		{
			base.Update();
			Name = IsActive ? WeeklyInterval.Name : string.Format("Понедельный график {0}", Index);
			Description = IsEnabled ? WeeklyInterval.Description : string.Empty;
			OnPropertyChanged(() => WeeklyInterval);
			OnPropertyChanged(() => TimeIntervals);
		}
		protected override void Activate()
		{
			if (!IsDefault)
			{
				if (IsActive && WeeklyInterval == null)
				{
					WeeklyInterval = new SKDWeeklyInterval()
					{
						ID = Index,
						Name = Name,
					};
					Initialize();
					SKDManager.TimeIntervalsConfiguration.WeeklyIntervals.Add(WeeklyInterval);
					ServiceFactory.SaveService.SKDChanged = true;
				}
				else if (!IsActive && WeeklyInterval != null)
				{
					SKDManager.TimeIntervalsConfiguration.WeeklyIntervals.Remove(WeeklyInterval);
					WeeklyInterval = null;
					Initialize();
					SKDManager.TimeIntervalsConfiguration.SlideWeeklyIntervals.ForEach(week => week.InvalidateWeekIntervals());
					ServiceFactory.SaveService.SKDChanged = true;
				}
			}
			base.Activate();
		}

		public void Paste(SKDWeeklyInterval interval)
		{
			IsActive = true;
			for (int i = 0; i < interval.WeeklyIntervalParts.Count; i++)
				WeeklyInterval.WeeklyIntervalParts[i].TimeIntervalID = interval.WeeklyIntervalParts[i].TimeIntervalID;
			Initialize();
			ServiceFactory.SaveService.SKDChanged = true;
			Update();
		}
	}
}