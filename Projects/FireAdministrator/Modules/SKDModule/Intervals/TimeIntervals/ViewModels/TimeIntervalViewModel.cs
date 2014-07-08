using System.Collections.Generic;
using System.Collections.ObjectModel;
using FiresecAPI.SKD;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using SKDModule.Intervals.Base;
using Common;
using SKDModule.Intervals.Base.ViewModels;

namespace SKDModule.ViewModels
{
	public class TimeIntervalViewModel : BaseIntervalViewModel<TimeIntervalPartViewModel, SKDTimeInterval>
	{
		public TimeIntervalViewModel(int index, SKDTimeInterval timeInterval)
			: base(index, timeInterval)
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			RemoveCommand = new RelayCommand(OnRemove, CanEdit);
			InitParts();
			Update();
		}

		public override void Update()
		{
			base.Update();
			Name = IsDefault ? "Никогда" : (IsActive ? Model.Name : string.Format("Дневной график {0}", Index));
			Description = IsEnabled ? Model.Description : string.Empty;
		}
		protected override void Activate()
		{
			if (!IsDefault)
			{
				if (IsActive && Model == null)
				{
					Model = new SKDTimeInterval()
					{
						ID = Index,
						Name = Name,
						TimeIntervalParts = new List<SKDTimeIntervalPart>(),
					};
					InitParts();
					SKDManager.TimeIntervalsConfiguration.TimeIntervals.Add(Model);
					ServiceFactory.SaveService.SKDChanged = true;
				}
				else if (!IsActive && Model != null)
				{
					SKDManager.TimeIntervalsConfiguration.TimeIntervals.Remove(Model);
					Model = null;
					InitParts();
					SKDManager.TimeIntervalsConfiguration.WeeklyIntervals.ForEach(week => week.InvalidateDayIntervals());
					SKDManager.TimeIntervalsConfiguration.SlideDayIntervals.ForEach(week => week.InvalidateDayIntervals());
					ServiceFactory.SaveService.SKDChanged = true;
				}
			}
			base.Activate();
		}

		public RelayCommand AddCommand { get; private set; }
		private void OnAdd()
		{
			Edit(null);
		}
		private bool CanAdd()
		{
			return Parts.Count < 4;
		}

		public RelayCommand RemoveCommand { get; private set; }
		private void OnRemove()
		{
			Model.TimeIntervalParts.Remove(SelectedPart.TimeIntervalPart);
			Parts.Remove(SelectedPart);
			ServiceFactory.SaveService.SKDChanged = true;
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			Edit(SelectedPart);
		}
		private bool CanEdit()
		{
			return !IsDefault && SelectedPart != null;
		}

		public override void Paste(SKDTimeInterval timeInterval)
		{
			IsActive = true;
			Model.TimeIntervalParts = timeInterval.TimeIntervalParts;
			InitParts();
			ServiceFactory.SaveService.SKDChanged = true;
			Update();
		}
		private void InitParts()
		{
			Parts = new ObservableCollection<TimeIntervalPartViewModel>();
			if (Model != null)
				foreach (var timeIntervalPart in Model.TimeIntervalParts)
				{
					var timeIntervalPartViewModel = new TimeIntervalPartViewModel(timeIntervalPart);
					Parts.Add(timeIntervalPartViewModel);
				}
		}

		private void Edit(TimeIntervalPartViewModel timeIntervalPartViewModel)
		{
			var timeIntervalPartDetailsViewModel = new TimeIntervalPartDetailsViewModel(timeIntervalPartViewModel == null ? null : timeIntervalPartViewModel.TimeIntervalPart);
			if (DialogService.ShowModalWindow(timeIntervalPartDetailsViewModel))
			{
				if (timeIntervalPartViewModel == null)
				{
					Model.TimeIntervalParts.Add(timeIntervalPartDetailsViewModel.TimeIntervalPart);
					timeIntervalPartViewModel = new TimeIntervalPartViewModel(timeIntervalPartDetailsViewModel.TimeIntervalPart);
					Parts.Add(timeIntervalPartViewModel);
					SelectedPart = timeIntervalPartViewModel;
				}
				SelectedPart.Update();
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
	}
}