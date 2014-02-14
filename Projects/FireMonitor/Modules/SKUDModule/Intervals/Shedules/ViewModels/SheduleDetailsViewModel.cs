using System;
using System.Linq;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;
using System.Collections.ObjectModel;

namespace SKDModule.ViewModels
{
	public class SheduleDetailsViewModel : SaveCancelDialogViewModel
	{
		ShedulesViewModel ShedulesViewModel;
		bool IsNew;
		public EmployeeShedule Shedule { get; private set; }

		public SheduleDetailsViewModel(ShedulesViewModel shedulesViewModel, EmployeeShedule shedule = null)
		{
			ShedulesViewModel = shedulesViewModel;
			if (shedule == null)
			{
				Title = "Новый график";
				IsNew = true;
				shedule = new EmployeeShedule()
				{
					Name = "Новый график работы"
				};
			}
			else
			{
				Title = "Редактирование графика работы";
				IsNew = false;
			}
			Shedule = shedule;
			Name = Shedule.Name;
			IsIgnoreHoliday = Shedule.IsIgnoreHoliday;
			IsOnlyFirstEnter = Shedule.IsOnlyFirstEnter;
			AvailableSheduleTypes = new ObservableCollection<EmployeeSheduleType>(Enum.GetValues(typeof(EmployeeSheduleType)).OfType<EmployeeSheduleType>());
			SelectedSheduleType = Shedule.EmployeeSheduleType;
			SelectedSheduleSceme = AvailableSheduleScemes.FirstOrDefault(x => x.UID == Shedule.SheduleUID);
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged("Name");
			}
		}

		bool _isIgnoreHoliday;
		public bool IsIgnoreHoliday
		{
			get { return _isIgnoreHoliday; }
			set
			{
				_isIgnoreHoliday = value;
				OnPropertyChanged("IsIgnoreHoliday");
			}
		}

		bool _isOnlyFirstEnter;
		public bool IsOnlyFirstEnter
		{
			get { return _isOnlyFirstEnter; }
			set
			{
				_isOnlyFirstEnter = value;
				OnPropertyChanged("IsOnlyFirstEnter");
			}
		}

		ObservableCollection<EmployeeSheduleType> _availableSheduleTypes;
		public ObservableCollection<EmployeeSheduleType> AvailableSheduleTypes
		{
			get { return _availableSheduleTypes; }
			set
			{
				_availableSheduleTypes = value;
				OnPropertyChanged("AvailableSheduleTypes");
			}
		}

		EmployeeSheduleType _selectedSheduleType;
		public EmployeeSheduleType SelectedSheduleType
		{
			get { return _selectedSheduleType; }
			set
			{
				_selectedSheduleType = value;
				OnPropertyChanged("SelectedSheduleType");

				AvailableSheduleScemes = new ObservableCollection<SheduleScemeViewModel>();
				SelectedSheduleSceme = AvailableSheduleScemes.FirstOrDefault();
			}
		}

		ObservableCollection<SheduleScemeViewModel> _availableSheduleScemes;
		public ObservableCollection<SheduleScemeViewModel> AvailableSheduleScemes
		{
			get { return _availableSheduleScemes; }
			set
			{
				_availableSheduleScemes = value;
				OnPropertyChanged("AvailableSheduleScemes");
			}
		}

		SheduleScemeViewModel _selectedSheduleSceme;
		public SheduleScemeViewModel SelectedSheduleSceme
		{
			get { return _selectedSheduleSceme; }
			set
			{
				_selectedSheduleSceme = value;
				OnPropertyChanged("SelectedSheduleSceme");
			}
		}

		protected override bool CanSave()
		{
			return !string.IsNullOrEmpty(Name) && Name != "Доступ запрещен";
		}

		protected override bool Save()
		{
			if (ShedulesViewModel.Shedules.Any(x => x.Shedule.Name == Name && x.Shedule.UID != Shedule.UID))
			{
				MessageBoxService.ShowWarning("Название графика совпадает с введенным ранее");
				return false;
			}

			Shedule.Name = Name;
			Shedule.IsIgnoreHoliday = IsIgnoreHoliday;
			Shedule.IsOnlyFirstEnter = IsOnlyFirstEnter;
			Shedule.EmployeeSheduleType = SelectedSheduleType;
			if (SelectedSheduleSceme != null)
				Shedule.SheduleUID = SelectedSheduleSceme.UID;
			return true;
		}
	}

	public class SheduleScemeViewModel : BaseViewModel
	{
		public SheduleScemeViewModel(Guid uid, string name)
		{
			UID = uid;
			Name = name;
		}

		public Guid UID { get; private set; }
		public string Name { get; private set; }
	}
}