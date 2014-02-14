using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;

namespace SKDModule.ViewModels
{
	public class SheduleViewModel : BaseViewModel
	{
		public EmployeeShedule Shedule { get; private set; }

		public SheduleViewModel(EmployeeShedule shedule)
		{
			Shedule = shedule;
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			SheduleParts = new ObservableCollection<ShedulePartViewModel>();
			foreach (var employeeShedulePart in shedule.EmployeeSheduleParts)
			{
				var shedulePartViewModel = new ShedulePartViewModel(this, employeeShedulePart);
				SheduleParts.Add(shedulePartViewModel);
			}
		}

		public ObservableCollection<ShedulePartViewModel> SheduleParts { get; private set; }

		ShedulePartViewModel _selectedShedulePart;
		public ShedulePartViewModel SelectedShedulePart
		{
			get { return _selectedShedulePart; }
			set
			{
				_selectedShedulePart = value;
				OnPropertyChanged("SelectedShedulePart");
			}
		}

		public void Update()
		{
			OnPropertyChanged("Shedule");

			Shedule.EmployeeSheduleParts = new List<EmployeeShedulePart>();
			foreach (var shedulePart in SheduleParts)
			{
				Shedule.EmployeeSheduleParts.Add(shedulePart.ShedulePart);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var shedulePartDetailsViewModel = new ShedulePartDetailsViewModel();
			if (DialogService.ShowModalWindow(shedulePartDetailsViewModel))
			{
				var shedulePart = shedulePartDetailsViewModel.ShedulePart;
				Shedule.EmployeeSheduleParts.Add(shedulePart);
				var shedulePartViewModel = new ShedulePartViewModel(this, shedulePart);
				SheduleParts.Add(shedulePartViewModel);
			}
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			Shedule.EmployeeSheduleParts.Remove(SelectedShedulePart.ShedulePart);
			SheduleParts.Remove(SelectedShedulePart);
		}
		bool CanRemove()
		{
			return SelectedShedulePart != null && SheduleParts.Count > 1;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var shedulePartDetailsViewModel = new ShedulePartDetailsViewModel(SelectedShedulePart.ShedulePart);
			if (DialogService.ShowModalWindow(shedulePartDetailsViewModel))
			{
				SelectedShedulePart.ShedulePart = SelectedShedulePart.ShedulePart;
				SelectedShedulePart.Update();
			}
		}
		bool CanEdit()
		{
			return SelectedShedulePart != null && !Shedule.IsDefault;
		}

		public bool IsEnabled
		{
			get { return !Shedule.IsDefault; }
		}
	}
}