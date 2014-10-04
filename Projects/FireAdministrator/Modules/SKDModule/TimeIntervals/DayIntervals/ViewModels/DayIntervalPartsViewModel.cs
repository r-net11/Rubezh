using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using SKDModule.Intervals.Base.ViewModels;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DayIntervalPartsViewModel : BaseViewModel
	{
		public SKDDayInterval DayInterval { get; set; }

		public DayIntervalPartsViewModel(SKDDayInterval dayInterval)
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			RemoveCommand = new RelayCommand(OnRemove, CanEdit);

			DayInterval = dayInterval;
			Parts = new ObservableCollection<DayIntervalPartViewModel>();
			foreach (var dayIntervalPart in dayInterval.DayIntervalParts)
			{
				var dayIntervalPartViewModel = new DayIntervalPartViewModel(dayIntervalPart);
				Parts.Add(dayIntervalPartViewModel);
			}
		}

		public ObservableCollection<DayIntervalPartViewModel> Parts { get; protected set; }

		private DayIntervalPartViewModel _selectedPart;
		public DayIntervalPartViewModel SelectedPart
		{
			get { return _selectedPart; }
			set
			{
				_selectedPart = value;
				OnPropertyChanged(() => SelectedPart);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			Edit(null);
		}
		bool CanAdd()
		{
			return Parts.Count < 4;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			DayInterval.DayIntervalParts.Remove(SelectedPart.DayIntervalPart);
			Parts.Remove(SelectedPart);
			ServiceFactory.SaveService.SKDChanged = true;
			ServiceFactory.SaveService.TimeIntervalChanged();
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			Edit(SelectedPart);
		}
		bool CanEdit()
		{
			return SelectedPart != null;
		}

		void Edit(DayIntervalPartViewModel dayIntervalPartViewModel)
		{
			var dayIntervalPartDetailsViewModel = new DayIntervalPartDetailsViewModel(dayIntervalPartViewModel == null ? null : dayIntervalPartViewModel.DayIntervalPart);
			if (DialogService.ShowModalWindow(dayIntervalPartDetailsViewModel))
			{
				if (dayIntervalPartViewModel == null)
				{
					DayInterval.DayIntervalParts.Add(dayIntervalPartDetailsViewModel.DayIntervalPart);
					dayIntervalPartViewModel = new DayIntervalPartViewModel(dayIntervalPartDetailsViewModel.DayIntervalPart);
					Parts.Add(dayIntervalPartViewModel);
					SelectedPart = dayIntervalPartViewModel;
				}
				SelectedPart.Update();
				ServiceFactory.SaveService.SKDChanged = true;
				ServiceFactory.SaveService.TimeIntervalChanged();
			}
		}

		bool ConfirmDeactivation()
		{
			var hasReference = SKDManager.TimeIntervalsConfiguration.WeeklyIntervals.Any(item => item.WeeklyIntervalParts.Any(part => part.DayIntervalID == DayInterval.No));
			if (!hasReference)
				hasReference = SKDManager.TimeIntervalsConfiguration.SlideDayIntervals.Any(item => item.DayIntervalIDs.Contains(DayInterval.No));
			return !hasReference || MessageBoxService.ShowConfirmation2("Данный дневной график используется в одном или нескольких недельных графиках, Вы уверены что хотите его деактивировать?");
		}
	}
}