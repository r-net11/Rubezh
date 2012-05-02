using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace JournalModule.ViewModels
{
	public class ArchiveViewModel : RegionViewModel
	{
		public static DateTime ArchiveFirstDate { get; private set; }
		ArchiveDefaultState _archiveDefaultState;
		ArchiveFilter _archiveFilter;

		public ArchiveViewModel()
		{
			ShowFilterCommand = new RelayCommand(OnShowFilter);
			ShowSettingsCommand = new RelayCommand(OnShowSettings);

			_archiveDefaultState = ClientSettings.ArchiveDefaultState;
			if (_archiveDefaultState == null)
				_archiveDefaultState = new ArchiveDefaultState();

			Initialize();
		}

		public void Initialize()
		{
			var operationResult = FiresecManager.FiresecService.GetArchiveStartDate();
			if (operationResult.HasError == false)
			{
				ArchiveFirstDate = operationResult.Result;
				IsFilterOn = false;
			}
		}

		ObservableCollection<JournalRecordViewModel> _journalRecords;
		public ObservableCollection<JournalRecordViewModel> JournalRecords
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
				ArchiveFilter archiveFilter = null;
				if (value)
					archiveFilter = _archiveFilter;
				else
					archiveFilter = GerFilterFromDefaultState(_archiveDefaultState);

				var operationResult = FiresecManager.FiresecService.GetFilteredArchive(archiveFilter);
				UpdateJournals(operationResult);

				_isFilterOn = value;
				OnPropertyChanged("IsFilterOn");
			}
		}

		void UpdateJournals(OperationResult<List<JournalRecord>> operationResult)
		{
			JournalRecords = new ObservableCollection<JournalRecordViewModel>();
			if (operationResult.HasError == false)
			{
				foreach (var journalRecord in operationResult.Result)
				{
					var journalRecordViewModel = new JournalRecordViewModel(journalRecord);
					JournalRecords.Add(journalRecordViewModel);
				}
			}
		}

		public bool IsFilterExists
		{
			get { return _archiveFilter != null; }
		}

		public RelayCommand ShowFilterCommand { get; private set; }
		void OnShowFilter()
		{
			if (_archiveFilter == null)
				_archiveFilter = GerFilterFromDefaultState(_archiveDefaultState);

		   var archiveFilterViewModel = new ArchiveFilterViewModel(_archiveFilter);

			if (ServiceFactory.UserDialogs.ShowModalWindow(archiveFilterViewModel))
			{
				_archiveFilter = archiveFilterViewModel.GetModel();
				OnPropertyChanged("IsFilterExists");
				IsFilterOn = true;
			}
		}

		public RelayCommand ShowSettingsCommand { get; private set; }
		void OnShowSettings()
		{
			var archiveSettingsViewModel = new ArchiveSettingsViewModel(_archiveDefaultState);
			if (ServiceFactory.UserDialogs.ShowModalWindow(archiveSettingsViewModel))
			{
				_archiveDefaultState = archiveSettingsViewModel.GetModel();
				ClientSettings.ArchiveDefaultState = _archiveDefaultState;
				if (IsFilterOn == false)
					IsFilterOn = IsFilterOn;
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
	}
}