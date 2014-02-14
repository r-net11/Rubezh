using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FiresecAPI;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using KeyboardKey = System.Windows.Input.Key;

namespace SKDModule.ViewModels
{
	public class ShedulesViewModel : ViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		EmployeeShedule SheduleToCopy;

		public ShedulesViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			CopyCommand = new RelayCommand(OnCopy, CanCopy);
			PasteCommand = new RelayCommand(OnPaste, CanPaste);
			RefreshCommand = new RelayCommand(OnRefresh);
			Initialize();
		}

		public void Initialize()
		{
			var employeeShedules = new List<EmployeeShedule>();
			var neverShedule = employeeShedules.FirstOrDefault(x => x.Name == "Никогда" && x.IsDefault);
			if (neverShedule == null)
			{
				neverShedule = new EmployeeShedule() { Name = "Никогда", IsDefault = true };
				employeeShedules.Add(neverShedule);
			}

			Shedules = new ObservableCollection<SheduleViewModel>();
			foreach (var employeeShedule in employeeShedules)
			{
				var sheduleViewModel = new SheduleViewModel(employeeShedule);
				Shedules.Add(sheduleViewModel);
			}
			SelectedShedule = Shedules.FirstOrDefault();
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			Initialize();
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
			return SelectedShedule != null && !SelectedShedule.Shedule.IsDefault;
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
			return SelectedShedule != null && !SelectedShedule.Shedule.IsDefault;
		}

		public RelayCommand CopyCommand { get; private set; }
		void OnCopy()
		{
			SheduleToCopy = CopyShedule(SelectedShedule.Shedule);
		}
		bool CanCopy()
		{
			return SelectedShedule != null && !SelectedShedule.Shedule.IsDefault;
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

		EmployeeShedule CopyShedule(EmployeeShedule source)
		{
			var copy = new EmployeeShedule();
			copy.Name = source.Name;
			foreach (var employeeShedulePart in source.EmployeeSheduleParts)
			{
				var copyEmployeeShedulePart = new EmployeeShedulePart()
				{
				};
				copy.EmployeeSheduleParts.Add(copyEmployeeShedulePart);
			}
			return copy;
		}
	}
}