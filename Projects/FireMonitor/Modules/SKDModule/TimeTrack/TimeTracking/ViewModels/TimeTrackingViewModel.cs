using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Common;
using FiresecAPI.SKD;
using RubezhClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using SKDModule.Events;
using SKDModule.Model;
using RubezhClient.SKDHelpers;
using FiresecAPI.Models;

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
			ShowNightSettingsCommand = new RelayCommand(OnShowNightSettingsCommand, CanShowNightSettings);
			ServiceFactory.Events.GetEvent<EditDocumentEvent>().Unsubscribe(OnEditDocument);
			ServiceFactory.Events.GetEvent<EditDocumentEvent>().Subscribe(OnEditDocument);
			ServiceFactory.Events.GetEvent<RemoveDocumentEvent>().Unsubscribe(OnRemoveDocument);
			ServiceFactory.Events.GetEvent<RemoveDocumentEvent>().Subscribe(OnRemoveDocument);
			ServiceFactory.Events.GetEvent<EditTimeTrackPartEvent>().Unsubscribe(OnEditTimeTrackPart);
			ServiceFactory.Events.GetEvent<EditTimeTrackPartEvent>().Subscribe(OnEditTimeTrackPart);
			ServiceFactory.Events.GetEvent<ChiefChangedEvent>().Unsubscribe(OnInitializeLeadUIDs);
			ServiceFactory.Events.GetEvent<ChiefChangedEvent>().Subscribe(OnInitializeLeadUIDs);

			TimeTrackFilter = new TimeTrackFilter();
			TimeTrackFilter.EmployeeFilter = new EmployeeFilter()
			{
				UserUID = ClientManager.CurrentUser.UID,
			};

			TimeTrackFilter.Period = TimeTrackingPeriod.CurrentMonth;
			TimeTrackFilter.StartDate = DateTime.Today.AddDays(1 - DateTime.Today.Day);
			TimeTrackFilter.EndDate = DateTime.Today;

			TimeTracks = new SortableObservableCollection<TimeTrackViewModel>();
			//UpdateGrid();
		}

		SortableObservableCollection<TimeTrackViewModel> _timeTracks;
		Guid chiefUID;
		Guid departmentChiefUID;
		Guid hrChiefUID;
		public SortableObservableCollection<TimeTrackViewModel> TimeTracks
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
			OnInitializeLeadUIDs(filterViewModel);
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

			var reportSettingsViewModel = new ReportSettingsViewModel(TimeTrackFilter, TimeTrackEmployeeResults, chiefUID, departmentChiefUID, hrChiefUID);
			DialogService.ShowModalWindow(reportSettingsViewModel);
		}
		bool CanPrint()
		{
			return ApplicationService.IsReportEnabled && ClientManager.CheckPermission(PermissionType.Oper_Reports_T13);
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

				TimeTracks = new SortableObservableCollection<TimeTrackViewModel>();
				var stream = ClientManager.FiresecService.GetTimeTracksStream(TimeTrackFilter.EmployeeFilter, TimeTrackFilter.StartDate, TimeTrackFilter.EndDate);
				var folderName = AppDataFolderHelper.GetFolder("TempServer");
				var resultFileName = Path.Combine(folderName, "ClientTimeTrackResult.xml");
				var resultFileStream = File.Create(resultFileName);
				CopyStream(stream, resultFileStream);
				var timeTrackResult = Deserialize(resultFileName);

				if (timeTrackResult != null)
				{
					TimeTrackEmployeeResults = timeTrackResult.TimeTrackEmployeeResults;
					foreach (var timeTrackEmployeeResult in TimeTrackEmployeeResults)
					{
						var timeTrackViewModel = new TimeTrackViewModel(TimeTrackFilter, timeTrackEmployeeResult);
						TimeTracks.Add(timeTrackViewModel);
					}

					TimeTracks.Sort(x => x.ShortEmployee.LastName);
					RowHeight = 60 + 20 * TimeTrackFilter.TotalTimeTrackTypeFilters.Count;
				}

				SelectedTimeTrack = TimeTracks.FirstOrDefault(x => x.ShortEmployee.UID == employeeUID) ??
									TimeTracks.FirstOrDefault();
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
		public RelayCommand ShowNightSettingsCommand { get; private set; }
		void OnShowNightSettingsCommand()
		{
			var nightSettingsViewModel = new NightSettingsViewModel();
			DialogService.ShowModalWindow(nightSettingsViewModel);
		}
		bool CanShowNightSettings()
		{
			return ClientManager.CheckPermission(FiresecAPI.Models.PermissionType.Oper_SKD_TimeTrack_NightSettings_Edit);
		}

		public void OnInitializeLeadUIDs(TimeTrackFilterViewModel filterViewModel)
		{
			if (filterViewModel == null)
				filterViewModel = new TimeTrackFilterViewModel(TimeTrackFilter);
			if (filterViewModel.DepartmentsFilterViewModel.UIDs.Count == 1)
			{
				Guid depUID = filterViewModel.DepartmentsFilterViewModel.UIDs[0];
				var department = DepartmentHelper.GetSingleShort(depUID);
				Guid orgUID = department.OrganisationUID;
				var organisation = OrganisationHelper.GetSingle(orgUID);
				chiefUID = organisation.ChiefUID;
				hrChiefUID = organisation.HRChiefUID;
				departmentChiefUID = department.ChiefUID;
			}
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