using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using StrazhAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;
using Organisation = StrazhAPI.SKD.Organisation;

namespace SKDModule.ViewModels
{
	public class HolidayDetailsViewModel : SaveCancelDialogViewModel, IDetailsViewModel<Holiday>, IDataErrorInfo
	{
		private Organisation Organisation;
		private bool _isNew;
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

			TimeReduction = holiday.Reduction;
			TransferDate = holiday.TransferDate.HasValue ? holiday.TransferDate.Value : holiday.Date;

			AvailableHolidayTypes = new ObservableCollection<HolidayType>(Enum.GetValues(typeof(HolidayType)).OfType<HolidayType>());
			HolidayType = holiday.Type;

			_validationErrors = new Dictionary<string, string>();

			return true;
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

		public DateTime SelectDateStartLimit
		{
			get { return new DateTime(Model.Date.Year, 1, 1); }
		}

		public DateTime SelectDateEndLimit
		{
			get { return new DateTime(Model.Date.Year, 12, 31); }
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

				if (_isNew)
					switch (_holidayType)
					{
						case HolidayType.BeforeHoliday:
							TimeReduction = TimeSpan.FromHours(1);
							break;
						default:
							TimeReduction = TimeSpan.FromHours(0);
							break;
					}
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

		private TimeSpan _timeReduction;
		public TimeSpan TimeReduction
		{
			get { return _timeReduction; }
			set
			{
				if (_timeReduction == value)
					return;
				_timeReduction = value;
				OnPropertyChanged(() => TimeReduction);
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
			Model.Reduction = TimeReduction;
			Model.TransferDate = IsTransferDateEnabled ? (DateTime?)TransferDate : null;

			return HolidayHelper.Save(Model, _isNew);
		}

		protected override bool CanSave()
		{
			// Не должно быть ошибок клиентской валидации данных
			return _validationErrors.Count == 0;
		}

		#region <Реализация интерфейса IDataErrorInfo>

		public string Error
		{
			get { return String.Join("\n", _validationErrors.Values.ToArray()); }
		}

		public string this[string propertyName]
		{
			get { return GetValidationError(propertyName); }
		}
		
		#endregion </Реализация интерфейса IDataErrorInfo>

		private IDictionary<string, string> _validationErrors;

		private string GetValidationError(string propertyName)
		{
			string error = null;

			switch (propertyName)
			{
				case "Date":
				{
					
					if (Date.Year != Model.Date.Year)
						error = FormatErrorString("Дата", GetInputYearError(Model.Date.Year));
					break;
				}
				case "TransferDate":
				{
					if (IsTransferDateEnabled && TransferDate.Year != Model.Date.Year)
						error = FormatErrorString("Дата переноса", GetInputYearError(Model.Date.Year));
					break;
				}
			}

			if (!String.IsNullOrEmpty(error))
			{
				if (!_validationErrors.ContainsKey(propertyName))
					_validationErrors.Add(propertyName, error);
				else
					_validationErrors[propertyName] = error;
			}
			else
			{
				if (_validationErrors.ContainsKey(propertyName))
					_validationErrors.Remove(propertyName);
			}
			OnPropertyChanged(() => Error);

			return error;
		}

		private string FormatErrorString(string fieldName, object neededValue)
		{
			return String.Format("* {0}: {1}", fieldName, neededValue);
		}

		private string GetInputYearError(int neededYear)
		{
			return String.Format("Год не равен '{0}'", neededYear);
		}
	}
}