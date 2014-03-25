using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;
using SKDModule.Models;
using Infrastructure.Common.Windows;
using System.Collections.ObjectModel;

namespace SKDModule.ViewModels
{
	public class ReportSettingsViewModel : SaveCancelDialogViewModel
	{
		private EmployeeReportSettings _employeeReportSettings;

		public ReportSettingsViewModel(EmployeeReportSettings employeeReportSettings)
		{
			Title = "Настройка отчета";
			_employeeReportSettings = employeeReportSettings;
			ReportTypes = new ObservableCollection<EmployeeReportType>(Enum.GetValues(typeof(EmployeeReportType)).OfType<EmployeeReportType>());
			ReportPeriods = new ObservableCollection<EmployeeReportPeriod>(Enum.GetValues(typeof(EmployeeReportPeriod)).OfType<EmployeeReportPeriod>());

			ReportType = _employeeReportSettings.EmployeeReportType;
			ReportPeriod = _employeeReportSettings.EmployeeReportPeriod;
			StartDateTime = _employeeReportSettings.StartDateTime;
			EndDateTime = _employeeReportSettings.EndDateTime;
		}

		private DateTime _startDateTime;
		public DateTime StartDateTime
		{
			get { return _startDateTime; }
			set
			{
				_startDateTime = value;
				OnPropertyChanged(() => StartDateTime);
			}
		}

		private DateTime _endDateTime;
		public DateTime EndDateTime
		{
			get { return _endDateTime; }
			set
			{
				_endDateTime = value;
				OnPropertyChanged(() => EndDateTime);
			}
		}

		public ObservableCollection<EmployeeReportType> ReportTypes { get; private set; }
		public ObservableCollection<EmployeeReportPeriod> ReportPeriods { get; private set; }

		private EmployeeReportType _reportType;
		public EmployeeReportType ReportType
		{
			get { return _reportType; }
			set
			{
				_reportType = value;
				OnPropertyChanged("ReportType");
			}
		}
		private EmployeeReportPeriod _reportPeriod;
		public EmployeeReportPeriod ReportPeriod
		{
			get { return _reportPeriod; }
			set
			{
				_reportPeriod = value;
				OnPropertyChanged(() => ReportPeriod);
				OnPropertyChanged(() => IsFreePeriod);
			}
		}

		public bool IsFreePeriod
		{
			get { return ReportPeriod == EmployeeReportPeriod.Period; }
		}

		protected override bool Save()
		{
			if (ReportPeriod == EmployeeReportPeriod.Period && StartDateTime > EndDateTime)
			{
				MessageBoxService.ShowWarning("Дата окончания не может быть раньше даты начала");
				return false;
			}

			_employeeReportSettings.StartDateTime = StartDateTime;
			_employeeReportSettings.EndDateTime = EndDateTime;
			_employeeReportSettings.EmployeeReportType = ReportType;
			_employeeReportSettings.EmployeeReportPeriod = ReportPeriod;
			return true;
		}
	}
}