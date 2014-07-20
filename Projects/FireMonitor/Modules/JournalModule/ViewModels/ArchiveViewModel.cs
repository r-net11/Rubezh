using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Common;
using FiresecAPI.Journal;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrastructure.Models;
using JournalModule.Events;

namespace JournalModule.ViewModels
{
	public class ArchiveViewModel : ViewPartViewModel
	{
		public static DateTime ArchiveFirstDate { get; private set; }
		public ArchiveDefaultState ArchiveDefaultState;
		ArchiveFilter ArchiveFilter;
		bool FirstTime = true;
		Guid ArchivePortionUID;

		public ArchiveViewModel()
		{
			ShowFilterCommand = new RelayCommand(OnShowFilter);
			ShowSettingsCommand = new RelayCommand(OnShowSettings);
			FirstPageCommand = new RelayCommand(OnFirstPage, CanFirstPage);
			PreviousPageCommand = new RelayCommand(OnPreviousPage, CanPreviousPage);
			NextPageCommand = new RelayCommand(OnNextPage, CanNextPage);
			LastPageCommand = new RelayCommand(OnLastPage, CanLastPage);
			Pages = new ObservableCollection<ArchivePageViewModel>();

			ServiceFactory.Events.GetEvent<SKDJournalSettingsUpdatedEvent>().Unsubscribe(OnSettingsChanged);
			ServiceFactory.Events.GetEvent<SKDJournalSettingsUpdatedEvent>().Subscribe(OnSettingsChanged);
			ArchiveDefaultState = ClientSettings.ArchiveDefaultState;
			if (ArchiveDefaultState == null)
				ArchiveDefaultState = new ArchiveDefaultState();

			ServiceFactory.Events.GetEvent<GetFilteredSKDArchiveCompletedEvent>().Subscribe(OnGetFilteredArchiveCompleted);
		}

		public void Initialize()
		{
			var result = FiresecManager.FiresecService.GetMinJournalDateTime();
			if (!result.HasError)
			{
				ArchiveFirstDate = result.Result;
			}
			else
			{
				ArchiveFirstDate = DateTime.Now.AddYears(-10);
			}
			_isFilterOn = false;
		}

		ObservableCollection<ArchivePageViewModel> _pages;
		public ObservableCollection<ArchivePageViewModel> Pages
		{
			get { return _pages; }
			set
			{
				_pages = value;
				OnPropertyChanged("Pages");
			}
		}

		ArchivePageViewModel _selectedPage;
		public ArchivePageViewModel SelectedPage
		{
			get { return _selectedPage; }
			set
			{
				_selectedPage = value;
				OnPropertyChanged("SelectedPage");
			}
		}

		public void Sort(ShowSKDArchiveEventArgs showSKDArchiveEventArgs)
		{
			ArchiveFilter = new ArchiveFilter();
			ArchiveFilter.PageSize = ClientSettings.ArchiveDefaultState.PageSize;
			ArchiveFilter.StartDate = DateTime.Now.AddDays(-7);
			if (showSKDArchiveEventArgs.Device != null)
				ArchiveFilter.ObjectUIDs.Add(showSKDArchiveEventArgs.Device.UID);
			IsFilterOn = true;
			OnPropertyChanged("IsFilterExists");
		}

		ObservableCollection<JournalItemViewModel> _journalItems;
		public ObservableCollection<JournalItemViewModel> JournalItems
		{
			get { return _journalItems; }
			private set
			{
				_journalItems = value;
				OnPropertyChanged("JournalItems");
			}
		}

		JournalItemViewModel _selectedJournal;
		public JournalItemViewModel SelectedJournal
		{
			get { return _selectedJournal; }
			set
			{
				_selectedJournal = value;
				OnPropertyChanged("SelectedJournal");
			}
		}

		bool _isFilterOn;
		public bool IsFilterOn
		{
			get
			{
				return _isFilterOn;
			}
			set
			{
				_isFilterOn = value;
				OnPropertyChanged("IsFilterOn");
				Update();
			}
		}

		public bool IsFilterExists
		{
			get { return ArchiveFilter != null; }
		}

		public RelayCommand ShowFilterCommand { get; private set; }
		void OnShowFilter()
		{
			if (ArchiveFilter == null)
				ArchiveFilter = GerFilterFromDefaultState(ArchiveDefaultState);

			ArchiveFilterViewModel archiveFilterViewModel = null;

			var result = WaitHelper.Execute(() =>
			{
				archiveFilterViewModel = new ArchiveFilterViewModel(ArchiveFilter);
			});

			if (result)
			{
				if (DialogService.ShowModalWindow(archiveFilterViewModel))
				{
					ArchiveFilter = archiveFilterViewModel.GetModel();
					OnPropertyChanged("IsFilterExists");
					IsFilterOn = true;
				}
			}
		}

		public RelayCommand ShowSettingsCommand { get; private set; }
		void OnShowSettings()
		{
			try
			{
				var archiveSettingsViewModel = new ArchiveSettingsViewModel(ArchiveDefaultState);
				if (DialogService.ShowModalWindow(archiveSettingsViewModel))
				{
					ArchiveDefaultState = archiveSettingsViewModel.ArchiveDefaultState;
					ClientSettings.ArchiveDefaultState = ArchiveDefaultState;
					ServiceFactory.Events.GetEvent<SKDJournalSettingsUpdatedEvent>().Publish(null);
					if (IsFilterOn == false)
						Update();
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове ArchiveViewModel.ShowSettingsCommand");
				MessageBoxService.ShowException(e);
			}
		}

		ArchiveFilter GerFilterFromDefaultState(ArchiveDefaultState archiveDefaultState)
		{
			var archiveFilter = new ArchiveFilter()
			{
				StartDate = ArchiveFirstDate,
				EndDate = DateTime.Now,
				PageSize = archiveDefaultState.PageSize
			};

			switch (archiveDefaultState.ArchiveDefaultStateType)
			{
				case ArchiveDefaultStateType.LastHours:
					if (archiveDefaultState.Count.HasValue)
						archiveFilter.StartDate = archiveFilter.EndDate.AddHours(-archiveDefaultState.Count.Value);
					break;

				case ArchiveDefaultStateType.LastDays:
					if (archiveDefaultState.Count.HasValue)
						archiveFilter.StartDate = archiveFilter.EndDate.AddDays(-archiveDefaultState.Count.Value);
					break;

				case ArchiveDefaultStateType.FromDate:
					if (archiveDefaultState.StartDate.HasValue)
						archiveFilter.StartDate = archiveDefaultState.StartDate.Value;
					break;

				case ArchiveDefaultStateType.RangeDate:
					if (archiveDefaultState.StartDate.HasValue)
						archiveFilter.StartDate = archiveDefaultState.StartDate.Value;
					if (archiveDefaultState.EndDate.HasValue)
						archiveFilter.EndDate = archiveDefaultState.EndDate.Value;
					break;
				case ArchiveDefaultStateType.All:
					archiveFilter.StartDate = DateTime.MinValue.AddYears(1900);
					break;
			}
			return archiveFilter;
		}

		string _status;
		public string Status
		{
			get { return _status; }
			set
			{
				_status = value;
				OnPropertyChanged("Status");
				OnPropertyChanged("IsLoading");
			}
		}

		public bool HasPages
		{
			get { return TotalPageNumber > 0; }
		}

		public RelayCommand FirstPageCommand { get; private set; }
		void OnFirstPage()
		{
			CurrentPageNumber = 1;
		}
		bool CanFirstPage()
		{
			return CurrentPageNumber > 1;
		}

		public RelayCommand PreviousPageCommand { get; private set; }
		void OnPreviousPage()
		{
			CurrentPageNumber--;
		}
		bool CanPreviousPage()
		{
			return CurrentPageNumber > 1;
		}

		public RelayCommand NextPageCommand { get; private set; }
		void OnNextPage()
		{
			CurrentPageNumber++;
		}
		bool CanNextPage()
		{
			return CurrentPageNumber < TotalPageNumber;
		}

		public RelayCommand LastPageCommand { get; private set; }
		void OnLastPage()
		{
			CurrentPageNumber = TotalPageNumber;
		}
		bool CanLastPage()
		{
			return CurrentPageNumber < TotalPageNumber;
		}

		int _totalPageNumber;
		public int TotalPageNumber
		{
			get { return _totalPageNumber; }
			set
			{
				_totalPageNumber = value;
				OnPropertyChanged("TotalPageNumber");
				OnPropertyChanged("HasPages");
			}
		}

		int _currentPageNumber;
		public int CurrentPageNumber
		{
			get { return _currentPageNumber; }
			set
			{
				if (value < 1)
					value = 1;
				if (value > Pages.Count)
					value = Pages.Count;

				_currentPageNumber = value;
				OnPropertyChanged("CurrentPageNumber");

				if (value > 0 && value <= Pages.Count)
				{
					var page = Pages[value - 1];
					page.Create();

					var journalItems = new ObservableRangeCollection<JournalItemViewModel>();
					journalItems.AddRange(page.JournalItems);
					JournalItems = journalItems;
					SelectedJournal = JournalItems.FirstOrDefault();
				}
			}
		}

		public bool IsLoading
		{
			get
			{
				return Status == "Загрузка данных";
			}
		}

		public List<JournalColumnType> AdditionalColumns
		{
			get
			{
				return ClientSettings.ArchiveDefaultState.AdditionalColumns;
			}
		}

		bool additionalColumnsChanged;
		public bool AdditionalColumnsChanged
		{
			get { return additionalColumnsChanged; }
			set
			{
				additionalColumnsChanged = value;
				OnPropertyChanged("AdditionalColumnsChanged");
			}
		}

		public void Update()
		{
			Status = "Загрузка данных";
			JournalItems = new ObservableRangeCollection<JournalItemViewModel>();

			Pages = new ObservableCollection<ArchivePageViewModel>();
			TotalPageNumber = 0;
			CurrentPageNumber = 0;
			SelectedPage = null;

			try
			{
				ArchiveFilter archiveFilter = null;
				if (IsFilterOn)
					archiveFilter = ArchiveFilter;
				else
					archiveFilter = GerFilterFromDefaultState(ArchiveDefaultState);

				archiveFilter.PageSize = ClientSettings.ArchiveDefaultState.PageSize;

				JournalItems = new ObservableCollection<JournalItemViewModel>();
				ArchivePortionUID = Guid.NewGuid();
				FiresecManager.FiresecService.BeginGetFilteredArchive(archiveFilter, ArchivePortionUID);
			}
			catch (ThreadAbortException) { }
			catch (Exception e)
			{
				Logger.Error(e, "ArchiveViewModel.OnUpdate");
			}
		}

		void OnGetFilteredArchiveCompleted(SKDArchiveResult archiveResult)
		{
			if (archiveResult.ArchivePortionUID == ArchivePortionUID)
			{
				if (archiveResult.JournalItems.Count() > 0)
				{
					var archivePageViewModel = new ArchivePageViewModel(archiveResult.JournalItems);
					Pages.Add(archivePageViewModel);
					TotalPageNumber = Pages.Count;
					if (CurrentPageNumber == 0)
						CurrentPageNumber = 1;
					Status = "Количество записей: " + ((TotalPageNumber - 1) * ArchiveDefaultState.PageSize + archiveResult.JournalItems.Count()).ToString();
				}
			}
		}

		public override void OnShow()
		{
			if (FirstTime)
			{
				FirstTime = false;
				Update();
			}
		}

		void OnSettingsChanged(object o)
		{
			AdditionalColumnsChanged = !AdditionalColumnsChanged;
		}
	}
}