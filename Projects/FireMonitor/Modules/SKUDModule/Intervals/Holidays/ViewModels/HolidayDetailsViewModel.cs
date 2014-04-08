using System;
using System.Linq;
using System.Collections.ObjectModel;
using FiresecAPI.EmployeeTimeIntervals;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.Windows;
using Organization = FiresecAPI.Organization;

namespace SKDModule.ViewModels
{
	public class HolidayDetailsViewModel : SaveCancelDialogViewModel
	{
		OrganisationHolidaysYearViewModel OrganisationHolidaysYearViewModel;
		bool IsNew;
		public Holiday Holiday { get; private set; }

		public HolidayDetailsViewModel(OrganisationHolidaysYearViewModel organisationHolidaysYearViewModel, Holiday holiday = null)
		{
			OrganisationHolidaysYearViewModel = organisationHolidaysYearViewModel;
			if (holiday == null)
			{
				Title = "Новый приаздничный день";
				IsNew = true;
				holiday = new Holiday()
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
			DateTime = holiday.Date;
			ShortageTime = holiday.Reduction;
			TransitionDateTime = holiday.TransferDate;

			AvailableHolidayTypes = new ObservableCollection<HolidayType>(Enum.GetValues(typeof(HolidayType)).OfType<HolidayType>());
			HolidayType = holiday.Type;
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

		public ObservableCollection<HolidayType> AvailableHolidayTypes { get; private set; }

		HolidayType _holidayType;
		public HolidayType HolidayType
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

		TimeSpan _shortageTime;
		public TimeSpan ShortageTime
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
			get { return HolidayType != HolidayType.Holiday; }
		}

		public bool IsTransitionDateTimeEnabled
		{
			get { return HolidayType == HolidayType.WorkingHoliday; }
		}

		protected override bool Save()
		{
			if (OrganisationHolidaysYearViewModel.Holidays.Any(x => x.Holiday.Date.Month == DateTime.Month && x.Holiday.Date.Date == DateTime.Date && x.Holiday.UID != Holiday.UID))
			{
				MessageBoxService.ShowWarning("Дата праздника совпадает с введенным ранее");
				return false;
			}

			if (ShortageTime.TotalHours > 2)
			{
				MessageBoxService.ShowWarning("Величина сокращения не может быть больше двух часов");
				return false;
			}

			if (HolidayType == HolidayType.WorkingHoliday && DateTime.DayOfWeek != DayOfWeek.Saturday && DateTime.DayOfWeek != DayOfWeek.Sunday)
			{
				MessageBoxService.ShowWarning("Дата переноса устанавливается только на субботу или воскресенье");
				return false;
			}

			Holiday.Name = Name;
			Holiday.Date = DateTime;
			Holiday.Type = HolidayType;
			Holiday.Reduction = ShortageTime;
			Holiday.TransferDate = TransitionDateTime;
			return true;
		}
	}
}