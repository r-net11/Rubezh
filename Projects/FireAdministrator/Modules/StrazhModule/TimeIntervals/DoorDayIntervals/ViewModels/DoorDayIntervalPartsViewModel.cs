using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using StrazhModule.Intervals.Base.ViewModels;
using Infrastructure.Common.Windows.ViewModels;

namespace StrazhModule.ViewModels
{
	public class DoorDayIntervalPartsViewModel : BaseViewModel
	{
		public SKDDoorDayInterval DayInterval { get; set; }

		public DoorDayIntervalPartsViewModel(SKDDoorDayInterval dayInterval)
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			RemoveCommand = new RelayCommand(OnRemove, CanEdit);

			DayInterval = dayInterval;
			Parts = new ObservableCollection<DoorDayIntervalPartViewModel>();
			foreach (var dayIntervalPart in dayInterval.DayIntervalParts)
			{
				var dayIntervalPartViewModel = new DoorDayIntervalPartViewModel(dayIntervalPart);
				Parts.Add(dayIntervalPartViewModel);
			}
		}

		public ObservableCollection<DoorDayIntervalPartViewModel> Parts { get; protected set; }

		private DoorDayIntervalPartViewModel _selectedPart;
		public DoorDayIntervalPartViewModel SelectedPart
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
			var dayIntervalPartDetailsViewModel = new DoorDayIntervalPartDetailsViewModel();
			if (DialogService.ShowModalWindow(dayIntervalPartDetailsViewModel))
			{
				DayInterval.DayIntervalParts.Add(dayIntervalPartDetailsViewModel.DayIntervalPart);
				var dayIntervalPartViewModel = new DoorDayIntervalPartViewModel(dayIntervalPartDetailsViewModel.DayIntervalPart);
				Parts.Add(dayIntervalPartViewModel);
				SelectedPart = dayIntervalPartViewModel;
				SelectedPart.Update();
				ServiceFactory.SaveService.SKDChanged = true;
				ServiceFactory.SaveService.TimeIntervalChanged();
			}
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
			var dayIntervalPartDetailsViewModel = new DoorDayIntervalPartDetailsViewModel(SelectedPart.DayIntervalPart);
			if (DialogService.ShowModalWindow(dayIntervalPartDetailsViewModel))
			{
				SelectedPart.Update();
				ServiceFactory.SaveService.SKDChanged = true;
				ServiceFactory.SaveService.TimeIntervalChanged();
			}
		}
		bool CanEdit()
		{
			return SelectedPart != null;
		}
	}
}