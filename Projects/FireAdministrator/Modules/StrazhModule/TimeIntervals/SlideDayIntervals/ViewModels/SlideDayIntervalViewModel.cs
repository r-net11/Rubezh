using System;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using StrazhAPI.SKD;
using Infrastructure;
using Infrastructure.Common;
using StrazhModule.Intervals.Base;
using StrazhModule.Intervals.Base.ViewModels;

namespace StrazhModule.ViewModels
{
	public class SlideDayIntervalViewModel : BaseIntervalViewModel<SlideDayIntervalPartViewModel, SKDSlideDayInterval>
	{
		private SlideDayIntervalsViewModel _slideDayIntervalsViewModel;

		public SlideDayIntervalViewModel(int index, SKDSlideDayInterval slideDayInterval, SlideDayIntervalsViewModel slideDayIntervalsViewModel)
			: base(index, slideDayInterval)
		{
			_slideDayIntervalsViewModel = slideDayIntervalsViewModel;
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			Initialize();
			Update();
		}

		public void Initialize()
		{
			Parts = new ObservableCollection<SlideDayIntervalPartViewModel>();
			if (Model != null)
				for (int i = 0; i < Model.DayIntervalIDs.Count; i++)
				{
					var slideDayIntervalPartViewModel = new SlideDayIntervalPartViewModel(_slideDayIntervalsViewModel, Model, i);
					Parts.Add(slideDayIntervalPartViewModel);
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
			Name = IsActive ? Model.Name : string.Format("Скользящий посуточный график {0}", Index);
			Description = IsActive ? Model.Description : string.Empty;
			StartDate = IsActive ? (DateTime?)Model.StartDate : null;
		}
		protected override void Activate()
		{
			if (IsActive && Model == null)
			{
				Model = new SKDSlideDayInterval()
				{
					ID = Index,
					Name = Name,
					StartDate = DateTime.Today,
				};
				Initialize();
				SKDManager.TimeIntervalsConfiguration.SlideDayIntervals.Add(Model);
				ServiceFactory.SaveService.SKDChanged = true;
			}
			else if (!IsActive && Model != null)
			{
				SKDManager.TimeIntervalsConfiguration.SlideDayIntervals.Remove(Model);
				Model = null;
				Initialize();
				ServiceFactory.SaveService.SKDChanged = true;
			}
			base.Activate();
		}

		public override void Paste(SKDSlideDayInterval interval)
		{
			IsActive = true;
			Model.StartDate = interval.StartDate;
			Model.DayIntervalIDs = interval.DayIntervalIDs;
			Initialize();
			ServiceFactory.SaveService.SKDChanged = true;
			Update();
		}

		public RelayCommand AddCommand { get; private set; }
		private void OnAdd()
		{
			Model.DayIntervalIDs.Add(0);
			var slideDayIntervalPartViewModel = new SlideDayIntervalPartViewModel(_slideDayIntervalsViewModel, Model, Model.DayIntervalIDs.Count - 1);
			Parts.Add(slideDayIntervalPartViewModel);
			SelectedPart = slideDayIntervalPartViewModel;
			ServiceFactory.SaveService.SKDChanged = true;
		}
		private bool CanAdd()
		{
			return Parts.Count < 31;
		}

		public RelayCommand RemoveCommand { get; private set; }
		private void OnRemove()
		{
			Model.DayIntervalIDs.RemoveAt(SelectedPart.Index);
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