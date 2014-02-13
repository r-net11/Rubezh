using System;
using System.Linq;
using System.Collections.ObjectModel;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;

namespace SKDModule.ViewModels
{
	public class HolidayDetailsViewModel : SaveCancelDialogViewModel
	{
		HolidayYearViewModel HolidayYearViewModel;
		bool IsNew;
		public EmployeeHoliday Holiday { get; private set; }

		public HolidayDetailsViewModel(HolidayYearViewModel holidayYearViewModel, EmployeeHoliday holiday = null)
		{
			HolidayYearViewModel = holidayYearViewModel;
			if (holiday == null)
			{
				Title = "Новый приаздничный день";
				IsNew = true;
				holiday = new EmployeeHoliday()
				{
					Name = "Название праздника",
				};
			}
			else
			{
				Title = "Редактирование праздничного дня";
				IsNew = false;
			}

			Holiday = holiday;
			Name = holiday.Name;
			DateTime = holiday.DateTime;
			ShortageTime = holiday.ShortageTime;
			TransitionDateTime = holiday.TransitionDateTime;

			AvailableHolidayTypes = new ObservableCollection<EmployeeHolidayType>(Enum.GetValues(typeof(EmployeeHolidayType)).OfType<EmployeeHolidayType>());
			HolidayType = holiday.EmployeeHolidayType;
		}

		DateTime _dateTime;
		public DateTime DateTime
		{
			get { return _dateTime; }
			set
			{
				_dateTime = value;
				OnPropertyChanged("DateTime");
			}
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

		public ObservableCollection<EmployeeHolidayType> AvailableHolidayTypes { get; private set; }

		EmployeeHolidayType _holidayType;
		public EmployeeHolidayType HolidayType
		{
			get { return _holidayType; }
			set
			{
				_holidayType = value;
				OnPropertyChanged("HolidayType");
				OnPropertyChanged("IsShortageTimeEnabled");
				OnPropertyChanged("IsTransitionDateTimeEnabled");
			}
		}

		DateTime _shortageTime;
		public DateTime ShortageTime
		{
			get { return _shortageTime; }
			set
			{
				_shortageTime = value;
				OnPropertyChanged("ShortageTime");
			}
		}

		DateTime _transitionDateTime;
		public DateTime TransitionDateTime
		{
			get { return _transitionDateTime; }
			set
			{
				_transitionDateTime = value;
				OnPropertyChanged("TransitionDateTime");
			}
		}

		public bool IsShortageTimeEnabled
		{
			get { return HolidayType != EmployeeHolidayType.Holiday; }
		}

		public bool IsTransitionDateTimeEnabled
		{
			get { return HolidayType == EmployeeHolidayType.WorkingHoliday; }
		}

		protected override bool Save()
		{
			if (HolidayYearViewModel.Holidays.Any(x => x.Holiday.DateTime.Month == DateTime.Month && x.Holiday.DateTime.Date == DateTime.Date && x.Holiday.UID != Holiday.UID))
			{
				MessageBoxService.ShowWarning("Дата праздника совпадает с введенным ранее");
				return false;
			}

			if (ShortageTime.Hour > 2)
			{
				MessageBoxService.ShowWarning("Величина сокращения не может быть больше двух часов");
				return false;
			}

			if (HolidayType == EmployeeHolidayType.WorkingHoliday && DateTime.DayOfWeek != DayOfWeek.Saturday && DateTime.DayOfWeek != DayOfWeek.Sunday)
			{
				MessageBoxService.ShowWarning("Дата переноса устанавливается только на субботу или воскресенье");
				return false;
			}

			Holiday.Name = Name;
			Holiday.DateTime = DateTime;
			Holiday.EmployeeHolidayType = HolidayType;
			Holiday.ShortageTime = ShortageTime;
			Holiday.TransitionDateTime = TransitionDateTime;
			return true;
		}
	}
}