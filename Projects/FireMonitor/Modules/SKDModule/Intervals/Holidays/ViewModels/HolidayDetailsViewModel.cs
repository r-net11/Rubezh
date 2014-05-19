using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Organisation = FiresecAPI.SKD.Organisation;

namespace SKDModule.ViewModels
{
	public class HolidayDetailsViewModel : SaveCancelDialogViewModel
	{
		private Organisation Organisation;
		public Holiday Holiday { get; private set; }

		public HolidayDetailsViewModel(Organisation organisation, int year, Holiday holiday = null)
		{
			Organisation = organisation;
			if (holiday == null)
			{
				Title = "Новый приаздничный день";
				holiday = new Holiday()
				{
					Name = "Название праздника",
					OrganisationUID = Organisation.UID,
					Date = new DateTime(year, DateTime.Today.Month, DateTime.Today.Day),
				};
			}
			else
			{
				Title = "Редактирование праздничного дня";
			}
			Holiday = holiday;
			Name = holiday.Name;
			Date = holiday.Date;
			Reduction = holiday.Reduction;
			TransferDate = holiday.TransferDate.HasValue ? holiday.TransferDate.Value : holiday.Date;

			AvailableHolidayTypes = new ObservableCollection<HolidayType>(Enum.GetValues(typeof(HolidayType)).OfType<HolidayType>());
			HolidayType = holiday.Type;
		}

		private DateTime _date;
		public DateTime Date
		{
			get { return _date; }
			set
			{
				_date = value;
				OnPropertyChanged(() => Date);
			}
		}

		private string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		public ObservableCollection<HolidayType> AvailableHolidayTypes { get; private set; }

		private HolidayType _holidayType;
		public HolidayType HolidayType
		{
			get { return _holidayType; }
			set
			{
				_holidayType = value;
				OnPropertyChanged(() => HolidayType);
				OnPropertyChanged(() => IsReductionEnabled);
				OnPropertyChanged(() => IsTransferDateEnabled);
			}
		}

		private TimeSpan _reduction;
		public TimeSpan Reduction
		{
			get { return _reduction; }
			set
			{
				_reduction = value;
				OnPropertyChanged(() => Reduction);
			}
		}

		private DateTime _transferDate;
		public DateTime TransferDate
		{
			get { return _transferDate; }
			set
			{
				_transferDate = value;
				OnPropertyChanged(() => TransferDate);
			}
		}

		public bool IsReductionEnabled
		{
			get { return HolidayType != HolidayType.Holiday; }
		}
		public bool IsTransferDateEnabled
		{
			get { return HolidayType == HolidayType.WorkingHoliday; }
		}

		protected override bool Save()
		{
			if (Reduction.TotalHours > 2)
			{
				MessageBoxService.ShowWarning("Величина сокращения не может быть больше двух часов");
				return false;
			}
			if (HolidayType == HolidayType.WorkingHoliday && Date.DayOfWeek != DayOfWeek.Saturday && Date.DayOfWeek != DayOfWeek.Sunday)
			{
				MessageBoxService.ShowWarning("Дата переноса устанавливается только на субботу или воскресенье");
				return false;
			}
			Holiday.Name = Name;
			Holiday.Date = Date;
			Holiday.Type = HolidayType;
			Holiday.Reduction = Reduction;
			Holiday.TransferDate = IsTransferDateEnabled ? (DateTime?)TransferDate : null;
			return HolidayHelper.Save(Holiday);
		}
	}
}