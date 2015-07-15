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
using ReactiveUI.Xaml;
using SKDModule.Events;
using SKDModule.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using System.Windows.Input;

namespace SKDModule.ViewModels
{
	public class TimeTrackingViewModel : ViewPartViewModel
	{
		#region Fields

		readonly TimeTrackFilter _timeTrackFilter;
		List<TimeTrackEmployeeResult> _timeTrackEmployeeResults;
		private List<TimeTrack> _cachedTimeTracks;
		private const int PageCountElements = 25; //Определяет количество записей, отображаемых на одной странице в таблице УРВ
		private readonly ObservableAsPropertyHelper<ObservableCollection<TimeTrack>> _searchResults;

		#endregion

		#region Properties

		public ObservableCollection<TimeTrack> SearchResults
		{
			get
			{
				return _searchResults != null ? _searchResults.Value : null;
			}
		}

		public List<Holiday> HolydaysOfCurrentOrganisation
		{
			get { return HolidayHelper.GetByOrganisation(_timeTrackFilter.EmployeeFilter.OrganisationUIDs.FirstOrDefault()).ToList(); }
		}

		private int _pageNumber;
		public int PageNumber
		{
			get { return _pageNumber; }
			set
			{
				if (value == _pageNumber) return;
				_pageNumber = value;
				OnPropertyChanged(() => PageNumber);
			}
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
			SubscribeOnEvents();

			ShowFilterCommand = new RelayCommand(OnShowFilter);
			PrintCommand = new RelayCommand(OnPrint, CanPrint);
			ShowDocumentTypesCommand = new RelayCommand(OnShowDocumentTypes);

			_timeTrackFilter = CreateTimeTrackFilter();

			UpdateGrid();
			PreviousPageCommand = new ReactiveAsyncCommand();
			PreviousPageCommand.RegisterAsyncAction(_ => { --PageNumber; });

			NextPageCommand = new ReactiveAsyncCommand();
			NextPageCommand.RegisterAsyncAction(_ => { PageNumber++; });

			RefreshCommand = new ReactiveAsyncCommand();
			RefreshCommand.RegisterAsyncAction(_ => UpdateGrid()).Subscribe(x =>
			{
				//if (SelectedTimeTrack == null && SearchResults == null) return;
				//SelectedTimeTrack = SelectedTimeTrack ?? SearchResults.FirstOrDefault();
			});

			var executeSearchCommand = new ReactiveAsyncCommand();
			executeSearchCommand.Subscribe(x =>
			{
				if (SelectedTimeTrack == null && SearchResults == null) return;
				SelectedTimeTrack = SelectedTimeTrack ?? SearchResults.FirstOrDefault();

			});
			var results = executeSearchCommand.RegisterAsyncFunction(s => ExecuteSearch((int) s));
			_executeSearchCommand = executeSearchCommand;

			this.WhenAny(x => x.IsActive, x => x.PageNumber,
				(isActive, pageNumber) => new {IsActive = isActive.Value, PageNumber = pageNumber.Value})
			//	.Throttle(TimeSpan.FromMilliseconds(1000))
				.Select(x => x.PageNumber)
				.Where(x => x >= 0)
				.Subscribe(value => _executeSearchCommand.Execute(value));

			_searchResults = new ObservableAsPropertyHelper<ObservableCollection<TimeTrack>>(results, _ => OnPropertyChanged(() => SearchResults));

			this.WhenAny(x => x.IsActive, x => x.Value)
				.Subscribe(value =>
				{
					if (!value || (SelectedTimeTrack == null && SearchResults == null)) return;
					SelectedTimeTrack = SelectedTimeTrack ?? SearchResults.FirstOrDefault();
				});

			this.WhenAny(x => x.SelectedTimeTrack, x => x.Value)
				.Subscribe(_ =>
				{
					HasSelectedTimeTrack = _ != null;
				});
		}

		#endregion

		#region Methods

		private void SubscribeOnEvents()
		{
			ServiceFactory.Events.GetEvent<UserChangedEvent>().Unsubscribe(OnUserChanged);
			ServiceFactory.Events.GetEvent<UserChangedEvent>().Subscribe(OnUserChanged);
			ServiceFactory.Events.GetEvent<EditDocumentEvent>().Unsubscribe(OnEditDocument);
			ServiceFactory.Events.GetEvent<EditDocumentEvent>().Subscribe(OnEditDocument);
			ServiceFactory.Events.GetEvent<RemoveDocumentEvent>().Unsubscribe(OnRemoveDocument);
			ServiceFactory.Events.GetEvent<RemoveDocumentEvent>().Subscribe(OnRemoveDocument);
			ServiceFactory.Events.GetEvent<EditTimeTrackPartEvent>().Unsubscribe(OnEditTimeTrackPart);
			ServiceFactory.Events.GetEvent<EditTimeTrackPartEvent>().Subscribe(OnEditTimeTrackPart);
		}

		private ObservableCollection<TimeTrack> ExecuteSearch(int pageNumber)
		{
			return new ObservableCollection<TimeTrack>(_cachedTimeTracks.Skip(pageNumber * PageCountElements).Take(PageCountElements));
		}

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
			TotalDays = (int)(_timeTrackFilter.EndDate - _timeTrackFilter.StartDate).TotalDays + 1;
			FirstDay = _timeTrackFilter.StartDate;

			var timeTrackResult = GetServerTimeTrackResults();

			if (timeTrackResult == null) return;

			TimeTracks = new SortableObservableCollection<TimeTrack>();

			_timeTrackEmployeeResults = timeTrackResult.TimeTrackEmployeeResults;
			_cachedTimeTracks = _timeTrackEmployeeResults.Select(x => new TimeTrack(_timeTrackFilter, x)).OrderBy(x => x.ShortEmployee.FirstName).ToList();

			RowHeight = 60 + 20 * _timeTrackFilter.TotalTimeTrackTypeFilters.Count;
		}

		private TimeTrackResult GetServerTimeTrackResults()
		{
			var resultFileName = Path.Combine(AppDataFolderHelper.GetFolder("TempServer"), "ClientTimeTrackResult.xml");
			var resultFileStream = File.Create(resultFileName);
			var stream = FiresecManager.FiresecService.GetTimeTracksStream(_timeTrackFilter.EmployeeFilter, _timeTrackFilter.StartDate, _timeTrackFilter.EndDate);
			FiresecManager.CopyStream(stream, resultFileStream);

			return Deserialize(resultFileName);
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

		private readonly ICommand _executeSearchCommand;

		public ReactiveAsyncCommand PreviousPageCommand { get; private set; }

		public ReactiveAsyncCommand NextPageCommand { get; private set; }

		public RelayCommand ShowFilterCommand { get; private set; }
		void OnShowFilter()
		{
			var filterViewModel = new TimeTrackFilterViewModel(_timeTrackFilter);
			if (DialogService.ShowModalWindow(filterViewModel))
			{
				UpdateGrid();
			}
		}

		public ReactiveAsyncCommand RefreshCommand { get; private set; }

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