using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Organisation = FiresecAPI.SKD.Organisation;

namespace SKDModule.ViewModels
{
    public class HolidayDetailsViewModel : SaveCancelDialogViewModel, IDetailsViewModel<Holiday>
	{
		Organisation Organisation;
		public Holiday Model { get; private set; }

		public HolidayDetailsViewModel() { }

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

		TimeSpan _reduction;
		public TimeSpan Reduction
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
			get { return HolidayType == HolidayType.BeforeHoliday; }
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
			Model.Name = Name;
			Model.Date = Date;
			Model.Type = HolidayType;
			Model.Reduction = Reduction;
			Model.TransferDate = IsTransferDateEnabled ? (DateTime?)TransferDate : null;
			if (!DetailsValidateHelper.Validate(Model))
				return false;
			return HolidayHelper.Save(Model);
		}


        public bool Initialize(Organisation organisation, Holiday holiday, ViewPartViewModel parentViewModel)
        {
            Organisation = organisation;
            var holidaysViewModel = parentViewModel as HolidaysViewModel;
            if (holiday == null)
            {
                Title = "Новый приаздничный день";
                holiday = new Holiday()
                {
                    Name = "Название праздника",
                    OrganisationUID = Organisation.UID,
                    Date = new DateTime(holidaysViewModel.SelectedYear, DateTime.Today.Month, DateTime.Today.Day),
                };
            }
            else
            {
                Title = "Редактирование праздничного дня";
            }
            Model = holiday;
            Name = holiday.Name;
            Date = holiday.Date;
            Reduction = holiday.Reduction;
            TransferDate = holiday.TransferDate.HasValue ? holiday.TransferDate.Value : holiday.Date;

            AvailableHolidayTypes = new ObservableCollection<HolidayType>(Enum.GetValues(typeof(HolidayType)).OfType<HolidayType>());
            HolidayType = holiday.Type;
			return true;
        }
    }
}