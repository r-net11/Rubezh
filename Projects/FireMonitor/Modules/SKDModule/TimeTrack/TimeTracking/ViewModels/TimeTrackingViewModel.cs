using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Common;
using FiresecAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using ReactiveUI;
using SKDModule.Events;
using SKDModule.Model;

namespace SKDModule.ViewModels
{
	public class TimeTrackingViewModel : ViewPartViewModel
	{
		#region Fields

		readonly TimeTrackFilter _timeTrackFilter;
		List<TimeTrackEmployeeResult> _timeTrackEmployeeResults;

		#endregion

		#region Properties

		public List<Holiday> HolydaysOfCurrentOrganisation
		{
			get { return HolidayHelper.GetByOrganisation(_timeTrackFilter.EmployeeFilter.OrganisationUIDs.FirstOrDefault()).ToList(); }
		}

		SortableObservableCollection<TimeTrack> _timeTracks;
		public SortableObservableCollection<TimeTrack> TimeTracks
		{
			get { return _timeTracks; }
			set
			{
				_timeTracks = value;
				OnPropertyChanged(() => TimeTracks);
			}
		}

		TimeTrack _selectedTimeTrack;
		public TimeTrack SelectedTimeTrack
		{
			get { return _selectedTimeTrack; }
			set
			{
				_selectedTimeTrack = value;
				OnPropertyChanged(() => SelectedTimeTrack);
			}
		}

		private bool _hasSelectedTimeTrack;
		public bool HasSelectedTimeTrack
		{
			get { return _hasSelectedTimeTrack; }
			set
			{
				_hasSelectedTimeTrack = value;
				OnPropertyChanged(() => HasSelectedTimeTrack);
			}
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

		#endregion

		#region Constructors

		public TimeTrackingViewModel()
		{
			ServiceFactory.Events.GetEvent<UserChangedEvent>().Unsubscribe(OnUserChanged);
			ServiceFactory.Events.GetEvent<UserChangedEvent>().Subscribe(OnUserChanged);
			ServiceFactory.Events.GetEvent<EditDocumentEvent>().Unsubscribe(OnEditDocument);
			ServiceFactory.Events.GetEvent<EditDocumentEvent>().Subscribe(OnEditDocument);
			ServiceFactory.Events.GetEvent<RemoveDocumentEvent>().Unsubscribe(OnRemoveDocument);
			ServiceFactory.Events.GetEvent<RemoveDocumentEvent>().Subscribe(OnRemoveDocument);
			ServiceFactory.Events.GetEvent<EditTimeTrackPartEvent>().Unsubscribe(OnEditTimeTrackPart);
			ServiceFactory.Events.GetEvent<EditTimeTrackPartEvent>().Subscribe(OnEditTimeTrackPart);

			ShowFilterCommand = new RelayCommand(OnShowFilter);
			RefreshCommand = new RelayCommand(OnRefresh);
			PrintCommand = new RelayCommand(OnPrint, CanPrint);
			ShowDocumentTypesCommand = new RelayCommand(OnShowDocumentTypes);

			_timeTrackFilter = CreateTimeTrackFilter();

			UpdateGrid();

			this.WhenAny(x => x.SelectedTimeTrack, x => x.Value)
				.Subscribe(_ =>
				{
					HasSelectedTimeTrack = _ != null;
				});
		}

		#endregion

		#region Methods

		private TimeTrackFilter CreateTimeTrackFilter() //TODO:Implement to TimeTrackFilter class
		{
			return new TimeTrackFilter
			{
				EmployeeFilter = new EmployeeFilter
				{
					OrganisationUIDs = new List<Guid> { GetFirstOrganizationUID() },
					UserUID = FiresecManager.CurrentUser.UID,
				},
				Period = TimeTrackingPeriod.CurrentMonth,
				StartDate = GetFirstDayOfMonth(),
				EndDate = DateTime.Today
			};
		}

		public static DateTime GetFirstDayOfMonth() //TODO: Implement to static extension class
		{
			return DateTime.Today.AddDays(1 - DateTime.Today.Day);
		}

		private Guid GetFirstOrganizationUID()
		{
			var firstOrganizationElement = OrganisationHelper.GetByCurrentUser().FirstOrDefault();
			return firstOrganizationElement != null ? firstOrganizationElement.UID : Guid.Empty;
		}

		void UpdateGrid()
		{
			var employeeUID = SelectedTimeTrack != null ? SelectedTimeTrack.ShortEmployee.UID : Guid.Empty;

			TotalDays = (int)(_timeTrackFilter.EndDate - _timeTrackFilter.StartDate).TotalDays + 1;
			FirstDay = _timeTrackFilter.StartDate;


			var stream = FiresecManager.FiresecService.GetTimeTracksStream(_timeTrackFilter.EmployeeFilter, _timeTrackFilter.StartDate, _timeTrackFilter.EndDate);
			var folderName = AppDataFolderHelper.GetFolder("TempServer");
			var resultFileName = Path.Combine(folderName, "ClientTimeTrackResult.xml");
			var resultFileStream = File.Create(resultFileName);
			FiresecManager.CopyStream(stream, resultFileStream);
			var timeTrackResult = Deserialize(resultFileName);

			TimeTracks = new SortableObservableCollection<TimeTrack>();

			if (timeTrackResult != null)
			{
				_timeTrackEmployeeResults = timeTrackResult.TimeTrackEmployeeResults;
				foreach (var timeTrackEmployeeResult in _timeTrackEmployeeResults)
				{
					TimeTracks.Add(new TimeTrack(_timeTrackFilter, timeTrackEmployeeResult));
				}

				TimeTracks.Sort(x => x.ShortEmployee.LastName);
				RowHeight = 60 + 20 * _timeTrackFilter.TotalTimeTrackTypeFilters.Count;
			}

			SelectedTimeTrack = TimeTracks.FirstOrDefault(x => x.ShortEmployee.UID == employeeUID) ??
								TimeTracks.FirstOrDefault();
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

		#endregion

		#region Commands

		public RelayCommand ShowFilterCommand { get; private set; }
		void OnShowFilter()
		{
			var filterViewModel = new TimeTrackFilterViewModel(_timeTrackFilter);
			if (DialogService.ShowModalWindow(filterViewModel))
			{
				UpdateGrid();
			}
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			UpdateGrid();
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

			if (_timeTrackFilter.StartDate.Date.Month < _timeTrackFilter.EndDate.Date.Month || _timeTrackFilter.StartDate.Date.Year < _timeTrackFilter.EndDate.Date.Year)
			{
				MessageBoxService.ShowWarning("В отчете содержаться данные за несколько месяцев. Будут показаны данные только за первый месяц");
			}

			var reportSettingsViewModel = new ReportSettingsViewModel(_timeTrackFilter, _timeTrackEmployeeResults);
			DialogService.ShowModalWindow(reportSettingsViewModel);
		}
		bool CanPrint()
		{
			return ApplicationService.IsReportEnabled;
		}

		public RelayCommand ShowDocumentTypesCommand { get; private set; }
		void OnShowDocumentTypes()
		{
			var documentTypesViewModel = new DocumentTypesViewModel();
			DialogService.ShowModalWindow(documentTypesViewModel);
		}

		#endregion

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

		#region MonitorEvents
		void OnUserChanged(UserChangedEventArgs args)
		{
			_timeTrackFilter.EmployeeFilter.UserUID = FiresecManager.CurrentUser.UID;
			UpdateGrid();
		}

		#endregion
	}
}