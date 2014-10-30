using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System;

namespace GKModule.ViewModels
{
	public class HolidayDetailsViewModel : SaveCancelDialogViewModel
	{
		public GKHoliday Holiday { get; private set; }

		public HolidayDetailsViewModel(GKHoliday holiday = null)
		{
			if (holiday == null)
			{
				Title = "Новый праздничный день";
				holiday = new GKHoliday()
				{
					Name = "Название праздника",
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

			AvailableHolidayTypes = new ObservableCollection<GKHolidayType>(Enum.GetValues(typeof(GKHolidayType)).OfType<GKHolidayType>());
			HolidayType = holiday.HolidayType;
		}

		DateTime _date;
		public DateTime Date
		{
			get { return _date; }
			set
			{
				_date = value;
				OnPropertyChanged(() => Date);
			}
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		public ObservableCollection<GKHolidayType> AvailableHolidayTypes { get; private set; }

		GKHolidayType _holidayType;
		public GKHolidayType HolidayType
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

		int _reduction;
		public int Reduction
		{
			get { return _reduction; }
			set
			{
				_reduction = value;
				OnPropertyChanged(() => Reduction);
			}
		}

		DateTime _transferDate;
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
			get { return HolidayType == GKHolidayType.BeforeHoliday; }
		}
		public bool IsTransferDateEnabled
		{
			get { return HolidayType == GKHolidayType.WorkingHoliday; }
		}

		protected override bool Save()
		{
			if (Reduction > 2)
			{
				MessageBoxService.ShowWarning("Величина сокращения не может быть больше двух часов");
				return false;
			}
			Holiday.Name = Name;
			Holiday.Date = Date;
			Holiday.HolidayType = HolidayType;
			Holiday.Reduction = Reduction;
			Holiday.TransferDate = IsTransferDateEnabled ? (DateTime?)TransferDate : null;
			return true;
		}
	}
}