using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrastructure.Models;
using System.Diagnostics;

namespace JournalModule.ViewModels
{
	public class ArchiveViewModel : ViewPartViewModel
	{
		public static DateTime ArchiveFirstDate { get; private set; }
		ArchiveDefaultState ArchiveDefaultState;
		ArchiveFilter ArchiveFilter;
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
			ServiceFactory.Events.GetEvent<GetFilteredArchiveCompletedEvent>().Subscribe(OnGetFilteredArchiveCompleted);
			//ServiceFactory.Events.GetEvent<GetFS2FilteredArchiveCompletedEvent>().Subscribe(OnGetFS2FilteredArchiveCompleted);

			ArchiveDefaultState = ClientSettings.ArchiveDefaultState;
			if (ArchiveDefaultState == null)
				ArchiveDefaultState = new ArchiveDefaultState();
		}

		public void Initialize()
		{
			var operationResult = FiresecManager.GetArchiveStartDate();
			if (operationResult.HasError == false)
			{
				ArchiveFirstDate = operationResult.Result;
				_isFilterOn = false;
			}
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

		ObservableRangeCollection<JournalRecordViewModel> _journalRecords;
		public ObservableRangeCollection<JournalRecordViewModel> JournalRecords
		{
			get { return _journalRecords; }
			private set
			{
				_journalRecords = value;
				OnPropertyChanged("JournalRecords");
			}
		}

		JournalRecordViewModel _selectedRecord;
		public JournalRecordViewModel SelectedRecord
		{
			get { return _selectedRecord; }
			set
			{
				_selectedRecord = value;
				OnPropertyChanged("SelectedRecord");
			}
		}

		bool _isFilterOn;
		public bool IsFilterOn
		{
			get { return _isFilterOn; }
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
			try
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
			catch (Exception e)
			{
				Logger.Error(e, "ArchiveViewModel.ShowSettingsCommand");
				MessageBoxService.ShowException(e);
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
					ArchiveFilter = null;
					ArchiveDefaultState = archiveSettingsViewModel.GetModel();
					ClientSettings.ArchiveDefaultState = ArchiveDefaultState;
					IsFilterOn = false;
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "ArchiveViewModel.ShowSettingsCommand");
				MessageBoxService.ShowException(e);
			}
		}

		ArchiveFilter GerFilterFromDefaultState(ArchiveDefaultState archiveDefaultState)
		{
			var archiveFilter = new ArchiveFilter()
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

					var journalRecords = new ObservableRangeCollection<JournalRecordViewModel>();
					journalRecords.AddRange(page.JournalRecords);
					JournalRecords = journalRecords;
					SelectedRecord = JournalRecords.FirstOrDefault();
				}
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
				JournalRecords = new ObservableRangeCollection<JournalRecordViewModel>();

				Pages = new ObservableCollection<ArchivePageViewModel>();
				TotalPageNumber = 0;
				CurrentPageNumber = 0;
				SelectedPage = null;

				UpdateThread = new Thread(OnUpdate);
				UpdateThread.Name = "FS1 ArchiveViewModel Update";
				UpdateThread.Start();
			}
		}

		void OnUpdate()
		{
			try
			{
				ArchiveFilter archiveFilter = null;
				if (IsFilterOn)
					archiveFilter = ArchiveFilter;
				else
					archiveFilter = GerFilterFromDefaultState(ArchiveDefaultState);

				JournalRecords = new ObservableRangeCollection<JournalRecordViewModel>();
				FiresecManager.BeginGetFilteredArchive(archiveFilter);
			}
			catch (Exception e)
			{
				Logger.Error(e, "ArchiveViewModel.OnUpdate");
			}
			UpdateThread = null;
		}

		void OnGetFilteredArchiveCompleted(IEnumerable<JournalRecord> journalRecords)
		{
			var archivePageViewModel = new ArchivePageViewModel(journalRecords);
			Pages.Add(archivePageViewModel);
			TotalPageNumber = Pages.Count;
			if (CurrentPageNumber == 0)
				CurrentPageNumber = 1;
			Status = "Количество записей: " + ((TotalPageNumber - 1) * 100 + journalRecords.Count()).ToString();
		}

		//void OnGetFS2FilteredArchiveCompleted(IEnumerable<FS2JournalItem> journalItems)
		//{
		//    var archivePageViewModel = new ArchivePageViewModel(journalItems);
		//    Pages.Add(archivePageViewModel);
		//    TotalPageNumber = Pages.Count;
		//    if (CurrentPageNumber == 0)
		//        CurrentPageNumber = 1;
		//    Status = "Количество записей: " + ((TotalPageNumber - 1) * 100 + journalItems.Count()).ToString();
		//}

		public override void OnShow()
		{
			if (FirstTime)
			{
				FirstTime = false;
				Update(false);
			}
		}
	}
}