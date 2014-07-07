using System;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using FiresecAPI.SKD;
using Infrastructure;
using Infrastructure.Common;
using SKDModule.Intervals.Base;

namespace SKDModule.ViewModels
{
	public class SlideDayIntervalViewModel : BaseIntervalViewModel
	{
		private SlideDayIntervalsViewModel _slideDayIntervalsViewModel;
		public SKDSlideDayInterval SlideDayInterval { get; private set; }

		public SlideDayIntervalViewModel(int index, SKDSlideDayInterval slideDayInterval, SlideDayIntervalsViewModel slideDayIntervalsViewModel)
			: base(index, slideDayInterval != null)
		{
			_slideDayIntervalsViewModel = slideDayIntervalsViewModel;
			SlideDayInterval = slideDayInterval;
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			Initialize();
			Update();
		}

		public void Initialize()
		{
			TimeIntervals = new ObservableCollection<SlideDayIntervalPartViewModel>();
			if (SlideDayInterval != null)
				for (int i = 0; i < SlideDayInterval.TimeIntervalIDs.Count; i++)
				{
					var slideDayIntervalPartViewModel = new SlideDayIntervalPartViewModel(_slideDayIntervalsViewModel, SlideDayInterval, i);
					TimeIntervals.Add(slideDayIntervalPartViewModel);
				}
		}

		public ObservableCollection<SlideDayIntervalPartViewModel> TimeIntervals { get; private set; }

		private SlideDayIntervalPartViewModel _selectedTimeInterval;
		public SlideDayIntervalPartViewModel SelectedTimeInterval
		{
			get { return _selectedTimeInterval; }
			set
			{
				_selectedTimeInterval = value;
				OnPropertyChanged("SelectedTimeInterval");
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
			Name = IsActive ? SlideDayInterval.Name : string.Format("Скользящий посуточный график {0}", Index);
			Description = IsEnabled ? SlideDayInterval.Description : string.Empty;
			StartDate = IsEnabled ? (DateTime?)SlideDayInterval.StartDate : null;
			OnPropertyChanged(() => SlideDayInterval);
			OnPropertyChanged(() => TimeIntervals);
		}
		protected override void Activate()
		{
			if (!IsDefault)
			{
				if (IsActive && SlideDayInterval == null)
				{
					SlideDayInterval = new SKDSlideDayInterval()
					{
						ID = Index,
						Name = Name,
						StartDate = DateTime.Today,
					};
					Initialize();
					SKDManager.TimeIntervalsConfiguration.SlideDayIntervals.Add(SlideDayInterval);
					ServiceFactory.SaveService.SKDChanged = true;
				}
				else if (!IsActive && SlideDayInterval != null)
				{
					SKDManager.TimeIntervalsConfiguration.SlideDayIntervals.Remove(SlideDayInterval);
					SlideDayInterval = null;
					Initialize();
					ServiceFactory.SaveService.SKDChanged = true;
				}
			}
			base.Activate();
		}

		public void Paste(SKDSlideDayInterval interval)
		{
			IsActive = true;
			SlideDayInterval.StartDate = interval.StartDate;
			SlideDayInterval.TimeIntervalIDs = interval.TimeIntervalIDs;
			Initialize();
			ServiceFactory.SaveService.SKDChanged = true;
			Update();
		}

		public RelayCommand AddCommand { get; private set; }
		private void OnAdd()
		{
			SlideDayInterval.TimeIntervalIDs.Add(0);
			var slideDayIntervalPartViewModel = new SlideDayIntervalPartViewModel(_slideDayIntervalsViewModel, SlideDayInterval, SlideDayInterval.TimeIntervalIDs.Count - 1);
			TimeIntervals.Add(slideDayIntervalPartViewModel);
			SelectedTimeInterval = slideDayIntervalPartViewModel;
			ServiceFactory.SaveService.SKDChanged = true;
		}
		private bool CanAdd()
		{
			return TimeIntervals.Count < 31;
		}

		public RelayCommand RemoveCommand { get; private set; }
		private void OnRemove()
		{
			SlideDayInterval.TimeIntervalIDs.RemoveAt(SelectedTimeInterval.Index);
			TimeIntervals.RemoveAt(TimeIntervals.Count - 1);
			TimeIntervals.ForEach(item => item.Update());
			ServiceFactory.SaveService.SKDChanged = true;
			if (SelectedTimeInterval == null)
				SelectedTimeInterval = TimeIntervals.FirstOrDefault();
		}
		private bool CanRemove()
		{
			return SelectedTimeInterval != null && TimeIntervals.Count > 1;
		}
	}
}