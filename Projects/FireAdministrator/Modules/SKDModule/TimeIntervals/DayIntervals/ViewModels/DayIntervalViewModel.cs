using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using SKDModule.Intervals.Base.ViewModels;

namespace SKDModule.ViewModels
{
	public class DayIntervalViewModel : BaseIntervalViewModel<DayIntervalPartViewModel, SKDDayInterval>
	{
		public DayIntervalViewModel(int index, SKDDayInterval dayInterval)
			: base(index, dayInterval)
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
			Name = IsActive ? Model.Name : string.Format("Дневной график {0}", Index);
			Description = IsActive ? Model.Description : string.Empty;
		}
		protected override void Activate()
		{
			if (IsActive && Model == null)
			{
				Model = new SKDDayInterval()
				{
					ID = Index,
					Name = Name,
					DayIntervalParts = new List<SKDDayIntervalPart>(),
				};
				InitParts();
				SKDManager.TimeIntervalsConfiguration.DayIntervals.Add(Model);
				ServiceFactory.SaveService.SKDChanged = true;
				ServiceFactory.SaveService.TimeIntervalChanged();
			}
			else if (!IsActive && Model != null)
			{
				if (ConfirmDeactivation())
				{
					SKDManager.TimeIntervalsConfiguration.DayIntervals.Remove(Model);
					Model = null;
					InitParts();
					SKDManager.TimeIntervalsConfiguration.WeeklyIntervals.ForEach(week => week.InvalidateDayIntervals());
					SKDManager.TimeIntervalsConfiguration.SlideDayIntervals.ForEach(week => week.InvalidateDayIntervals());
					ServiceFactory.SaveService.SKDChanged = true;
					ServiceFactory.SaveService.TimeIntervalChanged();
				}
				else
					IsActive = true;
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
			Model.DayIntervalParts.Remove(SelectedPart.DayIntervalPart);
			Parts.Remove(SelectedPart);
			ServiceFactory.SaveService.SKDChanged = true;
			ServiceFactory.SaveService.TimeIntervalChanged();
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			Edit(SelectedPart);
		}
		private bool CanEdit()
		{
			return SelectedPart != null;
		}

		public override void Paste(SKDDayInterval dayInterval)
		{
			IsActive = true;
			Model.DayIntervalParts = dayInterval.DayIntervalParts;
			InitParts();
			ServiceFactory.SaveService.SKDChanged = true;
			ServiceFactory.SaveService.TimeIntervalChanged();
			Update();
		}
		private void InitParts()
		{
			Parts = new ObservableCollection<DayIntervalPartViewModel>();
			if (Model != null)
				foreach (var dayIntervalPart in Model.DayIntervalParts)
				{
					var dayIntervalPartViewModel = new DayIntervalPartViewModel(dayIntervalPart);
					Parts.Add(dayIntervalPartViewModel);
				}
		}

		private void Edit(DayIntervalPartViewModel dayIntervalPartViewModel)
		{
			var dayIntervalPartDetailsViewModel = new DayIntervalPartDetailsViewModel(dayIntervalPartViewModel == null ? null : dayIntervalPartViewModel.DayIntervalPart);
			if (DialogService.ShowModalWindow(dayIntervalPartDetailsViewModel))
			{
				if (dayIntervalPartViewModel == null)
				{
					Model.DayIntervalParts.Add(dayIntervalPartDetailsViewModel.DayIntervalPart);
					dayIntervalPartViewModel = new DayIntervalPartViewModel(dayIntervalPartDetailsViewModel.DayIntervalPart);
					Parts.Add(dayIntervalPartViewModel);
					SelectedPart = dayIntervalPartViewModel;
				}
				SelectedPart.Update();
				ServiceFactory.SaveService.SKDChanged = true;
				ServiceFactory.SaveService.TimeIntervalChanged();
			}
		}

		private bool ConfirmDeactivation()
		{
			var hasReference = SKDManager.TimeIntervalsConfiguration.WeeklyIntervals.Any(item => item.WeeklyIntervalParts.Any(part => part.DayIntervalID == Index));
			if (!hasReference)
				hasReference = SKDManager.TimeIntervalsConfiguration.SlideDayIntervals.Any(item => item.DayIntervalIDs.Contains(Index));
			return !hasReference || MessageBoxService.ShowConfirmation2("Данный дневной график используется в одном или нескольких недельных графиках, Вы уверены что хотите его деактивировать?");
		}
	}
}