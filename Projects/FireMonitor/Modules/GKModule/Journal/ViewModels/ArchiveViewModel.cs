using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Common;
using FiresecClient;
using GKModule.Events;
using GKProcessor;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrastructure.Models;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class ArchiveViewModel : ViewPartViewModel
	{
		public static DateTime ArchiveFirstDate { get; private set; }
		public ArchiveDefaultState ArchiveDefaultState;
		XArchiveFilter ArchiveFilter;
		Thread UpdateThread;
		bool FirstTime = true;

		public ArchiveViewModel()
		{
			ShowFilterCommand = new RelayCommand(OnShowFilter);
			ShowSettingsCommand = new RelayCommand(OnShowSettings);
			FirstPageCommand = new RelayCommand(OnFirstPage, CanFirstPage);
			PreviousPageCommand = new RelayCommand(OnPreviousPage, CanPreviousPage);
			NextPageCommand = new RelayCommand(OnNextPage, CanNextPage);
			LastPageCommand = new RelayCommand(OnLastPage, CanLastPage);
			Pages = new ObservableCollection<ArchivePageViewModel>();

			ServiceFactory.Events.GetEvent<XJournalSettingsUpdatedEvent>().Unsubscribe(OnSettingsChanged);
			ServiceFactory.Events.GetEvent<XJournalSettingsUpdatedEvent>().Subscribe(OnSettingsChanged);
			ArchiveDefaultState = ClientSettings.ArchiveDefaultState;
			if (ArchiveDefaultState == null)
				ArchiveDefaultState = new ArchiveDefaultState();

			ServiceFactory.Events.GetEvent<GetFilteredGKArchiveCompletedEvent>().Subscribe(OnGetFilteredArchiveCompleted);
		}

		public void Initialize()
		{
			ArchiveFirstDate = DateTime.Now.AddDays(-1);
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

		public void Sort(ShowXArchiveEventArgs showXArchiveEventArgs)
		{
			ArchiveFilter = new XArchiveFilter();
			ArchiveFilter.StartDate = DateTime.Now.AddDays(-7);
			if (showXArchiveEventArgs.Device != null)
				ArchiveFilter.DeviceUIDs.Add(showXArchiveEventArgs.Device.UID);
			if (showXArchiveEventArgs.Zone != null)
				ArchiveFilter.ZoneUIDs.Add(showXArchiveEventArgs.Zone.UID);
			if (showXArchiveEventArgs.Direction != null)
				ArchiveFilter.DirectionUIDs.Add(showXArchiveEventArgs.Direction.UID);
			if (showXArchiveEventArgs.Delay != null)
				ArchiveFilter.DelayUIDs.Add(showXArchiveEventArgs.Delay.UID);
			if (showXArchiveEventArgs.Pim != null)
				ArchiveFilter.PimUIDs.Add(showXArchiveEventArgs.Pim.UID);
			if (showXArchiveEventArgs.PumpStation != null)
				ArchiveFilter.PumpStationUIDs.Add(showXArchiveEventArgs.PumpStation.UID);
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
				return  _isFilterOn; 
			}
			set
			{
				_isFilterOn = value;
				OnPropertyChanged("IsFilterOn");
				Update(true);
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
					ServiceFactory.Events.GetEvent<XJournalSettingsUpdatedEvent>().Publish(null);
					if (IsFilterOn == false)
						Update(true);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове ArchiveViewModel.ShowSettingsCommand");
				MessageBoxService.ShowException(e);
			}
		}

		XArchiveFilter GerFilterFromDefaultState(ArchiveDefaultState archiveDefaultState)
		{
			var archiveFilter = new XArchiveFilter()
			{
				StartDate = ArchiveFirstDate,
				EndDate = DateTime.Now
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

		public void Update(bool abortRunnig = true)
		{
			if (abortRunnig)
			{
				if (UpdateThread != null)
					UpdateThread.Abort();
				UpdateThread = null;
			}
			if (UpdateThread == null)
			{
				Status = "Загрузка данных";
				JournalItems = new ObservableRangeCollection<JournalItemViewModel>();

				Pages = new ObservableCollection<ArchivePageViewModel>();
				TotalPageNumber = 0;
				CurrentPageNumber = 0;
				SelectedPage = null;

				UpdateThread = new Thread(new ThreadStart(OnUpdate));
				UpdateThread.Name = "GK Journal Update";
				UpdateThread.Start();
			}
		}

		void OnUpdate()
		{
			try
			{
				XArchiveFilter archiveFilter = null;
				if (IsFilterOn)
					archiveFilter = ArchiveFilter;
				else
					archiveFilter = GerFilterFromDefaultState(ArchiveDefaultState);

				//if (GlobalSettingsHelper.GlobalSettings.IsGKAsAService)
				{
					JournalItems = new ObservableCollection<JournalItemViewModel>();
					FiresecManager.FiresecService.BeginGetGKFilteredArchive(archiveFilter);
				}
				//else
				//{
				//    GKDBHelper.IsAbort = false;
				//    var journalItems = GKDBHelper.BeginGetGKFilteredArchive(archiveFilter, false);
				//    Dispatcher.BeginInvoke(new Action(() => { OnGetFilteredArchiveCompleted(journalItems); }));
				//}
			}
			catch (ThreadAbortException) { }
			catch (Exception e)
			{
				Logger.Error(e, "ArchiveViewModel.OnUpdate");
			}
			UpdateThread = null;
		}

		void OnGetFilteredArchiveCompleted(IEnumerable<JournalItem> journalItems)
		{
			var archivePageViewModel = new ArchivePageViewModel(journalItems);
			Pages.Add(archivePageViewModel);
			TotalPageNumber = Pages.Count;
			if (CurrentPageNumber == 0)
				CurrentPageNumber = 1;
			Status = "Количество записей: " + ((TotalPageNumber - 1) * 100 + journalItems.Count()).ToString();
		}

		public override void OnShow()
		{
			if (FirstTime)
			{
				FirstTime = false;
				Update(false);
			}
		}

		void OnSettingsChanged(object o)
		{
			AdditionalColumnsChanged = !AdditionalColumnsChanged; 
		}
	}
}