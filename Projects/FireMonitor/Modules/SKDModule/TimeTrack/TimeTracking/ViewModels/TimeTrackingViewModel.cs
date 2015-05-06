using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using SKDModule.Events;
using SKDModule.Model;

namespace SKDModule.ViewModels
{
	public class TimeTrackingViewModel : ViewPartViewModel
	{
		TimeTrackFilter TimeTrackFilter;
		List<TimeTrackEmployeeResult> TimeTrackEmployeeResults;

		public TimeTrackingViewModel()
		{
			ShowFilterCommand = new RelayCommand(OnShowFilter);
			RefreshCommand = new RelayCommand(OnRefresh);
			PrintCommand = new RelayCommand(OnPrint, CanPrint);
			ShowDocumentTypesCommand = new RelayCommand(OnShowDocumentTypes);
			ServiceFactory.Events.GetEvent<UserChangedEvent>().Unsubscribe(OnUserChanged);
			ServiceFactory.Events.GetEvent<UserChangedEvent>().Subscribe(OnUserChanged);
			ServiceFactory.Events.GetEvent<EditDocumentEvent>().Unsubscribe(OnEditDocument);
			ServiceFactory.Events.GetEvent<EditDocumentEvent>().Subscribe(OnEditDocument);
			ServiceFactory.Events.GetEvent<RemoveDocumentEvent>().Unsubscribe(OnRemoveDocument);
			ServiceFactory.Events.GetEvent<RemoveDocumentEvent>().Subscribe(OnRemoveDocument);
			ServiceFactory.Events.GetEvent<EditTimeTrackPartEvent>().Unsubscribe(OnEditTimeTrackPart);
			ServiceFactory.Events.GetEvent<EditTimeTrackPartEvent>().Subscribe(OnEditTimeTrackPart);

			TimeTrackFilter = new TimeTrackFilter();
			TimeTrackFilter.EmployeeFilter = new EmployeeFilter()
			{
				UserUID = FiresecClient.FiresecManager.CurrentUser.UID,
			};

			TimeTrackFilter.Period = TimeTrackingPeriod.CurrentMonth;
			TimeTrackFilter.StartDate = DateTime.Today.AddDays(1 - DateTime.Today.Day);
			TimeTrackFilter.EndDate = DateTime.Today;


			var stopWatch = new Stopwatch();
			stopWatch.Start();
			UpdateGrid();
			stopWatch.Stop();
			Trace.WriteLine(string.Format("UpdateGrid(): {0}", stopWatch.Elapsed));
		}

		ObservableCollection<TimeTrackViewModel> _timeTracks;
		public ObservableCollection<TimeTrackViewModel> TimeTracks
		{
			get { return _timeTracks; }
			set
			{
				_timeTracks = value;
				OnPropertyChanged(() => TimeTracks);
			}
		}

		TimeTrackViewModel _selectedTimeTrack;
		public TimeTrackViewModel SelectedTimeTrack
		{
			get { return _selectedTimeTrack; }
			set
			{
				_selectedTimeTrack = value;
				OnPropertyChanged(() => SelectedTimeTrack);
				OnPropertyChanged(() => HasSelectedTimeTrack);
			}
		}

		public bool HasSelectedTimeTrack
		{
			get { return SelectedTimeTrack != null; }
		}

		public int TotalDays { get; private set; }
		public DateTime FirstDay { get; private set; }

		int _rowHeight;
		public int RowHeight
		{
			get { return _rowHeight; }
			set
			{
				_rowHeight = value;
				OnPropertyChanged(() => RowHeight);
			}
		}

		public RelayCommand ShowFilterCommand { get; private set; }
		void OnShowFilter()
		{
			var filterViewModel = new TimeTrackFilterViewModel(TimeTrackFilter);
			if (DialogService.ShowModalWindow(filterViewModel))
			{
				UpdateGrid();
			}
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			UpdateGrid();
			stopwatch.Stop();
			Trace.WriteLine("OnRefresh time " + stopwatch.Elapsed.ToString());
		}

		public RelayCommand PrintCommand { get; private set; }
		void OnPrint()
		{
			if (TimeTracks.Count == 0)
			{
				MessageBoxService.ShowWarning("В отчете нет ни одного сотрудника");
				return;
			}
			var organisationUIDs = new HashSet<Guid>();
			var departmentNames = new HashSet<string>();
			foreach (var timeTrack in TimeTracks)
			{
				if (timeTrack.ShortEmployee.OrganisationUID != Guid.Empty)
					organisationUIDs.Add(timeTrack.ShortEmployee.OrganisationUID);

				if (string.IsNullOrEmpty(timeTrack.ShortEmployee.DepartmentName))
				{
					MessageBoxService.ShowWarning("Сотрудник " + timeTrack.ShortEmployee.FIO + " не относится ни к одному из подразделений");
					return;
				}
				departmentNames.Add(timeTrack.ShortEmployee.DepartmentName);
			}
			if (organisationUIDs.Count > 1)
			{
				MessageBoxService.ShowWarning("В отчете должны дыть сотрудники только из одной организации");
				return;
			}
			if (departmentNames.Count > 1)
			{
				MessageBoxService.ShowWarning("В отчете должны дыть сотрудники только из одного подразделения");
				return;
			}

			if (TimeTrackFilter.StartDate.Date.Month < TimeTrackFilter.EndDate.Date.Month || TimeTrackFilter.StartDate.Date.Year < TimeTrackFilter.EndDate.Date.Year)
			{
				MessageBoxService.ShowWarning("В отчете содержаться данные за несколько месяцев. Будут показаны данные только за первый месяц");
			}

			var reportSettingsViewModel = new ReportSettingsViewModel(TimeTrackFilter, TimeTrackEmployeeResults);
			DialogService.ShowModalWindow(reportSettingsViewModel);
		}
		bool CanPrint()
		{
			return ApplicationService.IsReportEnabled;
		}

		void UpdateGrid()
		{
			using (new WaitWrapper())
			{
				var employeeUID = Guid.Empty;
				if (SelectedTimeTrack != null)
				{
					employeeUID = SelectedTimeTrack.ShortEmployee.UID;
				}

				TotalDays = (int)(TimeTrackFilter.EndDate - TimeTrackFilter.StartDate).TotalDays + 1;
				FirstDay = TimeTrackFilter.StartDate;

				TimeTracks = new ObservableCollection<TimeTrackViewModel>();
				//var timeTrackResult = EmployeeHelper.GetTimeTracks(TimeTrackFilter.EmployeeFilter, TimeTrackFilter.StartDate, TimeTrackFilter.EndDate);
				var stream = FiresecManager.FiresecService.GetTimeTracksStream(TimeTrackFilter.EmployeeFilter, TimeTrackFilter.StartDate, TimeTrackFilter.EndDate);
				var folderName = AppDataFolderHelper.GetFolder("TempServer");
				var inputFileName = Path.Combine(folderName, "TimeTrackResult.xml");
				var resultFileName = Path.Combine(folderName, "ClientTimeTrackResult.xml");
				var resultFileStream = File.Create(resultFileName);
				//CopyStream(stream, resultFileStream);
				using (var inputStream = new FileStream(inputFileName, FileMode.Open, FileAccess.Read))
				{
					CopyStream(inputStream, resultFileStream);
				}
				var timeTrackResult = Deserialize(resultFileName);
				if (timeTrackResult != null)
				{
					TimeTrackEmployeeResults = timeTrackResult.TimeTrackEmployeeResults;
					foreach (var timeTrackEmployeeResult in TimeTrackEmployeeResults)
					{
						var timeTrackViewModel = new TimeTrackViewModel(TimeTrackFilter, timeTrackEmployeeResult);
						TimeTracks.Add(timeTrackViewModel);
					}

					RowHeight = 60 + 20 * TimeTrackFilter.TotalTimeTrackTypeFilters.Count;
				}
				SelectedTimeTrack = TimeTracks.FirstOrDefault(x => x.ShortEmployee.UID == employeeUID);
				if (SelectedTimeTrack == null)
					SelectedTimeTrack = TimeTracks.FirstOrDefault();
			}
		}

		public static void CopyStream(Stream input, Stream output)
		{
			var buffer = new byte[8 * 1024];
			int length;
			while ((length = input.Read(buffer, 0, buffer.Length)) > 0)
			{
				output.Write(buffer, 0, length);
			}
			output.Close();
		}

		public static TimeTrackResult Deserialize(string fileName)
		{
			using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
			{
				var dataContractSerializer = new DataContractSerializer(typeof(TimeTrackResult));
				var result = (TimeTrackResult)dataContractSerializer.ReadObject(fileStream);
				fileStream.Close();
				return result;
			}
		}

		public RelayCommand ShowDocumentTypesCommand { get; private set; }
		void OnShowDocumentTypes()
		{
			var documentTypesViewModel = new DocumentTypesViewModel();
			DialogService.ShowModalWindow(documentTypesViewModel);
		}

		void OnUserChanged(UserChangedEventArgs args)
		{
			TimeTrackFilter.EmployeeFilter.UserUID = FiresecClient.FiresecManager.CurrentUser.UID;
			UpdateGrid();
		}

		#region DocumentEvents
		void OnEditDocument(TimeTrackDocument document)
		{
			var timeTrackViewModel = TimeTracks.FirstOrDefault(x => x.ShortEmployee.UID == document.EmployeeUID);
			if (timeTrackViewModel != null)
			{
				timeTrackViewModel.DocumentsViewModel.OnEditDocument(document);
			}
		}

		void OnRemoveDocument(TimeTrackDocument document)
		{
			var timeTrackViewModel = TimeTracks.FirstOrDefault(x => x.ShortEmployee.UID == document.EmployeeUID);
			if (timeTrackViewModel != null)
			{
				timeTrackViewModel.DocumentsViewModel.OnRemoveDocument(document);
			}
		}

		void OnEditTimeTrackPart(Guid uid)
		{
			var timeTrackViewModel = TimeTracks.FirstOrDefault(x => x.ShortEmployee.UID == uid);
			if (timeTrackViewModel != null)
			{
				timeTrackViewModel.DocumentsViewModel.OnEditTimeTrackPart(uid);
			}
		}
		#endregion
	}
}