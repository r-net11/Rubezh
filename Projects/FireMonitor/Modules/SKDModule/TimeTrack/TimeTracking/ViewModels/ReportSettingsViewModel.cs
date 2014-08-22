using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using FiresecAPI.SKD;
using SKDModule.Model;
using Infrastructure.Events.Reports;
using Infrastructure;
using SKDModule.Reports;

namespace SKDModule.ViewModels
{
	public class ReportSettingsViewModel : SaveCancelDialogViewModel
	{
		TimeTrackFilter TimeTrackFilter;
		public List<TimeTrackEmployeeResult> TimeTrackEmployeeResults { get; private set; }

		public ReportSettingsViewModel(TimeTrackFilter timeTrackFilter, List<TimeTrackEmployeeResult> timeTrackEmployeeResults)
		{
			Title = "Настройка печати отчета Т-13";
			TimeTrackFilter = timeTrackFilter;
			TimeTrackEmployeeResults = timeTrackEmployeeResults;
		}

		string _name1;
		public string Name1
		{
			get { return _name1; }
			set
			{
				_name1 = value;
				OnPropertyChanged(() => Name1);
			}
		}

		string _name2;
		public string Name2
		{
			get { return _name2; }
			set
			{
				_name2 = value;
				OnPropertyChanged(() => Name2);
			}
		}

		string _name3;
		public string Name3
		{
			get { return _name3; }
			set
			{
				_name3 = value;
				OnPropertyChanged(() => Name3);
			}
		}

		DateTime _dateTime;
		public DateTime DateTime
		{
			get { return _dateTime; }
			set
			{
				_dateTime = value;
				OnPropertyChanged(() => DateTime);
			}
		}

		protected override bool Save()
		{
			var reportModel = new ReportModel();
			reportModel.Name1 = Name1;
			reportModel.Name2 = Name2;
			reportModel.Name3 = Name3;
			reportModel.CreationDateTime = DateTime;
			reportModel.StartDateTime = new DateTime(TimeTrackFilter.StartDate.Date.Year, TimeTrackFilter.StartDate.Month, 1);
			reportModel.EndDateTime = reportModel.StartDateTime.AddMonths(1).AddDays(-1);
			if (reportModel.EndDateTime > TimeTrackFilter.EndDate)
				reportModel.EndDateTime = TimeTrackFilter.EndDate;

			int currentNo = 0;
			foreach (var timeTrackEmployeeResult in TimeTrackEmployeeResults)
			{
				var employeeReportModel = new EmployeeReportModel();
				reportModel.EmployeeRepors.Add(employeeReportModel);

				employeeReportModel.No = currentNo++;
				employeeReportModel.EmploueeFIO = timeTrackEmployeeResult.ShortEmployee.FIO;

				foreach (var dayTimeTrack in timeTrackEmployeeResult.DayTimeTracks)
				{
					if (dayTimeTrack.Date > reportModel.StartDateTime && dayTimeTrack.Date < reportModel.EndDateTime)
					{
						var employeeReportModelDay = new EmployeeReportModelDay();
						employeeReportModel.Days.Add(employeeReportModelDay);
						employeeReportModelDay.Code = dayTimeTrack.LetterCode;
						employeeReportModelDay.TimeSpan = dayTimeTrack.Total;

						if (dayTimeTrack.Documents != null)
						{
							foreach (var trackPart in dayTimeTrack.CombinedTimeTrackParts)
							{
								foreach (var documentCode in trackPart.DocumentCodes)
								{

								}
							}
						}
					}
				}
			}

			ServiceFactory.Events.GetEvent<PrintReportPreviewEvent>().Publish(new T13Report(reportModel));
			return true;
		}
	}
}