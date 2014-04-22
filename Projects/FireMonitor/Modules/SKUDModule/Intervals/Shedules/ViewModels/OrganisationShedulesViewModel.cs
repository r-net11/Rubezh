using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Organisation = FiresecAPI.Organisation;

namespace SKDModule.ViewModels
{
	public class OrganisationShedulesViewModel : ViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public Organisation Organisation { get; private set; }
		Schedule SheduleToCopy;

		public OrganisationShedulesViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			CopyCommand = new RelayCommand(OnCopy, CanCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
		}

		public void Initialize(Organisation organisation, List<Schedule> employeeShedules)
		{
			Organisation = organisation;

			Shedules = new ObservableCollection<SheduleViewModel>();
			foreach (var employeeShedule in employeeShedules)
			{
				var sheduleViewModel = new SheduleViewModel(employeeShedule);
				Shedules.Add(sheduleViewModel);
			}
			SelectedShedule = Shedules.FirstOrDefault();
		}

		ObservableCollection<SheduleViewModel> _shedule;
		public ObservableCollection<SheduleViewModel> Shedules
		{
			get { return _shedule; }
			set
			{
				_shedule = value;
				OnPropertyChanged("Shedules");
			}
		}

		SheduleViewModel _selectedShedule;
		public SheduleViewModel SelectedShedule
		{
			get { return _selectedShedule; }
			set
			{
				_selectedShedule = value;
				OnPropertyChanged("SelectedShedule");
			}
		}

		public void Select(Guid intervalUID)
		{
			if (intervalUID != Guid.Empty)
			{
				var intervalViewModel = Shedules.FirstOrDefault(x => x.Shedule.UID == intervalUID);
				if (intervalViewModel != null)
				{
					SelectedShedule = intervalViewModel;
				}
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var sheduleDetailsViewModel = new SheduleDetailsViewModel(this);
			if (DialogService.ShowModalWindow(sheduleDetailsViewModel))
			{
				var sheduleViewModel = new SheduleViewModel(sheduleDetailsViewModel.Shedule);
				Shedules.Add(sheduleViewModel);
				SelectedShedule = sheduleViewModel;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			Shedules.Remove(SelectedShedule);
		}
		bool CanDelete()
		{
			return SelectedShedule != null;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var sheduleDetailsViewModel = new SheduleDetailsViewModel(this, SelectedShedule.Shedule);
			if (DialogService.ShowModalWindow(sheduleDetailsViewModel))
			{
				SelectedShedule.Update();
			}
		}
		bool CanEdit()
		{
			return SelectedShedule != null;
		}

		public RelayCommand CopyCommand { get; private set; }
		void OnCopy()
		{
			SheduleToCopy = CopyShedule(SelectedShedule.Shedule);
		}
		bool CanCopy()
		{
			return SelectedShedule != null;
		}

		public RelayCommand PasteCommand { get; private set; }
		void OnPaste()
		{
			var newShedule = CopyShedule(SheduleToCopy);
			var sheduleViewModel = new SheduleViewModel(newShedule);
			Shedules.Add(sheduleViewModel);
			SelectedShedule = sheduleViewModel;
		}
		bool CanPaste()
		{
			return SheduleToCopy != null;
		}

		Schedule CopyShedule(Schedule source)
		{
			var copy = new Schedule();
			copy.Name = source.Name;
			foreach (var employeeShedulePart in source.Zones)
			{
				var copyEmployeeShedulePart = new ScheduleZone()
				{
				};
				copy.Zones.Add(copyEmployeeShedulePart);
			}
			return copy;
		}
	}
}