using System;
using Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Intervals.Base;

namespace SKDModule.ViewModels
{
	public class SlideWeekIntervalViewModel : BaseIntervalViewModel
	{
		private SlideWeekIntervalsViewModel _slideWeekIntervalsViewModel;
		public SKDSlideWeeklyInterval SlideWeekInterval { get; private set; }

		public SlideWeekIntervalViewModel(int index, SKDSlideWeeklyInterval slideWeekInterval, SlideWeekIntervalsViewModel slideWeekIntervalsViewModel)
			: base(index, slideWeekInterval != null)
		{
			_slideWeekIntervalsViewModel = slideWeekIntervalsViewModel;
			SlideWeekInterval = slideWeekInterval;
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			Initialize();
			Update();
		}

		public void Initialize()
		{
			WeekIntervals = new ObservableCollection<SlideWeekIntervalPartViewModel>();
			if (SlideWeekInterval != null)
				for (int i = 0; i < SlideWeekInterval.WeeklyIntervalIDs.Count; i++)
				{
					var slideWeekIntervalPartViewModel = new SlideWeekIntervalPartViewModel(_slideWeekIntervalsViewModel, SlideWeekInterval, i);
					WeekIntervals.Add(slideWeekIntervalPartViewModel);
				}
		}

		public ObservableCollection<SlideWeekIntervalPartViewModel> WeekIntervals { get; private set; }

		private SlideWeekIntervalPartViewModel _selectedWeekInterval;
		public SlideWeekIntervalPartViewModel SelectedWeekInterval
		{
			get { return _selectedWeekInterval; }
			set
			{
				_selectedWeekInterval = value;
				OnPropertyChanged(() => SelectedWeekInterval);
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

		private DateTime? _startDate;
		public DateTime? StartDate
		{
			get { return _startDate; }
			set
			{
				_startDate = value;
				OnPropertyChanged(() => StartDate);
			}
		}

		public override void Update()
		{
			base.Update();
			Name = IsActive ? SlideWeekInterval.Name : string.Format("Скользящий понедельный график {0}", Index);
			Description = IsEnabled ? SlideWeekInterval.Description : string.Empty;
			StartDate = IsEnabled ? (DateTime?)SlideWeekInterval.StartDate : null;
			OnPropertyChanged(() => SlideWeekInterval);
			OnPropertyChanged(() => WeekIntervals);
		}
		protected override void Activate()
		{
			if (!IsDefault)
			{
				if (IsActive && SlideWeekInterval == null)
				{
					SlideWeekInterval = new SKDSlideWeeklyInterval()
					{
						ID = Index,
						Name = Name,
						StartDate = DateTime.Today,
					};
					Initialize();
					SKDManager.TimeIntervalsConfiguration.SlideWeeklyIntervals.Add(SlideWeekInterval);
					ServiceFactory.SaveService.SKDChanged = true;
				}
				else if (!IsActive && SlideWeekInterval != null)
				{
					SKDManager.TimeIntervalsConfiguration.SlideWeeklyIntervals.Remove(SlideWeekInterval);
					SlideWeekInterval = null;
					Initialize();
					ServiceFactory.SaveService.SKDChanged = true;
				}
			}
			base.Activate();
		}

		public void Paste(SKDSlideWeeklyInterval interval)
		{
			IsActive = true;
			SlideWeekInterval.StartDate = interval.StartDate;
			SlideWeekInterval.WeeklyIntervalIDs = interval.WeeklyIntervalIDs;
			Initialize();
			ServiceFactory.SaveService.SKDChanged = true;
			Update();
		}

		public RelayCommand AddCommand { get; private set; }
		private void OnAdd()
		{
			SlideWeekInterval.WeeklyIntervalIDs.Add(0);
			var slideWeekIntervalPartViewModel = new SlideWeekIntervalPartViewModel(_slideWeekIntervalsViewModel, SlideWeekInterval, SlideWeekInterval.WeeklyIntervalIDs.Count - 1);
			WeekIntervals.Add(slideWeekIntervalPartViewModel);
			SelectedWeekInterval = slideWeekIntervalPartViewModel;
			ServiceFactory.SaveService.SKDChanged = true;
		}
		private bool CanAdd()
		{
			return WeekIntervals.Count < 31;
		}

		public RelayCommand RemoveCommand { get; private set; }
		private void OnRemove()
		{
			SlideWeekInterval.WeeklyIntervalIDs.RemoveAt(SelectedWeekInterval.Index);
			WeekIntervals.RemoveAt(WeekIntervals.Count - 1);
			WeekIntervals.ForEach(item => item.Update());
			ServiceFactory.SaveService.SKDChanged = true;
			if (SelectedWeekInterval == null)
				SelectedWeekInterval = WeekIntervals.FirstOrDefault();
		}
		private bool CanRemove()
		{
			return SelectedWeekInterval != null && WeekIntervals.Count > 1;
		}
	}
}