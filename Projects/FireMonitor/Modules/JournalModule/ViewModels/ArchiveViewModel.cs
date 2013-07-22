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
using FS2Api;
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
			ServiceFactory.Events.GetEvent<GetFilteredArchiveCompletedEvent>().Subscribe(OnGetFilteredArchiveCompleted);
			ServiceFactory.Events.GetEvent<GetFS2FilteredArchiveCompletedEvent>().Subscribe(OnGetFS2FilteredArchiveCompleted);

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
				UpdateThread = new Thread(OnUpdate);
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
			Dispatcher.BeginInvoke(new Action(() =>
			{
				if (JournalRecords == null)
					JournalRecords = new ObservableRangeCollection<JournalRecordViewModel>();

				var journalRecordViewModels = new List<JournalRecordViewModel>();
				foreach (var journalRecord in journalRecords)
				{
					var journalRecordViewModel = new JournalRecordViewModel(journalRecord);
					journalRecordViewModels.Add(journalRecordViewModel);
				}
				JournalRecords.AddRange(journalRecordViewModels);

				Status = "Количество записей: " + JournalRecords.Count.ToString();
				ApplicationService.DoEvents();
			}
				));
			ApplicationService.DoEvents();
		}

		void OnGetFS2FilteredArchiveCompleted(IEnumerable<FS2JournalItem> journalItems)
		{
			Dispatcher.Invoke(new Action(() =>
			{
				if (JournalRecords == null)
					JournalRecords = new ObservableRangeCollection<JournalRecordViewModel>();

				var journalRecordViewModels = new List<JournalRecordViewModel>();
				foreach (var journalItem in journalItems)
				{
					var journalRecordViewModel = new JournalRecordViewModel(journalItem);
					journalRecordViewModels.Add(journalRecordViewModel);
				}
				JournalRecords.AddRange(journalRecordViewModels);

				Status = "Количество записей: " + JournalRecords.Count.ToString();
			}
				));
		}

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