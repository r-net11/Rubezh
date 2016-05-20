using Common;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.Models;
using RubezhAPI.SKD;
using RubezhClient;
using RubezhClient.SKDHelpers;
using SKDModule.Events;
using SKDModule.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace SKDModule.ViewModels
{
	public class TimeTrackingViewModel : ViewPartViewModel
	{
		TimeTrackFilter TimeTrackFilter;
		List<TimeTrackEmployeeResult> TimeTrackEmployeeResults;

		public TimeTrackingViewModel()
		{
			ShowFilterCommand = new RelayCommand(OnShowFilter, CanShowFilter);
			RefreshCommand = new RelayCommand(OnRefresh, CanRefresh);
			PrintCommand = new RelayCommand(OnPrint, CanPrint);
			ShowDocumentTypesCommand = new RelayCommand(OnShowDocumentTypes, CanShowDocumentTypes);
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
				User = ClientManager.CurrentUser, 
			};

			TimeTrackFilter.Period = TimeTrackingPeriod.CurrentMonth;
			TimeTrackFilter.StartDate = DateTime.Today.AddDays(1 - DateTime.Today.Day);
			TimeTrackFilter.EndDate = DateTime.Today;

			TimeTracks = new SortableObservableCollection<TimeTrackViewModel>();
		}

		Guid _chiefUID;
		Guid _departmentChiefUID;
		Guid _hrChiefUID;

		SortableObservableCollection<TimeTrackViewModel> _timeTracks;
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
		bool CanShowFilter()
		{
			return IsConnected;
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			UpdateGrid();
		}
		bool CanRefresh()
		{
			return IsConnected;
		}

		public RelayCommand PrintCommand { get; private set; }
		void OnPrint()
		{
			if (ValidatePrint())
			{
				var employee = TimeTracks.FirstOrDefault().ShortEmployee;
				var organisationUid = employee.OrganisationUID;
				var departmentUid = employee.DepartmentUid.Value;
				var reportSettingsViewModel = new ReportSettingsViewModel(TimeTrackFilter, TimeTrackEmployeeResults, _chiefUID, _departmentChiefUID, _hrChiefUID, organisationUid, departmentUid);
				DialogService.ShowModalWindow(reportSettingsViewModel);
			}
		}
		public bool ValidatePrint()
		{
			if (TimeTracks.Count == 0)
			{
				MessageBoxService.ShowWarning("В отчете нет ни одного сотрудника");
				return false;
			}
			var organisationUids = new HashSet<Guid>();
			var departmentUids = new HashSet<Guid>();
			foreach (var timeTrack in TimeTracks)
			{
				if (timeTrack.ShortEmployee.OrganisationUID != Guid.Empty)
					organisationUids.Add(timeTrack.ShortEmployee.OrganisationUID);

				if (timeTrack.ShortEmployee.DepartmentUid == null)
				{
					MessageBoxService.ShowWarning("Сотрудник " + timeTrack.ShortEmployee.FIO + " не относится ни к одному из подразделений");
					return false;
				}
				departmentUids.Add(timeTrack.ShortEmployee.DepartmentUid.Value);
			}
			if (organisationUids.Count > 1)
			{
				MessageBoxService.ShowWarning("В отчете должны быть сотрудники только из одной организации");
				return false;
			}
			if (departmentUids.Count > 1)
			{
				MessageBoxService.ShowWarning("В отчете должны быть сотрудники только из одного подразделения");
				return false;
			}
			if (TimeTrackFilter.StartDate.Date.Month < TimeTrackFilter.EndDate.Date.Month || TimeTrackFilter.StartDate.Date.Year < TimeTrackFilter.EndDate.Date.Year)
			{
				MessageBoxService.ShowWarning("В отчете содержаться данные за несколько месяцев. Будут показаны данные только за первый месяц");
			}
			return true;
		}
		bool CanPrint()
		{
			return ApplicationService.IsReportEnabled && ClientManager.CheckPermission(PermissionType.Oper_Reports_T13) && IsConnected;
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
				var stream = ClientManager.RubezhService.GetTimeTracksStream(TimeTrackFilter.EmployeeFilter, TimeTrackFilter.StartDate, TimeTrackFilter.EndDate);
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
		bool CanShowDocumentTypes()
		{
			return IsConnected;
		}

		public RelayCommand ShowNightSettingsCommand { get; private set; }
		void OnShowNightSettingsCommand()
		{
			var nightSettingsViewModel = new NightSettingsViewModel();
			DialogService.ShowModalWindow(nightSettingsViewModel);
		}
		bool CanShowNightSettings()
		{
			return ClientManager.CheckPermission(RubezhAPI.Models.PermissionType.Oper_SKD_TimeTrack_NightSettings_Edit) && IsConnected;
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
				_chiefUID = organisation.ChiefUID;
				_hrChiefUID = organisation.HRChiefUID;
				_departmentChiefUID = department.ChiefUID;
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

		public bool IsConnected { get { return ((SafeRubezhService)ClientManager.RubezhService).IsConnected; } }
	}
}