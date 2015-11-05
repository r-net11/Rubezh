﻿using FiresecAPI.SKD;
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

// ReSharper disable once CheckNamespace
namespace SKDModule.ViewModels
{
	public class TimeTrackingViewModel : ViewPartViewModel
	{
		#region Fields

		readonly TimeTrackFilter _timeTrackFilter;
		private TimeTrack _selectedTimeTrack;
		private bool _hasSelectedTimeTrack;
		private List<TimeTrackEmployeeResult> _timeTrackEmployeeResults;
		private List<TimeTrack> _cachedTimeTracks;
		private const int RecordsPerPage = 25;
		private readonly ObservableAsPropertyHelper<ObservableCollection<TimeTrack>> _searchResults;
		private int _pageNumber;
		int _rowHeight;
		private bool _isFilterAccepted;

		#endregion

		#region Properties

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

		public int TotalPageNumber
		{
			get
			{
				if (_cachedTimeTracks == null) return 0;
				return ((_cachedTimeTracks.Count - 1) / RecordsPerPage + 1) - 1;
			}
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
			SubscribeOnEvents();

			ShowFilterCommand = new RelayCommand(OnShowFilter);
			PrintCommand = new RelayCommand(OnPrint, CanPrint);
			ShowDocumentTypesCommand = new RelayCommand(OnShowDocumentTypes);
			_timeTrackFilter = CreateTimeTrackFilter();
			UpdateGrid();

			IObservable<bool> canSelectPreviousPage = this.WhenAny(x => x.PageNumber, x => (x.Value > default(int)));
			PreviousPageCommand = new ReactiveAsyncCommand(canSelectPreviousPage);
			PreviousPageCommand.RegisterAsyncAction(_ => PageNumber--);

			FirstPageCommand = new ReactiveAsyncCommand(canSelectPreviousPage);
			FirstPageCommand.RegisterAsyncAction(_ => PageNumber = default(int));

			IObservable<bool> canSelectNextPage = this.WhenAny(x => x.PageNumber,
													x => (_cachedTimeTracks != null && (x.Value < TotalPageNumber)));
			NextPageCommand = new ReactiveAsyncCommand(canSelectNextPage);
			NextPageCommand.RegisterAsyncAction(_ => PageNumber++);

			LastPageCommand = new ReactiveAsyncCommand(canSelectNextPage);
			LastPageCommand.RegisterAsyncAction(_ =>
			{
				if (_cachedTimeTracks == null) return;
				PageNumber = TotalPageNumber;
			});

			RefreshCommand = new ReactiveAsyncCommand();
			RefreshCommand.RegisterAsyncAction(_ => UpdateGrid());

			var executeSearchCommand = new ReactiveAsyncCommand();
			executeSearchCommand.Subscribe(x =>
			{
				if (SelectedTimeTrack == null && SearchResults == null) return;
				SelectedTimeTrack = SelectedTimeTrack ?? SearchResults.FirstOrDefault();

			});
			var results = executeSearchCommand.RegisterAsyncFunction(s => ExecuteSearch((int) s));
			_executeSearchCommand = executeSearchCommand;

			this.WhenAny(x => x.IsActive, x => x.PageNumber, x => x.IsFilterAccepted,
				(isActive, pageNumber, isFilterAccepted) => new { IsActive = isActive.Value, PageNumber = pageNumber.Value, IsFilterAccepted = isFilterAccepted.Value })
			//	.Throttle(TimeSpan.FromMilliseconds(1000))
				.Select(x => new { x.PageNumber, x.IsFilterAccepted })
				.Where(x => (x.PageNumber >= default(int) && x.PageNumber <= TotalPageNumber) || x.IsFilterAccepted)
				.Subscribe(value =>
				{
					_executeSearchCommand.Execute(value.PageNumber);
					IsFilterAccepted = false;
				});

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
			return _cachedTimeTracks == null
				? null
				: new ObservableCollection<TimeTrack>(_cachedTimeTracks.Skip(pageNumber * RecordsPerPage).Take(RecordsPerPage));
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
				OrganisationUIDs = new List<Guid> { GetFirstOrganizationUID() },
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

			_timeTrackEmployeeResults = timeTrackResult.TimeTrackEmployeeResults;
			_cachedTimeTracks = _timeTrackEmployeeResults.Select(x => new TimeTrack(_timeTrackFilter, x)).OrderBy(x => x.ShortEmployee.FirstName).ToList();
			if(_executeSearchCommand != null)
			_executeSearchCommand.Execute(PageNumber);
			RowHeight = 60 + 20 * _timeTrackFilter.TotalTimeTrackTypeFilters.Count;
		}

		private TimeTrackResult GetServerTimeTrackResults()
		{
			var resultFileName = Path.Combine(AppDataFolderHelper.GetFolder("Temp"), "ClientTimeTrackResult.xml");

			if (!Directory.Exists(resultFileName)) Directory.CreateDirectory(AppDataFolderHelper.GetFolder("Temp"));

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
				UpdateGrid();
				PageNumber = default(int);
				IsFilterAccepted = true;
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