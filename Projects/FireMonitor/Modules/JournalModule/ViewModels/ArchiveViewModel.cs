using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Common;
using StrazhAPI.Journal;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrastructure.Models;
using JournalModule.Events;
using Infrastructure.Common.Services.Layout;

namespace JournalModule.ViewModels
{
	public class ArchiveViewModel : ViewPartViewModel, ILayoutPartContent
	{
		public static DateTime ArchiveFirstDate { get; private set; }
		public ArchiveFilter ArchiveFilter { get; private set; }
		private Guid ArchivePortionUID;
		private LayoutPartContainerCollection _container;

		public ArchiveViewModel()
		{
			_container = new LayoutPartContainerCollection();
			ShowFilterCommand = new RelayCommand(OnShowFilter);
			RefreshCommand = new RelayCommand(OnRefresh);
			ShowSettingsCommand = new RelayCommand(OnShowSettings);
			FirstPageCommand = new RelayCommand(OnFirstPage, CanFirstPage);
			PreviousPageCommand = new RelayCommand(OnPreviousPage, CanPreviousPage);
			NextPageCommand = new RelayCommand(OnNextPage, CanNextPage);
			LastPageCommand = new RelayCommand(OnLastPage, CanLastPage);
			Pages = new ObservableCollection<ArchivePageViewModel>();

			ServiceFactoryBase.Events.GetEvent<JournalSettingsUpdatedEvent>().Unsubscribe(OnSettingsChanged);
			ServiceFactoryBase.Events.GetEvent<JournalSettingsUpdatedEvent>().Subscribe(OnSettingsChanged);
			ServiceFactoryBase.Events.GetEvent<GetFilteredArchiveCompletedEvent>().Subscribe(OnGetFilteredArchiveCompleted);
			ServiceFactoryBase.Events.GetEvent<JournalSettingsUpdatedEvent>().Unsubscribe(OnSettingsChanged);
			ServiceFactoryBase.Events.GetEvent<JournalSettingsUpdatedEvent>().Subscribe(OnSettingsChanged);
		}

		public void Initialize()
		{
			var result = FiresecManager.FiresecService.GetMinJournalDateTime();
			ArchiveFirstDate = result == null || result.HasError
							? DateTime.Now.AddYears(-10)
							: result.Result;
		}

		ObservableCollection<ArchivePageViewModel> _pages;
		public ObservableCollection<ArchivePageViewModel> Pages
		{
			get { return _pages; }
			set
			{
				_pages = value;
				OnPropertyChanged(() => Pages);
			}
		}

		ArchivePageViewModel _selectedPage;
		public ArchivePageViewModel SelectedPage
		{
			get { return _selectedPage; }
			set
			{
				_selectedPage = value;
				OnPropertyChanged(() => SelectedPage);
			}
		}

		public void Sort(ShowArchiveEventArgs showArchiveEventArgs)
		{
			ArchiveFilter = new ArchiveFilter();
			ArchiveFilter.PageSize = ClientSettings.ArchiveDefaultState.PageSize;
			ArchiveFilter.StartDate = DateTime.Now.AddDays(-7);
			ClientSettings.ArchiveDefaultState.ArchiveDefaultStateType = ArchiveDefaultStateType.LastDays;
			ClientSettings.ArchiveDefaultState.Count = 7;

			if (showArchiveEventArgs.SKDDevice != null)
				ArchiveFilter.ObjectUIDs.Add(showArchiveEventArgs.SKDDevice.UID);
			if (showArchiveEventArgs.SKDZone != null)
				ArchiveFilter.ObjectUIDs.Add(showArchiveEventArgs.SKDZone.UID);
			if (showArchiveEventArgs.SKDDoor != null)
				ArchiveFilter.ObjectUIDs.Add(showArchiveEventArgs.SKDDoor.UID);

			Update();
		}

		ObservableCollection<JournalItemViewModel> _journalItems;
		public ObservableCollection<JournalItemViewModel> JournalItems
		{
			get { return _journalItems; }
			private set
			{
				_journalItems = value;
				OnPropertyChanged(() => JournalItems);
			}
		}

		JournalItemViewModel _selectedJournal;
		public JournalItemViewModel SelectedJournal
		{
			get { return _selectedJournal; }
			set
			{
				_selectedJournal = value;
				OnPropertyChanged(() => SelectedJournal);
			}
		}

		public RelayCommand ShowFilterCommand { get; private set; }
		void OnShowFilter()
		{
			var archiveFilterViewModel = new ArchiveFilterViewModel(ArchiveFilter);

			if (DialogService.ShowModalWindow(archiveFilterViewModel))
			{
				ArchiveFilter = archiveFilterViewModel.GetModel();
				Update();
			}
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			Update();
		}

		public RelayCommand ShowSettingsCommand { get; private set; }
		void OnShowSettings()
		{
			try
			{
				var archiveSettingsViewModel = new ArchiveSettingsViewModel();
				if (DialogService.ShowModalWindow(archiveSettingsViewModel))
				{
					ServiceFactory.Events.GetEvent<JournalSettingsUpdatedEvent>().Publish(null);
					Update();
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове ArchiveViewModel.ShowSettingsCommand");
				MessageBoxService.ShowException(e);
			}
		}

		string _status;
		public string Status
		{
			get { return _status; }
			set
			{
				_status = value;
				OnPropertyChanged(() => Status);
				OnPropertyChanged(() => IsLoading);
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
				OnPropertyChanged(() => TotalPageNumber);
				OnPropertyChanged(() => HasPages);
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
				OnPropertyChanged(() => CurrentPageNumber);

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
			get { return Status == Resources.Language.ArchiveViewModel.Update_Status; }
		}

		public void Update()
		{
			Status = Resources.Language.ArchiveViewModel.Update_Status;
			JournalItems = new ObservableRangeCollection<JournalItemViewModel>();

			Pages = new ObservableCollection<ArchivePageViewModel>();
			TotalPageNumber = 0;
			CurrentPageNumber = 0;
			SelectedPage = null;

			try
			{
				if (ArchiveFilter == null)
					ArchiveFilter = new ArchiveFilter();

				ArchiveFilter.PageSize = ClientSettings.ArchiveDefaultState.PageSize;
				ArchiveFilter.UseDeviceDateTime = ClientSettings.ArchiveDefaultState.UseDeviceDateTime;
				ArchiveFilter.StartDate = ArchiveFirstDate;
				ArchiveFilter.EndDate = DateTime.Now;

				switch (ClientSettings.ArchiveDefaultState.ArchiveDefaultStateType)
				{
					case ArchiveDefaultStateType.LastHours:
						ArchiveFilter.StartDate = ArchiveFilter.EndDate.AddHours(-ClientSettings.ArchiveDefaultState.Count);
						break;

					case ArchiveDefaultStateType.LastDays:
						ArchiveFilter.StartDate = ArchiveFilter.EndDate.AddDays(-ClientSettings.ArchiveDefaultState.Count);
						break;

					case ArchiveDefaultStateType.FromDate:
						ArchiveFilter.StartDate = ClientSettings.ArchiveDefaultState.StartDate;
						break;

					case ArchiveDefaultStateType.RangeDate:
						ArchiveFilter.StartDate = ClientSettings.ArchiveDefaultState.StartDate;
						ArchiveFilter.EndDate = ClientSettings.ArchiveDefaultState.EndDate;
						break;
				}

				JournalItems = new ObservableCollection<JournalItemViewModel>();
				ArchivePortionUID = Guid.NewGuid();
				FiresecManager.FiresecService.BeginGetFilteredArchive(ArchiveFilter, ArchivePortionUID);
			}
			catch (ThreadAbortException) { }
			catch (Exception e)
			{
				Logger.Error(e, "ArchiveViewModel.OnUpdate");
			}
		}

		void OnGetFilteredArchiveCompleted(ArchiveResult archiveResult)
		{
			if (archiveResult.ArchivePortionUID == ArchivePortionUID)
			{
				var archivePageViewModel = new ArchivePageViewModel(archiveResult.JournalItems);
				Pages.Add(archivePageViewModel);
				TotalPageNumber = Pages.Count;
				if (CurrentPageNumber == 0)
					CurrentPageNumber = 1;
				//Status = "Количество записей: " + ((TotalPageNumber - 1) * ClientSettings.ArchiveDefaultState.PageSize + archiveResult.JournalItems.Count()).ToString();
			    Status = string.Format(Resources.Language.ArchiveViewModel.OnGetFilteredArchiveCompleted_Status,
			        ((TotalPageNumber - 1)*ClientSettings.ArchiveDefaultState.PageSize + archiveResult.JournalItems.Count()));
			}
		}

		public List<JournalColumnType> AdditionalColumns
		{
			get { return ClientSettings.ArchiveDefaultState.AdditionalJournalColumnTypes; }
		}

		bool additionalColumnsChanged;
		public bool AdditionalColumnsChanged
		{
			get { return additionalColumnsChanged; }
			set
			{
				additionalColumnsChanged = value;
				OnPropertyChanged(() => AdditionalColumnsChanged);
			}
		}

		void OnSettingsChanged(object o)
		{
			AdditionalColumnsChanged = !AdditionalColumnsChanged;
		}

		#region ILayoutPartContent Members

		public ILayoutPartContainer Container
		{
			get { return _container; }
		}

		public void SetLayoutPartContainer(ILayoutPartContainer container)
		{
			_container.Add(container);
		}

		#endregion
	}
}