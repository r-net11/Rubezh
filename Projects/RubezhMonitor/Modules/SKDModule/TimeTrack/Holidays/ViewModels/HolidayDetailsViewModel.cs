using System;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.SKD;
using RubezhClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;
using Organisation = RubezhAPI.SKD.Organisation;

namespace SKDModule.ViewModels
{
	public class HolidayDetailsViewModel : SaveCancelDialogViewModel, IDetailsViewModel<Holiday>
	{
		Organisation Organisation;
		bool _isNew;
		public Holiday Model { get; private set; }

		public HolidayDetailsViewModel() { }

		public bool Initialize(Organisation organisation, Holiday holiday, ViewPartViewModel parentViewModel)
		{
			Organisation = organisation;
			var holidaysViewModel = parentViewModel as HolidaysViewModel;
			_isNew = holiday == null;
			if (_isNew)
			{
				Title = "Новый праздничный день";
				holiday = new Holiday()
				{
					Name = "Название праздничного дня",
					OrganisationUID = Organisation.UID,
					Date = new DateTime(holidaysViewModel.SelectedYear, DateTime.Today.Month, DateTime.Today.Day),
				};
				Model = holiday;
			}
			else
			{
				Title = "Редактирование праздничного дня";
				Model = HolidayHelper.GetSingle(holiday.UID);
			}
			
			Name = holiday.Name;
			Date = holiday.Date;

			IsOneHourReduction = holiday.Reduction.Hours == 1;
			TransferDate = holiday.TransferDate.HasValue ? holiday.TransferDate.Value : holiday.Date;

			AvailableHolidayTypes = new ObservableCollection<HolidayType>(Enum.GetValues(typeof(HolidayType)).OfType<HolidayType>());
			HolidayType = holiday.Type;
			return true;
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

		public ObservableCollection<HolidayType> AvailableHolidayTypes { get; private set; }

		HolidayType _holidayType;
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

		bool _IsOneHourReduction;
		public bool IsOneHourReduction
		{
			get { return _IsOneHourReduction; }
			set
			{
				_IsOneHourReduction = value;
				OnPropertyChanged(() => IsOneHourReduction);
			}
		}

		public bool IsReductionEnabled
		{
			get { return HolidayType == HolidayType.BeforeHoliday; }
		}
		public bool IsTransferDateEnabled
		{
			get { return HolidayType == HolidayType.WorkingHoliday; }
		}

		protected override bool Save()
		{
			Model.Name = Name;
			Model.Date = Date;
			Model.Type = HolidayType;
			if (HolidayType == HolidayType.Holiday)
				Model.Reduction = new TimeSpan(0, 0, 0);
			else
				Model.Reduction = IsOneHourReduction ? new TimeSpan(1, 0, 0) : new TimeSpan(2, 0, 0);
			Model.TransferDate = IsTransferDateEnabled ? (DateTime?)TransferDate : null;
			if (!DetailsValidateHelper.Validate(Model))
				return false;
			return HolidayHelper.Save(Model, _isNew);
		}
	}
}