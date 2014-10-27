using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class DaySchedulePartsViewModel : BaseViewModel
	{
		public GKDaySchedule DaySchedule { get; set; }

		public DaySchedulePartsViewModel(GKDaySchedule daySchedule)
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			RemoveCommand = new RelayCommand(OnRemove, CanEdit);

			DaySchedule = daySchedule;
			Parts = new ObservableCollection<DaySchedulePartViewModel>();
			foreach (var dayIntervalPart in daySchedule.DayScheduleParts)
			{
				var daySchedulePartViewModel = new DaySchedulePartViewModel(dayIntervalPart);
				Parts.Add(daySchedulePartViewModel);
			}
		}

		public ObservableCollection<DaySchedulePartViewModel> Parts { get; protected set; }

		private DaySchedulePartViewModel _selectedPart;
		public DaySchedulePartViewModel SelectedPart
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
			var daySchedulePartDetailsViewModel = new DaySchedulePartDetailsViewModel();
			if (DialogService.ShowModalWindow(daySchedulePartDetailsViewModel))
			{
				DaySchedule.DayScheduleParts.Add(daySchedulePartDetailsViewModel.DaySchedulePart);
				var daySchedulePartViewModel = new DaySchedulePartViewModel(daySchedulePartDetailsViewModel.DaySchedulePart);
				Parts.Add(daySchedulePartViewModel);
				SelectedPart = daySchedulePartViewModel;
				SelectedPart.Update();
				ServiceFactory.SaveService.GKChanged = true;
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
			DaySchedule.DayScheduleParts.Remove(SelectedPart.DaySchedulePart);
			Parts.Remove(SelectedPart);
			ServiceFactory.SaveService.GKChanged = true;
			ServiceFactory.SaveService.TimeIntervalChanged();
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var daySchedulePartDetailsViewModel = new DaySchedulePartDetailsViewModel(SelectedPart.DaySchedulePart);
			if (DialogService.ShowModalWindow(daySchedulePartDetailsViewModel))
			{
				SelectedPart.Update();
				ServiceFactory.SaveService.GKChanged = true;
				ServiceFactory.SaveService.TimeIntervalChanged();
			}
		}
		bool CanEdit()
		{
			return SelectedPart != null;
		}
	}
}