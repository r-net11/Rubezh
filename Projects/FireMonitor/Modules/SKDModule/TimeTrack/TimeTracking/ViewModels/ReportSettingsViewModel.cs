using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events.Reports;
using SKDModule.Model;
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
			DateTime = DateTime.Now;
			RecordsPerPage = 3;
		}

		string _fillName;
		public string FillName
		{
			get { return _fillName; }
			set
			{
				_fillName = value;
				OnPropertyChanged(() => FillName);
			}
		}

		string _leadName;
		public string LeadName
		{
			get { return _leadName; }
			set
			{
				_leadName = value;
				OnPropertyChanged(() => LeadName);
			}
		}

		string _hrName;
		public string HRName
		{
			get { return _hrName; }
			set
			{
				_hrName = value;
				OnPropertyChanged(() => HRName);
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

		private string _docNumber;
		public string DocNumber
		{
			get { return _docNumber; }
			set
			{
				_docNumber = value;
				OnPropertyChanged(() => DocNumber);
			}
		}

		private string _OKPO;
		public string OKPO
		{
			get { return _OKPO; }
			set
			{
				_OKPO = value;
				OnPropertyChanged(() => OKPO);
			}
		}

		private int _recordsPerPage;
		public int RecordsPerPage
		{
			get { return _recordsPerPage; }
			set
			{
				_recordsPerPage = value;
				OnPropertyChanged(() => RecordsPerPage);
			}
		}
		

		protected override bool Save()
		{
			var report = new ReportT13()
			{
				FillName = FillName,
				LeadName = LeadName,
				HRName = HRName,
				DocNumber = DocNumber,
				DepartmentName = TimeTrackEmployeeResults[0].ShortEmployee.DepartmentName,
				OrganizationName = OrganisationHelper.GetByCurrentUser().Where(org => org.UID == TimeTrackEmployeeResults[0].ShortEmployee.OrganisationUID).Select(org => org.Name).FirstOrDefault(),
				OKPO = OKPO,
				CreationDateTime = DateTime,
				StartDateTime = TimeTrackFilter.StartDate.Date,
				EndDateTime = TimeTrackFilter.EndDate.Date,
				RecordsPerPage = RecordsPerPage,
			};
			if (report.EndDateTime > TimeTrackFilter.EndDate)
				report.EndDateTime = TimeTrackFilter.EndDate;
			if (!string.IsNullOrEmpty(report.DepartmentName) && TimeTrackEmployeeResults.Any(item => item.ShortEmployee.DepartmentName != report.DepartmentName))
				report.DepartmentName = null;

			foreach (var timeTrackEmployeeResult in TimeTrackEmployeeResults)
			{
				var employeeReport = new EmployeeReport();
				report.EmployeeRepors.Add(employeeReport);

				employeeReport.No = report.EmployeeRepors.Count;
				employeeReport.EmploueeFIO = timeTrackEmployeeResult.ShortEmployee.FIO;

				foreach (var dayTimeTrack in timeTrackEmployeeResult.DayTimeTracks)
				{
					if (dayTimeTrack.Date > report.StartDateTime && dayTimeTrack.Date < report.EndDateTime)
					{
						var employeeReportDay = new EmployeeReportDay();
						employeeReport.Days.Add(employeeReportDay);

						employeeReportDay.Code = dayTimeTrack.LetterCode;
						var totaPresence = dayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.Presence).TimeSpan;
						employeeReportDay.TimeSpan = totaPresence;

						if (dayTimeTrack.Date.Day <= 15)
						{
							if (totaPresence > TimeSpan.Zero)
								employeeReport.FirstHalfDaysCount++;
							employeeReport.FirstHalfTimeSpan += totaPresence;
						}
						else
						{
							if (totaPresence > TimeSpan.Zero)
								employeeReport.SecondHalfDaysCount++;
							employeeReport.SecondHalfTimeSpan += totaPresence;
						}
						if (totaPresence > TimeSpan.Zero)
							employeeReport.TotalDaysCount++;
						employeeReport.TotalTimeSpan += totaPresence;

						foreach (var trackPart in dayTimeTrack.CombinedTimeTrackParts)
						{
							if (employeeReport.MissReasons.Count < 8)
							{
								if (trackPart.MinTimeTrackDocumentType != null && trackPart.MinTimeTrackDocumentType.DocumentType == DocumentType.Absence)
								{
									var employeeReportMissReason = employeeReport.MissReasons.FirstOrDefault(x => x.Code == trackPart.MinTimeTrackDocumentType.ShortName);
									if (employeeReportMissReason == null)
									{
										employeeReportMissReason = new EmployeeReportMissReason()
										{
											Code = trackPart.MinTimeTrackDocumentType.ShortName
										};
									}
									employeeReportMissReason.TimeSpan += trackPart.Delta;
									employeeReport.MissReasons.Add(employeeReportMissReason);
								}
							}
						}
					}
				}
			}

			ServiceFactory.Events.GetEvent<PrintReportPreviewEvent>().Publish(new T13Report(report));
			return true;
		}
	}
}