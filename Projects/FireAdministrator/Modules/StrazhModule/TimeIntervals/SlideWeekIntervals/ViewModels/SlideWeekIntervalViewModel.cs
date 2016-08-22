using System;
using Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Localization.Strazh.ViewModels;
using StrazhAPI.SKD;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using StrazhModule.Intervals.Base;
using StrazhModule.Intervals.Base.ViewModels;

namespace StrazhModule.ViewModels
{
	public class SlideWeekIntervalViewModel : BaseIntervalViewModel<SlideWeekIntervalPartViewModel, SKDSlideWeeklyInterval>
	{
		private SlideWeekIntervalsViewModel _slideWeekIntervalsViewModel;

		public SlideWeekIntervalViewModel(int index, SKDSlideWeeklyInterval slideWeekInterval, SlideWeekIntervalsViewModel slideWeekIntervalsViewModel)
			: base(index, slideWeekInterval)
		{
			_slideWeekIntervalsViewModel = slideWeekIntervalsViewModel;
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			Initialize();
			Update();
		}

		public void Initialize()
		{
			Parts = new ObservableCollection<SlideWeekIntervalPartViewModel>();
			if (Model != null)
				for (int i = 0; i < Model.WeeklyIntervalIDs.Count; i++)
				{
					var slideWeekIntervalPartViewModel = new SlideWeekIntervalPartViewModel(_slideWeekIntervalsViewModel, Model, i);
					Parts.Add(slideWeekIntervalPartViewModel);
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
			Name = IsActive ? Model.Name : string.Format(CommonViewModels.SlideWeekSchedule, Index);
			Description = IsActive ? Model.Description : string.Empty;
			StartDate = IsActive ? (DateTime?)Model.StartDate : null;
		}
		protected override void Activate()
		{
			if (IsActive && Model == null)
			{
				Model = new SKDSlideWeeklyInterval()
				{
					ID = Index,
					Name = Name,
					StartDate = DateTime.Today,
				};
				Initialize();
				SKDManager.TimeIntervalsConfiguration.SlideWeeklyIntervals.Add(Model);
				ServiceFactory.SaveService.SKDChanged = true;
			}
			else if (!IsActive && Model != null)
			{
				SKDManager.TimeIntervalsConfiguration.SlideWeeklyIntervals.Remove(Model);
				Model = null;
				Initialize();
				ServiceFactory.SaveService.SKDChanged = true;
			}
			base.Activate();
		}

		public override void Paste(SKDSlideWeeklyInterval interval)
		{
			IsActive = true;
			Model.StartDate = interval.StartDate;
			Model.WeeklyIntervalIDs = interval.WeeklyIntervalIDs;
			Initialize();
			ServiceFactory.SaveService.SKDChanged = true;
			Update();
		}

		public RelayCommand AddCommand { get; private set; }
		private void OnAdd()
		{
			Model.WeeklyIntervalIDs.Add(0);
			var slideWeekIntervalPartViewModel = new SlideWeekIntervalPartViewModel(_slideWeekIntervalsViewModel, Model, Model.WeeklyIntervalIDs.Count - 1);
			Parts.Add(slideWeekIntervalPartViewModel);
			SelectedPart = slideWeekIntervalPartViewModel;
			ServiceFactory.SaveService.SKDChanged = true;
		}
		private bool CanAdd()
		{
			return Parts.Count < 31;
		}

		public RelayCommand RemoveCommand { get; private set; }
		private void OnRemove()
		{
			Model.WeeklyIntervalIDs.RemoveAt(SelectedPart.Index);
			Parts.RemoveAt(Parts.Count - 1);
			Parts.ForEach(item => item.Update());
			ServiceFactory.SaveService.SKDChanged = true;
			if (SelectedPart == null)
				SelectedPart = Parts.FirstOrDefault();
		}
		private bool CanRemove()
		{
			return SelectedPart != null && Parts.Count > 1;
		}
	}
}