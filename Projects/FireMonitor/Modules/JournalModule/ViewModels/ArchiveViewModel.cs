using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Common;
using RubezhAPI.Journal;
using RubezhClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrastructure.Models;
using JournalModule.Events;
using Infrastructure.Common.Services.Layout;
using System.Diagnostics;
using System.Windows.Threading;
using RubezhAPI;

namespace JournalModule.ViewModels
{
	public class ArchiveViewModel : ViewPartViewModel, ILayoutPartContent
	{
		public static DateTime ArchiveFirstDate { get; private set; }
		public JournalFilter Filter { get; private set; }
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
			IsVisibleBottomPanel = true;
			ServiceFactory.Events.GetEvent<JournalSettingsUpdatedEvent>().Unsubscribe(OnSettingsChanged);
			ServiceFactory.Events.GetEvent<JournalSettingsUpdatedEvent>().Subscribe(OnSettingsChanged);
			SafeFiresecService.CallbackOperationResultEvent -= OnCallbackOperationResult;
			SafeFiresecService.CallbackOperationResultEvent += OnCallbackOperationResult;
		}

		void OnCallbackOperationResult(CallbackOperationResult callbackOperationResult)
		{
			if(callbackOperationResult.CallbackOperationResultType == CallbackOperationResultType.GetArchivePage)
			{
				ApplicationService.BeginInvoke(() =>
				{
					JournalItems = new ObservableCollection<JournalItemViewModel>();
					foreach (var item in callbackOperationResult.JournalItems)
					{
						JournalItems.Add(new JournalItemViewModel(item));
					}
					IsLoading = false;
				});
			}
		}

		public void Initialize()
		{
			var result = ClientManager.FiresecService.GetMinJournalDateTime();
			if (!result.HasError)
			{
				ArchiveFirstDate = result.Result;
			}
			else
			{
				ArchiveFirstDate = DateTime.Now.AddYears(-10);
			}
		}

		public void Sort(List<Guid> objectUIDs)
		{
			Filter = new JournalFilter();
			Filter.PageSize = ClientSettings.ArchiveDefaultState.PageSize;
			Filter.StartDate = DateTime.Now.AddDays(-7);
			ClientSettings.ArchiveDefaultState.ArchiveDefaultStateType = ArchiveDefaultStateType.LastDays;
			ClientSettings.ArchiveDefaultState.Count = 7;

			Filter.ObjectUIDs.AddRange(objectUIDs);
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
				archiveFilterViewModel = new ArchiveFilterViewModel(Filter, true);
			});

			if (result)
			{
				if (DialogService.ShowModalWindow(archiveFilterViewModel))
				{
					Filter = archiveFilterViewModel.GetModel();
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
			return CurrentPageNumber > 1 && !IsLoading;
		}

		public RelayCommand PreviousPageCommand { get; private set; }
		void OnPreviousPage()
		{
			CurrentPageNumber--;
		}
		bool CanPreviousPage()
		{
			return CurrentPageNumber > 1 && !IsLoading;
		}

		public RelayCommand NextPageCommand { get; private set; }
		void OnNextPage()
		{
			CurrentPageNumber++;
		}
		bool CanNextPage()
		{
			return CurrentPageNumber < TotalPageNumber && !IsLoading;
		}

		public RelayCommand LastPageCommand { get; private set; }
		void OnLastPage()
		{
			CurrentPageNumber = TotalPageNumber;
		}
		bool CanLastPage()
		{
			return CurrentPageNumber < TotalPageNumber && !IsLoading;
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
				JournalItems = new ObservableCollection<JournalItemViewModel>();
				if (value < 1)
					_currentPageNumber = 1;
				if (value > TotalPageNumber)
					_currentPageNumber = TotalPageNumber;
				else
					_currentPageNumber = value;
				OnPropertyChanged(() => CurrentPageNumber);
				QueryPage(_currentPageNumber);
			}
		}

		bool _isLoading;
		public bool IsLoading 
		{
			get { return _isLoading; } 
			private set
			{
				_isLoading = value;
				OnPropertyChanged(() => IsLoading);
			}
		}

		public void Update()
		{
			try
			{
				if (Filter == null)
					Filter = new JournalFilter();

				Filter.PageSize = ClientSettings.ArchiveDefaultState.PageSize;
				Filter.UseDeviceDateTime = ClientSettings.ArchiveDefaultState.UseDeviceDateTime;
				Filter.IsSortAsc = ClientSettings.ArchiveDefaultState.IsSortAsc;
				Filter.StartDate = ArchiveFirstDate;
				Filter.EndDate = DateTime.Now;

				switch (ClientSettings.ArchiveDefaultState.ArchiveDefaultStateType)
				{
					case ArchiveDefaultStateType.LastHours:
						Filter.StartDate = Filter.EndDate.AddHours(-ClientSettings.ArchiveDefaultState.Count);
						break;

					case ArchiveDefaultStateType.LastDays:
						Filter.StartDate = Filter.EndDate.AddDays(-ClientSettings.ArchiveDefaultState.Count);
						break;

					case ArchiveDefaultStateType.FromDate:
						Filter.StartDate = ClientSettings.ArchiveDefaultState.StartDate;
						break;

					case ArchiveDefaultStateType.RangeDate:
						Filter.StartDate = ClientSettings.ArchiveDefaultState.StartDate;
						Filter.EndDate = ClientSettings.ArchiveDefaultState.EndDate;
						break;
				}

				var countResult = ClientManager.FiresecService.GetArchiveCount(Filter);
				if (!countResult.HasError)
				{
					TotalPageNumber = countResult.Result / Filter.PageSize + 1;
					CurrentPageNumber = 1;
				}
			}
			catch (ThreadAbortException) { }
			catch (Exception e)
			{
				Logger.Error(e, "ArchiveViewModel.OnUpdate");
			}
		}

		void QueryPage(int pageNo)
		{
			var result = ClientManager.FiresecService.BeginGetArchivePage(Filter, pageNo);
			if (result.HasError)
			{
				MessageBoxService.Show(result.Error);
				return;
			}
			IsLoading = true;
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

		public static bool IsVisibleBottomPanel { get; set; }
	}
}