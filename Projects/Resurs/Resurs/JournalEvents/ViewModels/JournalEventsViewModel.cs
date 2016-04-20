using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using ResursDAL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class JournalEventsViewModel : BaseViewModel
	{
		public Filter Filter { get; set; }
		public JournalEventsViewModel()
		{
			ShowFilterCommand = new RelayCommand(OnShowFilter);
			RefreshCommand = new RelayCommand(OnRefresh);
			FirstPageCommand = new RelayCommand(OnFirstPage, CanFirstPage);
			PreviousPageCommand = new RelayCommand(OnPreviousPage, CanPreviousPage);
			NextPageCommand = new RelayCommand(OnNextPage, CanNextPage);
			LastPageCommand = new RelayCommand(OnLastPage, CanLastPage);
			Update();
		}

		void Update()
		{
			if (Filter == null)
				Filter = new Filter();
			else
			{
				switch (Filter.StateType)
				{
					case StateType.LastHours:
						Filter.EndDate = DateTime.Now;
						Filter.StartDate = DateTime.Now.AddHours(-Filter.Count);
						break;

					case StateType.LastDays:
						Filter.EndDate = DateTime.Now;
						Filter.StartDate = DateTime.Now.AddDays(-Filter.Count);
						break;
				}
			}

			var count = DbCache.GetJournalCount(Filter);
			if (count.HasValue)
			{
				TotalPageNumber =  count.Value / Filter.PageSize + 1;
				CurrentPageNumber = 1;
			}
		}

		public RelayCommand ShowFilterCommand { get; set; }
		void OnShowFilter()
		{
			var filterJournalViewModel = new FilterJournalViewModel(Filter);
			if(DialogService.ShowModalWindow(filterJournalViewModel))
			{
				Filter = new Filter();
				Filter.JournalTypes = filterJournalViewModel.FilterEventsViewModel.GetJournalTypes();
				Filter.ConsumerUIDs = filterJournalViewModel.FilterConsumersViewModel.GetConsumerUIDs();
				Filter.DeviceUIDs = filterJournalViewModel.FilterDevicesViewModel.GetDeviceUIDs();
				Filter.TariffUIDs = filterJournalViewModel.FilterTariffsViewModel.GetTariffUIDs();
				Filter.UserUIDs = filterJournalViewModel.FilterUsersViewModel.GetUserUIDs();
				filterJournalViewModel.DateTimeViewModel.Save(Filter);
				Update();
			}
		}

		public ObservableCollection <JournalEventViewModel> Events { get; private set; }

		JournalEventViewModel _selectedEvent;
		public JournalEventViewModel SelectedEvent 
		{
			get {return _selectedEvent;}
			set
			{
				_selectedEvent = value;
				OnPropertyChanged(() => SelectedEvent);
			}
		}

		public bool IsVisible
		{
			get { return DbCache.CheckPermission(PermissionType.ViewJournal); }
		}

		public bool HasPages
		{
			get { return TotalPageNumber > 0; }
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			Update();
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

		void GetJournalItems()
		{
			var journalItemsResult = DbCache.GetJournalPage(Filter, CurrentPageNumber);
			if (journalItemsResult != null)
			{
				Events = new ObservableCollection<JournalEventViewModel>();
				foreach (var item in journalItemsResult)
				{
					Events.Add(new JournalEventViewModel(item));
				}
				OnPropertyChanged(() => Events);
				SelectedEvent = Events.FirstOrDefault();
			}
		}
	}
}