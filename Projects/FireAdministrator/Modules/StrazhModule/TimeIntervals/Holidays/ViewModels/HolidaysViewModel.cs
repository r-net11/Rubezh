using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using StrazhAPI.SKD;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using KeyboardKey = System.Windows.Input.Key;

namespace StrazhModule.ViewModels
{
	public class HolidaysViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public HolidaysViewModel()
		{
			Menu = new HolidaysMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			RegisterShortcuts();
			SetRibbonItems();
		}

		public void Initialize()
		{
			Holidays = new ObservableCollection<HolidayViewModel>();
			foreach (var holiday in SKDManager.TimeIntervalsConfiguration.Holidays)
			{
				var holidayViewModel = new HolidayViewModel(holiday);
				Holidays.Add(holidayViewModel);
			}
			SelectedHoliday = Holidays.FirstOrDefault();
		}

		ObservableCollection<HolidayViewModel> _holidays;
		public ObservableCollection<HolidayViewModel> Holidays
		{
			get { return _holidays; }
			set
			{
				_holidays = value;
				OnPropertyChanged(() => Holidays);
			}
		}

		HolidayViewModel _selectedHoliday;
		public HolidayViewModel SelectedHoliday
		{
			get { return _selectedHoliday; }
			set
			{
				_selectedHoliday = value;
				OnPropertyChanged(() => SelectedHoliday);
			}
		}

		public void Select(Guid holidayUID)
		{
			if (holidayUID != Guid.Empty)
			{
				var holidayViewModel = Holidays.FirstOrDefault(x => x.Holiday.UID == holidayUID);
				if (holidayViewModel != null)
				{
					SelectedHoliday = holidayViewModel;
				}
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var holidayDetailsViewModel = new HolidayDetailsViewModel();
			if (DialogService.ShowModalWindow(holidayDetailsViewModel))
			{
				SKDManager.TimeIntervalsConfiguration.Holidays.Add(holidayDetailsViewModel.Holiday);
				var holidayViewModel = new HolidayViewModel(holidayDetailsViewModel.Holiday);
				Holidays.Add(holidayViewModel);
				SelectedHoliday = holidayViewModel;
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
		bool CanAdd()
		{
			return Holidays.Count < 100;
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			SKDManager.TimeIntervalsConfiguration.Holidays.Remove(SelectedHoliday.Holiday);
			Holidays.Remove(SelectedHoliday);
			ServiceFactory.SaveService.SKDChanged = true;
		}
		bool CanDelete()
		{
			return SelectedHoliday != null;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var holidayDetailsViewModel = new HolidayDetailsViewModel(SelectedHoliday.Holiday);
			if (DialogService.ShowModalWindow(holidayDetailsViewModel))
			{
				SelectedHoliday.Update();
				ServiceFactory.SaveService.SKDChanged = true;
			}
		}
		bool CanEdit()
		{
			return SelectedHoliday != null;
		}

		void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
		}

		void SetRibbonItems()
		{
			RibbonItems = new List<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel("Редактирование", new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel("Добавить", "BAdd"),
					new RibbonMenuItemViewModel("Редактировать", "BEdit"),
					new RibbonMenuItemViewModel("Удалить", "BDelete"),
				}, "BEdit") { Order = 1 }
			};
		}
	}
}