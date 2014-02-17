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
		public EmployeeReportFilter EmployeeReportFilter { get; private set; }

		public ReportSettingsViewModel(EmployeeReportFilter employeeReportFilter)
		{
			Title = "Настройка отчета";
			EmployeeReportFilter = employeeReportFilter;
			StartDateTime = employeeReportFilter.StartDateTime;
			EndDateTime = employeeReportFilter.EndDateTime;

			AvailableReportTypes = new ObservableCollection<EmployeeReportType>(Enum.GetValues(typeof(EmployeeReportType)).OfType<EmployeeReportType>());
			ReportType = employeeReportFilter.EmployeeReportType;
		}

		DateTime _startDateTime;
		public DateTime StartDateTime
		{
			get { return _startDateTime; }
			set
			{
				_startDateTime = value;
				OnPropertyChanged("StartDateTime");
			}
		}

		DateTime _endDateTime;
		public DateTime EndDateTime
		{
			get { return _endDateTime; }
			set
			{
				_endDateTime = value;
				OnPropertyChanged("EndDateTime");
			}
		}

		public ObservableCollection<EmployeeReportType> AvailableReportTypes { get; private set; }

		EmployeeReportType _reportType;
		public EmployeeReportType ReportType
		{
			get { return _reportType; }
			set
			{
				_reportType = value;
				OnPropertyChanged("ReportType");
			}
		}

		protected override bool Save()
		{
			if (StartDateTime > EndDateTime)
			{
				MessageBoxService.ShowWarning("Дата окончания не может быть раньше даты начала");
				return false;
			}

			EmployeeReportFilter = new EmployeeReportFilter()
			{
				StartDateTime = StartDateTime,
				EndDateTime = EndDateTime,
				EmployeeReportType = ReportType
			};
			return true;
		}
	}
}