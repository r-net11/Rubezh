using System.Threading.Tasks;
using StrazhAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Services;
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

		private const int InitialPageNumber = 1;
		private bool _isEnabledTimeTrackFilter;
		private bool _isBisy;
		private bool _canShowDocumentTypes;
		readonly TimeTrackFilter _timeTrackFilter;
		private TimeTrack _selectedTimeTrack;
		private bool _hasSelectedTimeTrack;
		private List<TimeTrackEmployeeResult> _timeTrackEmployeeResults;
		private List<TimeTrack> _cachedTimeTracks;
		private const int RecordsPerPage = 25;
		private readonly ObservableAsPropertyHelper<ObservableCollection<TimeTrack>> _searchResults;
		private readonly ObservableAsPropertyHelper<int> _totalPageNumber;
		private int _pageNumber;
		int _rowHeight;
		private bool _isFilterAccepted;
		private static readonly object Locker = new object();

		#endregion

		#region Properties

		public bool IsEnabledTimeTrackFilter
		{
			get { return _isEnabledTimeTrackFilter; }
			set
			{
				if (_isEnabledTimeTrackFilter == value) return;

				_isEnabledTimeTrackFilter = value;
				OnPropertyChanged(() => IsEnabledTimeTrackFilter);
			}
		}

		public bool IsBisy
		{
			get { return _isBisy; }
			set
			{
				if (_isBisy == value) return;
				_isBisy = value;
				OnPropertyChanged(() => IsBisy);
			}
		}

		public bool IsFilterAccepted
		{
			get { return _isFilterAccepted; }
			set
			{
				_isFilterAccepted = value;
				OnPropertyChanged(() => IsFilterAccepted);
			}
		}

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

		public int PageNumber
		{
			get { return _pageNumber; }
			set
			{
				_pageNumber = value;
				OnPropertyChanged(() => PageNumber);
			}
		}

		
		public bool CanShowDocumentTypes
		{
			get { return _canShowDocumentTypes; }
			set
			{
				if (_canShowDocumentTypes == value) return;
				_canShowDocumentTypes = value;
				OnPropertyChanged(() => CanShowDocumentTypes);
			}
		}

		public int TotalPageNumber
		{
			get { return (_totalPageNumber != null && _totalPageNumber.Value != 0) ? _totalPageNumber.Value : InitialPageNumber; }
		}


		public TimeTrack SelectedTimeTrack
		{
			get { return _selectedTimeTrack; }
			set
			{
				_selectedTimeTrack = value;
				OnPropertyChanged(() => SelectedTimeTrack);
			}
		}

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
			_pageNumber = InitialPageNumber;
			SubscribeOnEvents();

			ShowFilterCommand = new RelayCommand(OnShowFilter, () => !IsBisy);
			PrintCommand = new RelayCommand(OnPrint, () => ApplicationService.IsReportEnabled);
			CanShowDocumentTypes = FiresecManager.CheckPermission(StrazhAPI.Models.PermissionType.Oper_SKD_TimeTrack_DocumentTypes_Edit);

			ShowDocumentTypesCommand = new ReactiveCommand();
			ShowDocumentTypesCommand.Subscribe(_ => OnShowDocumentTypes());

			_timeTrackFilter = CreateTimeTrackFilter();
			UpdateGrid();

			var canSelectPreviousPage = this.WhenAny(x => x.PageNumber, x => (x.Value > InitialPageNumber));
			PreviousPageCommand = new ReactiveAsyncCommand(canSelectPreviousPage);
			PreviousPageCommand.RegisterAsyncAction(_ => PageNumber--);

			FirstPageCommand = new ReactiveAsyncCommand(canSelectPreviousPage);
			FirstPageCommand.RegisterAsyncAction(_ => PageNumber = InitialPageNumber);

			var canSelectNextPage = this.ObservableForProperty(x => x.TotalPageNumber).Select(x => x.Value > PageNumber)
				.Merge(this.WhenAny(x => x.PageNumber, x => (_cachedTimeTracks != null && (x.Value < TotalPageNumber))));
			NextPageCommand = new ReactiveAsyncCommand(canSelectNextPage);
			NextPageCommand.RegisterAsyncAction(_ => PageNumber++);

			LastPageCommand = new ReactiveAsyncCommand(canSelectNextPage);
			LastPageCommand.RegisterAsyncAction(_ =>
			{
				if (_cachedTimeTracks == null) return;
				PageNumber = TotalPageNumber;
			});

			RefreshCommand = new ReactiveAsyncCommand();
			RefreshCommand.Subscribe(_ => IsBisy = true);
			RefreshCommand.RegisterAsyncAction(_ => UpdateGrid()).Subscribe(x => { IsBisy = false; });

			var executeUpdateTotalPageCounterCommand = new ReactiveAsyncCommand();

			var executeSearchCommand = new ReactiveAsyncCommand();
			executeSearchCommand
				//.ObserveOn(RxApp.TaskpoolScheduler)
				.Subscribe(x =>
				{
					if (SelectedTimeTrack == null && SearchResults == null) return;
					SelectedTimeTrack = SelectedTimeTrack ?? SearchResults.FirstOrDefault();

				});

			var resultsTotalPages =
				executeUpdateTotalPageCounterCommand.RegisterAsyncFunction(
					x => _cachedTimeTracks != null ? ((_cachedTimeTracks.Count - 1)/RecordsPerPage + 1) - 1 : InitialPageNumber);
			_executeUpdateTotalPageCounterCommand = executeUpdateTotalPageCounterCommand;

			var results = executeSearchCommand.RegisterAsyncFunction(s => ExecuteSearch((int) s));
			_executeSearchCommand = executeSearchCommand;

			this.WhenAny(x => x.IsActive, x => x.PageNumber, x => x.IsFilterAccepted,
				(isActive, pageNumber, isFilterAccepted) =>
					new {IsActive = isActive.Value, PageNumber = pageNumber.Value, IsFilterAccepted = isFilterAccepted.Value})
				.Select(x => new {x.PageNumber, x.IsFilterAccepted})
				.Where(x => (x.PageNumber >= InitialPageNumber && x.PageNumber <= TotalPageNumber) || x.IsFilterAccepted)
				.Subscribe(value =>
				{
					IsBisy = true;
					_executeSearchCommand.Execute(value.PageNumber);
					_executeUpdateTotalPageCounterCommand.Execute(null);
					IsFilterAccepted = false;
					IsBisy = false;
				});

			_searchResults = new ObservableAsPropertyHelper<ObservableCollection<TimeTrack>>(results,
				_ => OnPropertyChanged(() => SearchResults));
			_totalPageNumber = new ObservableAsPropertyHelper<int>(resultsTotalPages,
				_ => OnPropertyChanged(() => TotalPageNumber));

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
			ServiceFactoryBase.Events.GetEvent<UserChangedEvent>().Unsubscribe(OnUserChanged);
			ServiceFactoryBase.Events.GetEvent<UserChangedEvent>().Subscribe(OnUserChanged);
			ServiceFactoryBase.Events.GetEvent<EditDocumentEvent>().Unsubscribe(OnEditDocument);
			ServiceFactoryBase.Events.GetEvent<EditDocumentEvent>().Subscribe(OnEditDocument);
			ServiceFactoryBase.Events.GetEvent<RemoveDocumentEvent>().Unsubscribe(OnRemoveDocument);
			ServiceFactoryBase.Events.GetEvent<RemoveDocumentEvent>().Subscribe(OnRemoveDocument);
			ServiceFactoryBase.Events.GetEvent<EditTimeTrackPartEvent>().Unsubscribe(OnEditTimeTrackPart);
			ServiceFactoryBase.Events.GetEvent<EditTimeTrackPartEvent>().Subscribe(OnEditTimeTrackPart);
		}

		private ObservableCollection<TimeTrack> ExecuteSearch(int pageNumber)
		{
			var pageNum = pageNumber - 1; //Так как номер страницы начинается с единицы, то необходимо вычесть её, при осуществлении поиска записей.
			return _cachedTimeTracks == null
				? null
				: new ObservableCollection<TimeTrack>(_cachedTimeTracks.Skip(pageNum * RecordsPerPage).Take(RecordsPerPage));
		}

		private static TimeTrackFilter CreateTimeTrackFilter()
		{
			return new TimeTrackFilter
			{
				EmployeeFilter = new EmployeeFilter
				{
					OrganisationUIDs = new List<Guid> { GetFirstOrganizationUID() },
					UserUID = FiresecManager.CurrentUser.UID,
				},
				Period = TimeTrackingPeriod.CurrentMonth,
				OrganisationUIDs = new List<Guid> { GetFirstOrganizationUID() },
				StartDate = GetFirstDayOfMonth(),
				EndDate = DateTime.Today
			};
		}

		public static DateTime GetFirstDayOfMonth() //TODO: Implement to static extension class
		{
			return DateTime.Today.AddDays(1 - DateTime.Today.Day);
		}

		private static Guid GetFirstOrganizationUID()
		{
			var resp = OrganisationHelper.GetByCurrentUser();

			if (resp == null) return Guid.Empty;

			var firstOrganizationElement = resp.FirstOrDefault();
			return firstOrganizationElement != null ? firstOrganizationElement.UID : Guid.Empty;
		}

		void UpdateGrid()
		{
			TotalDays = (int) (_timeTrackFilter.EndDate - _timeTrackFilter.StartDate).TotalDays + 1;
			FirstDay = _timeTrackFilter.StartDate;
			IsEnabledTimeTrackFilter = _timeTrackFilter.TotalTimeTrackTypeFilters.Any();

			var timeTrackResult = GetServerTimeTrackResults();

			if (timeTrackResult == null) return;

			_timeTrackEmployeeResults = timeTrackResult.TimeTrackEmployeeResults;
			_cachedTimeTracks =
				_timeTrackEmployeeResults.Select(x => new TimeTrack(_timeTrackFilter, x))
					.OrderBy(x => x.ShortEmployee.FIO)
					.ToList();

			if (_executeSearchCommand != null)
				_executeSearchCommand.Execute(PageNumber);

			RowHeight = 60 + 20 * _timeTrackFilter.TotalTimeTrackTypeFilters.Count;
		}

		private TimeTrackResult GetServerTimeTrackResults()
		{
			var resultFileName = Path.Combine(AppDataFolderHelper.GetFolder("Temp"), "ClientTimeTrackResult.xml");

			if (!Directory.Exists(resultFileName))
				Directory.CreateDirectory(AppDataFolderHelper.GetFolder("Temp"));

			TimeTrackResult timeTrackResult;
			lock (Locker)
			{
				using (var fileStream = new FileStream(resultFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
				{
					using (var stream = FiresecManager.FiresecService.GetTimeTracksStream(_timeTrackFilter.EmployeeFilter, _timeTrackFilter.StartDate, _timeTrackFilter.EndDate))
					{
						FiresecManager.CopyStream(stream, fileStream);
					}
				}
				timeTrackResult = Deserialize(resultFileName);
			}

			return timeTrackResult;
		}

		public static TimeTrackResult Deserialize(string fileName)
		{
			using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None))
			{
				var dataContractSerializer = new DataContractSerializer(typeof(TimeTrackResult));
				var result = (TimeTrackResult)dataContractSerializer.ReadObject(fileStream);
				return result;
			}
		}

		#endregion

		#region Commands

		private readonly ICommand _executeSearchCommand;
		private readonly ICommand _executeUpdateTotalPageCounterCommand;

		public ReactiveAsyncCommand LastPageCommand { get; private set; }

		public ReactiveAsyncCommand FirstPageCommand { get; private set; }

		public ReactiveAsyncCommand PreviousPageCommand { get; private set; }

		public ReactiveAsyncCommand NextPageCommand { get; private set; }

		public RelayCommand ShowFilterCommand { get; private set; }
		void OnShowFilter()
		{
			var filterViewModel = new TimeTrackFilterViewModel(_timeTrackFilter);
			if (DialogService.ShowModalWindow(filterViewModel))
			{
				IsBisy = true;
				Task.Factory.StartNew(UpdateGrid).ContinueWith(_ =>
				{
					IsBisy = false;
					PageNumber = default(int);
					IsFilterAccepted = true;
				}, TaskContinuationOptions.OnlyOnRanToCompletion);
			}
		}

		public ReactiveAsyncCommand RefreshCommand { get; private set; }

		public RelayCommand PrintCommand { get; private set; }
		void OnPrint()
		{
			if (_cachedTimeTracks.Count == 0)
			{
				MessageBoxService.ShowWarning("В отчете нет ни одного сотрудника");
				return;
			}
			var organisationUIDs = new HashSet<Guid>();
			var departmentNames = new HashSet<string>();
			foreach (var timeTrack in _cachedTimeTracks)
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

		public ReactiveCommand ShowDocumentTypesCommand { get; private set; }
		void OnShowDocumentTypes()
		{
			var documentTypesViewModel = new DocumentTypesViewModel();
			DialogService.ShowModalWindow(documentTypesViewModel);
		}

		#endregion

		#region DocumentEvents
		void OnEditDocument(TimeTrackDocument document)
		{
			var timeTrackViewModel = _cachedTimeTracks.FirstOrDefault(x => x.ShortEmployee.UID == document.EmployeeUID);
			if (timeTrackViewModel != null)
			{
				timeTrackViewModel.DocumentsViewModel.OnEditDocument(document);
			}
		}

		void OnRemoveDocument(TimeTrackDocument document)
		{
			var timeTrackViewModel = _cachedTimeTracks.FirstOrDefault(x => x.ShortEmployee.UID == document.EmployeeUID);
			if (timeTrackViewModel != null)
			{
				timeTrackViewModel.DocumentsViewModel.OnRemoveDocument(document);
			}
		}

		void OnEditTimeTrackPart(Guid uid)
		{
			var timeTrackViewModel = _cachedTimeTracks.FirstOrDefault(x => x.ShortEmployee.UID == uid);
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