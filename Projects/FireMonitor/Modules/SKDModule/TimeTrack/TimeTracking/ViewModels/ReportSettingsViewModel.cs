using System;
using System.Collections.Generic;
using System.Linq;
using StrazhAPI.SKD;
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
			FillViewModel = new EmployeeSelectationViewModel(Guid.Empty, timeTrackFilter.EmployeeFilter);
			LeadViewModel = new EmployeeSelectationViewModel(Guid.Empty, timeTrackFilter.EmployeeFilter);
			var hrFilter = new EmployeeFilter { OrganisationUIDs = timeTrackFilter.EmployeeFilter.OrganisationUIDs };
			HRViewModel = new EmployeeSelectationViewModel(Guid.Empty, hrFilter);
		}

		public EmployeeSelectationViewModel FillViewModel { get; private set; }
		public EmployeeSelectationViewModel LeadViewModel { get; private set; }
		public EmployeeSelectationViewModel HRViewModel { get; private set; }
		
		//string _fillName;
		//public string FillName
		//{
		//	get { return _fillName; }
		//	set
		//	{
		//		_fillName = value;
		//		OnPropertyChanged(() => FillName);
		//	}
		//}

		//string _leadName;
		//public string LeadName
		//{
		//	get { return _leadName; }
		//	set
		//	{
		//		_leadName = value;
		//		OnPropertyChanged(() => LeadName);
		//	}
		//}

		//string _hrName;
		//public string HRName
		//{
		//	get { return _hrName; }
		//	set
		//	{
		//		_hrName = value;
		//		OnPropertyChanged(() => HRName);
		//	}
		//}

		//string _fillPosition;
		//public string FillPosition
		//{
		//	get { return _fillPosition; }
		//	set
		//	{
		//		_fillPosition = value;
		//		OnPropertyChanged(() => FillPosition);
		//	}
		//}

		//string _leadPosition;
		//public string LeadPosition
		//{
		//	get { return _leadPosition; }
		//	set
		//	{
		//		_leadPosition = value;
		//		OnPropertyChanged(() => LeadPosition);
		//	}
		//}

		//string _hrPosition;
		//public string HRPosition
		//{
		//	get { return _hrPosition; }
		//	set
		//	{
		//		_hrPosition = value;
		//		OnPropertyChanged(() => HRPosition);
		//	}
		//}

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
			var report = new ReportT13()
			{
				FillName = FillViewModel.HasSelected ? FillViewModel.SelectedEmployee.Name : "",
				LeadName = LeadViewModel.HasSelected ? LeadViewModel.SelectedEmployee.Name : "",
				HRName = HRViewModel.HasSelected ? HRViewModel.SelectedEmployee.Name : "",
				FillPosition = FillViewModel.HasSelected ? FillViewModel.SelectedEmployee.PositionName : "",
				LeadPosition = LeadViewModel.HasSelected ? LeadViewModel.SelectedEmployee.PositionName : "",
				HRPosition = HRViewModel.HasSelected ? HRViewModel.SelectedEmployee.PositionName : "",
				OrganisationName = OrganisationHelper.GetByCurrentUser().Where(org => org.UID == TimeTrackEmployeeResults[0].ShortEmployee.OrganisationUID).Select(org => org.Name).FirstOrDefault(),
				DepartmentName = TimeTrackEmployeeResults.FirstOrDefault().ShortEmployee.DepartmentName,
				CreationDateTime = DateTime,
				StartDateTime = TimeTrackFilter.StartDate.Date,
				EndDateTime = TimeTrackFilter.EndDate.Date,
			};
			if (report.EndDateTime > TimeTrackFilter.EndDate)
				report.EndDateTime = TimeTrackFilter.EndDate;

			foreach (var timeTrackEmployeeResult in TimeTrackEmployeeResults)
			{
				var employeeReport = new EmployeeReport();
				report.EmployeeRepors.Add(employeeReport);

				employeeReport.No = report.EmployeeRepors.Count;
				employeeReport.EmployeeFIO = String.IsNullOrEmpty(timeTrackEmployeeResult.ShortEmployee.PositionName) ? timeTrackEmployeeResult.ShortEmployee.FIO : String.Format("{0}, {1}", timeTrackEmployeeResult.ShortEmployee.FIO, timeTrackEmployeeResult.ShortEmployee.PositionName);
				employeeReport.TabelNo = timeTrackEmployeeResult.ShortEmployee.TabelNo;
				employeeReport.DepartmentName = timeTrackEmployeeResult.ShortEmployee.DepartmentName;

				foreach (var dayTimeTrack in timeTrackEmployeeResult.DayTimeTracks)
				{
					if (dayTimeTrack.Date >= report.StartDateTime.Date && dayTimeTrack.Date <= report.EndDateTime.Date)
					{
						var employeeReportDay = new EmployeeReportDay();
						employeeReport.Days.Add(employeeReportDay);

						var codeCount = 1;
						string codeStrings = dayTimeTrack.LetterCode;
						if (codeStrings == "УР")
							codeStrings = "НН";
						var totaPresence = dayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.Presence).TimeSpan;
						string timeSpanStrings = totaPresence.Hours.ToString();
						foreach (var timeTrackPart in dayTimeTrack.DocumentTrackParts)
						{
							if (timeTrackPart.MinTimeTrackDocumentType != null && timeTrackPart.Delta > TimeSpan.Zero)
							{
								if (timeTrackPart.MinTimeTrackDocumentType.DocumentType == StrazhAPI.SKD.DocumentType.Overtime || timeTrackPart.MinTimeTrackDocumentType.DocumentType == StrazhAPI.SKD.DocumentType.Absence)
							{
									if (codeCount < 3)
									{
										codeStrings += "/" + timeTrackPart.MinTimeTrackDocumentType.ShortName;
										timeSpanStrings += "/" + timeTrackPart.Delta.Hours.ToString();
										codeCount++;
									}
								}
							}
						}
						if (timeSpanStrings == "0")
							timeSpanStrings = "";
						employeeReportDay.CodeStrings = codeStrings;
						employeeReportDay.TimeSpanStrings = timeSpanStrings;

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
								if (trackPart.MinTimeTrackDocumentType != null && (trackPart.MinTimeTrackDocumentType.DocumentType == StrazhAPI.SKD.DocumentType.Absence || trackPart.MinTimeTrackDocumentType.DocumentType == StrazhAPI.SKD.DocumentType.Presence))
								{
									var employeeReportMissReason = employeeReport.MissReasons.FirstOrDefault(x => x.Code == trackPart.MinTimeTrackDocumentType.ShortName);
									if (employeeReportMissReason == null)
									{
										employeeReportMissReason = new EmployeeReportMissReason()
										{
											Code = trackPart.MinTimeTrackDocumentType.ShortName
										};
										employeeReport.MissReasons.Add(employeeReportMissReason);
									}
									employeeReportMissReason.TimeSpan += trackPart.Delta;
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