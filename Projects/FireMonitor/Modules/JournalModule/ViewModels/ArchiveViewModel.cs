using System;
using System.Collections.ObjectModel;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Models;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace JournalModule.ViewModels
{
	public class ArchiveViewModel : ViewPartViewModel
	{
		public static DateTime ArchiveFirstDate { get; private set; }
		ArchiveDefaultState _archiveDefaultState;
		ArchiveFilter _archiveFilter;
		Thread _updateThread;

		public ArchiveViewModel()
		{
			ShowFilterCommand = new RelayCommand(OnShowFilter);
			ShowSettingsCommand = new RelayCommand(OnShowSettings);

			_archiveDefaultState = ClientSettings.ArchiveDefaultState;
			if (_archiveDefaultState == null)
				_archiveDefaultState = new ArchiveDefaultState();
		}

		public void Initialize()
		{
			var operationResult = FiresecManager.FiresecService.GetArchiveStartDate();
			if (operationResult.HasError == false)
			{
				ArchiveFirstDate = operationResult.Result;
				_isFilterOn = false;
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
				_isFilterOn = value;
				OnPropertyChanged("IsFilterOn");
				Update(true);
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

			ArchiveFilterViewModel archiveFilterViewModel = null;

			var result = WaitHelper.Execute(() =>
			{
				archiveFilterViewModel = new ArchiveFilterViewModel(_archiveFilter);
			});

			if (result)
			{
				if (DialogService.ShowModalWindow(archiveFilterViewModel))
				{
					_archiveFilter = archiveFilterViewModel.GetModel();
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
				var archiveSettingsViewModel = new ArchiveSettingsViewModel(_archiveDefaultState);
				if (DialogService.ShowModalWindow(archiveSettingsViewModel))
				{
					_archiveDefaultState = archiveSettingsViewModel.GetModel();
					ClientSettings.ArchiveDefaultState = _archiveDefaultState;
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
				if (_updateThread != null)
					_updateThread.Abort();
			}
			if (_updateThread == null)
			{
				Status = "Загрузка данных";
				JournalRecords = new ObservableCollection<JournalRecordViewModel>();
				_updateThread = new Thread(new ThreadStart(OnUpdate));
				_updateThread.Start();
			}
		}

		void OnUpdate()
		{
			try
			{
				ArchiveFilter archiveFilter = null;
				if (IsFilterOn)
					archiveFilter = _archiveFilter;
				else
					archiveFilter = GerFilterFromDefaultState(_archiveDefaultState);

				var operationResult = FiresecManager.FiresecService.GetFilteredArchive(archiveFilter);

				ServiceFactory.SafeCall(() =>
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
				});
			}
			catch (Exception e)
			{
				;
			}
			finally
			{
				_updateThread = null;
				ServiceFactory.SafeCall(() =>
				{
					Status = null;
				});
			}
		}

		public override void OnShow()
		{
			Update(false);
		}
	}
}