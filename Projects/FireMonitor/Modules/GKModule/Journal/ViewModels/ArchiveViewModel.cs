using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Common;
using Common.GK;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Models;
using GKModule.Journal.ViewModels;
using Microsoft.Win32;

namespace GKModule.ViewModels
{
	public class ArchiveViewModel : ViewPartViewModel
	{
		public static DateTime ArchiveFirstDate { get; private set; }
		ArchiveDefaultState ArchiveDefaultState;
		XArchiveFilter ArchiveFilter;
		Thread UpdateThread;
		bool FirstTime = true;

		public ArchiveViewModel()
		{
			ShowFilterCommand = new RelayCommand(OnShowFilter);
			ShowSettingsCommand = new RelayCommand(OnShowSettings);
			ExportToPdfCommand = new RelayCommand(OnExportToPdfCommand, CanExportToPdfCommand);

			ArchiveDefaultState = ClientSettings.ArchiveDefaultState;
			if (ArchiveDefaultState == null)
				ArchiveDefaultState = new ArchiveDefaultState();
		}

		public void Initialize()
		{
			ArchiveFirstDate = DateTime.Now.AddDays(-1);
			_isFilterOn = false;
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

		public bool IsManyGK
		{
			get
			{
				return XManager.IsManyGK();
			}
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
					ArchiveDefaultState = archiveSettingsViewModel.GetModel();
					ClientSettings.ArchiveDefaultState = ArchiveDefaultState;
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


		public RelayCommand ExportToPdfCommand { get; private set; }
		private void OnExportToPdfCommand()
		{
			try
			{
				ArchivePdfCreater.Create(JournalItems);
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове ArchiveViewModel.OnExportToPdfCommand");
				MessageBoxService.ShowException(e);
			}
		}
		private bool CanExportToPdfCommand()
		{
			return JournalItems.Count > 0;
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
				JournalItems = new ObservableCollection<JournalItemViewModel>();
				UpdateThread = new Thread(new ThreadStart(OnUpdate));
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

				var journalRecords = GKDBHelper.Select(archiveFilter);
				Dispatcher.BeginInvoke(new Action(() => { OnGetFilteredArchiveCompleted(journalRecords); }));
			}
			catch (Exception e)
			{
				Logger.Error(e, "ArchiveViewModel.OnUpdate");
			}
			UpdateThread = null;
		}

		void OnGetFilteredArchiveCompleted(IEnumerable<JournalItem> journalItems)
		{
			JournalItems = new ObservableCollection<JournalItemViewModel>();
			foreach (var journalItem in journalItems)
			{
				var journalItemViewModel = new JournalItemViewModel(journalItem);
				JournalItems.Add(journalItemViewModel);
			}
			SelectedJournal = JournalItems.FirstOrDefault();

			Status = "Всего записей " + journalItems.Count().ToString();
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