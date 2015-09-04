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
using Infrastructure.Common.Services.Layout;
using System.Diagnostics;

namespace JournalModule.ViewModels
{
	public class ArchiveViewModel : ViewPartViewModel, ILayoutPartContent
	{
		public static DateTime ArchiveFirstDate { get; private set; }
		public ArchiveFilter ArchiveFilter { get; private set; }
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
			
			ServiceFactory.Events.GetEvent<JournalSettingsUpdatedEvent>().Unsubscribe(OnSettingsChanged);
			ServiceFactory.Events.GetEvent<JournalSettingsUpdatedEvent>().Subscribe(OnSettingsChanged);
			ServiceFactory.Events.GetEvent<JournalSettingsUpdatedEvent>().Unsubscribe(OnSettingsChanged);
			ServiceFactory.Events.GetEvent<JournalSettingsUpdatedEvent>().Subscribe(OnSettingsChanged);
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
		}

		public void Sort(ShowArchiveEventArgs showArchiveEventArgs)
		{
			ArchiveFilter = new ArchiveFilter();
			ArchiveFilter.PageSize = ClientSettings.ArchiveDefaultState.PageSize;
			ArchiveFilter.StartDate = DateTime.Now.AddDays(-7);
			ClientSettings.ArchiveDefaultState.ArchiveDefaultStateType = ArchiveDefaultStateType.LastDays;
			ClientSettings.ArchiveDefaultState.Count = 7;

			if (showArchiveEventArgs.GKDevice != null)
				ArchiveFilter.ObjectUIDs.Add(showArchiveEventArgs.GKDevice.UID);
			if (showArchiveEventArgs.GKZone != null)
				ArchiveFilter.ObjectUIDs.Add(showArchiveEventArgs.GKZone.UID);
			if (showArchiveEventArgs.GKDirection != null)
				ArchiveFilter.ObjectUIDs.Add(showArchiveEventArgs.GKDirection.UID);
			if (showArchiveEventArgs.GKDelay != null)
				ArchiveFilter.ObjectUIDs.Add(showArchiveEventArgs.GKDelay.UID);
			if (showArchiveEventArgs.GKPim != null)
				ArchiveFilter.ObjectUIDs.Add(showArchiveEventArgs.GKPim.UID);
			if (showArchiveEventArgs.GKPumpStation != null)
				ArchiveFilter.ObjectUIDs.Add(showArchiveEventArgs.GKPumpStation.UID);
			if (showArchiveEventArgs.GKMPT != null)
				ArchiveFilter.ObjectUIDs.Add(showArchiveEventArgs.GKMPT.UID);
			if (showArchiveEventArgs.GKDelay != null)
				ArchiveFilter.ObjectUIDs.Add(showArchiveEventArgs.GKDelay.UID);
			if (showArchiveEventArgs.GKGuardZone != null)
				ArchiveFilter.ObjectUIDs.Add(showArchiveEventArgs.GKGuardZone.UID);
			if (showArchiveEventArgs.GKSKDZone != null)
				ArchiveFilter.ObjectUIDs.Add(showArchiveEventArgs.GKSKDZone.UID);
			if (showArchiveEventArgs.GKDoor != null)
				ArchiveFilter.ObjectUIDs.Add(showArchiveEventArgs.GKDoor.UID);
			if (showArchiveEventArgs.Camera != null)
				ArchiveFilter.ObjectUIDs.Add(showArchiveEventArgs.Camera.UID);
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
					Update();
				}
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
					_currentPageNumber = 1;
				if (value > TotalPageNumber)
					_currentPageNumber = TotalPageNumber;
				else
					_currentPageNumber = value;
				OnPropertyChanged(() => CurrentPageNumber);
				GetJournalItems();
			}
		}

		public void Update()
		{
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

				var countResult = FiresecManager.FiresecService.GetArchiveCount(ArchiveFilter);
				if(!countResult.HasError)
				{
					TotalPageNumber = countResult.Result / ArchiveFilter.PageSize + 1;
					CurrentPageNumber = 1;
				}
			}
			catch (ThreadAbortException) { }
			catch (Exception e)
			{
				Logger.Error(e, "ArchiveViewModel.OnUpdate");
			}
		}

		void GetJournalItems()
		{
			var journalItemsResult = FiresecManager.FiresecService.GetArchivePage(ArchiveFilter, CurrentPageNumber);
			if (!journalItemsResult.HasError)
			{
				JournalItems = new ObservableRangeCollection<JournalItemViewModel>();
				foreach (var item in journalItemsResult.Result)
				{
					JournalItems.Add(new JournalItemViewModel(item));
				}
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