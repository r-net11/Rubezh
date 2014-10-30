using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Ribbon;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using KeyboardKey = System.Windows.Input.Key;
using FiresecAPI.GK;

namespace GKModule.ViewModels
{
	public class HolidaysViewModel : MenuViewPartViewModel, IEditingViewModel, ISelectable<Guid>
	{
		public HolidaysViewModel()
		{
			Menu = new HolidaysMenuViewModel(this);
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanEditDelete);
			EditCommand = new RelayCommand(OnEdit, CanEditDelete);
			RegisterShortcuts();
			SetRibbonItems();
		}

		public void Initialize()
		{
			Holidays = new ObservableCollection<HolidayViewModel>();
			foreach (var holiday in GKManager.DeviceConfiguration.Holidays.OrderBy(x => x.No))
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
				if (value != null)
				{
					value.Update();
				}
				OnPropertyChanged(() => SelectedHoliday);
				OnPropertyChanged(() => HasSelectedHoliday);
			}
		}

		public bool HasSelectedHoliday
		{
			get { return SelectedHoliday != null; }
		}

		bool CanEditDelete()
		{
			return SelectedHoliday != null;
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var holidayDetailsViewModel = new HolidayDetailsViewModel();
			if (DialogService.ShowModalWindow(holidayDetailsViewModel))
			{
				var holiday = holidayDetailsViewModel.Holiday;
				GKManager.DeviceConfiguration.Holidays.Add(holiday);
				var holidayViewModel = new HolidayViewModel(holiday);
				Holidays.Add(holidayViewModel);
				SelectedHoliday = holidayViewModel;
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить график работ " + SelectedHoliday.Holiday.PresentationName))
			{
				var index = Holidays.IndexOf(SelectedHoliday);
				GKManager.DeviceConfiguration.Holidays.Remove(SelectedHoliday.Holiday);
				SelectedHoliday.Holiday.OnChanged();
				Holidays.Remove(SelectedHoliday);
				index = Math.Min(index, Holidays.Count - 1);
				if (index > -1)
					SelectedHoliday = Holidays[index];
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var holidayDetailsViewModel = new HolidayDetailsViewModel(SelectedHoliday.Holiday);
			if (DialogService.ShowModalWindow(holidayDetailsViewModel))
			{
				SelectedHoliday.Update(holidayDetailsViewModel.Holiday);
				holidayDetailsViewModel.Holiday.OnChanged();
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public override void OnShow()
		{
			base.OnShow();
			SelectedHoliday = SelectedHoliday;
		}

		#region ISelectable<Guid> Members

		public void Select(Guid holidayUID)
		{
			if (holidayUID != Guid.Empty)
				SelectedHoliday = Holidays.FirstOrDefault(x => x.Holiday.UID == holidayUID);
		}

		#endregion

		void RegisterShortcuts()
		{
			RegisterShortcut(new KeyGesture(KeyboardKey.N, ModifierKeys.Control), AddCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.Delete, ModifierKeys.Control), DeleteCommand);
			RegisterShortcut(new KeyGesture(KeyboardKey.E, ModifierKeys.Control), EditCommand);
		}

		void SetRibbonItems()
		{
			RibbonItems = new List<RibbonMenuItemViewModel>()
			{
					new RibbonMenuItemViewModel("Редактирование", new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel("Добавить", AddCommand, "/Controls;component/Images/BAdd.png"),
					new RibbonMenuItemViewModel("Редактировать", EditCommand, "/Controls;component/Images/BEdit.png"),
					new RibbonMenuItemViewModel("Удалить", DeleteCommand, "/Controls;component/Images/BDelete.png"),
				}, "/Controls;component/Images/BEdit.png") { Order = 2 }
			};
		}
	}
}