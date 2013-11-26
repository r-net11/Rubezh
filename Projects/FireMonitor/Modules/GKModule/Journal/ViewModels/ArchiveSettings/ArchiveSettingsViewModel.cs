using System;
using System.Collections.ObjectModel;
using System.Linq;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Models;
using System.Collections.Generic;

namespace GKModule.ViewModels
{
	public class ArchiveSettingsViewModel : SaveCancelDialogViewModel
	{
		public ArchiveSettingsViewModel(ArchiveDefaultState archiveDefaultState)
		{
			Title = "Настройки";

			ArchiveDefaultState = new ArchiveDefaultState();
			ArchiveDefaultStates = new ObservableCollection<ArchiveDefaultStateViewModel>();
			foreach (ArchiveDefaultStateType item in Enum.GetValues(typeof(ArchiveDefaultStateType)))
			{
				ArchiveDefaultStates.Add(new ArchiveDefaultStateViewModel(item));
			}

			HoursCount = 1;
			DaysCount = 1;
			StartDate = ArchiveFirstDate;
			EndDate = NowDate;

			ServiceFactory.Events.GetEvent<ArchiveDefaultStateCheckedEvent>().Subscribe(OnArchiveDefaultStateCheckedEvent);

			ArchiveDefaultStates.First(x => x.ArchiveDefaultStateType == archiveDefaultState.ArchiveDefaultStateType).IsActive = true;
			switch (archiveDefaultState.ArchiveDefaultStateType)
			{
				case ArchiveDefaultStateType.LastHours:
					if (archiveDefaultState.Count.HasValue)
						HoursCount = archiveDefaultState.Count.Value;
					break;

				case ArchiveDefaultStateType.LastDays:
					if (archiveDefaultState.Count.HasValue)
						DaysCount = archiveDefaultState.Count.Value;
					break;

				case ArchiveDefaultStateType.FromDate:
					if (archiveDefaultState.StartDate.HasValue)
						StartDate = archiveDefaultState.StartDate.Value;
					break;

				case ArchiveDefaultStateType.RangeDate:
					if (archiveDefaultState.StartDate.HasValue)
						StartDate = archiveDefaultState.StartDate.Value;
					if (archiveDefaultState.EndDate.HasValue)
						EndDate = archiveDefaultState.EndDate.Value;
					break;

				case ArchiveDefaultStateType.All:
				default:
					break;
			}

			AdditionalColumns = new List<JournalColumnTypeViewModel>();
			if (archiveDefaultState.AdditionalColumns == null)
				archiveDefaultState.AdditionalColumns = new List<JournalColumnType>();
			foreach (JournalColumnType journalColumnType in Enum.GetValues(typeof(JournalColumnType)))
			{
				var journalColumnTypeViewModel = new JournalColumnTypeViewModel(journalColumnType);
				AdditionalColumns.Add(journalColumnTypeViewModel);
				if (archiveDefaultState.AdditionalColumns.Any(x => x == journalColumnType))
				{
					journalColumnTypeViewModel.IsChecked = true;
				}
			}
		}

		public ObservableCollection<ArchiveDefaultStateViewModel> ArchiveDefaultStates { get; private set; }
		public ArchiveDefaultState ArchiveDefaultState { get; private set; }

		ArchiveDefaultStateType _checkedArchiveDefaultStateType;
		public ArchiveDefaultStateType CheckedArchiveDefaultStateType
		{
			get { return _checkedArchiveDefaultStateType; }
			set
			{
				_checkedArchiveDefaultStateType = value;
				OnPropertyChanged("CheckedArchiveDefaultStateType");
			}
		}

		public List<JournalColumnTypeViewModel> AdditionalColumns { get; private set; }
		public int HoursCount { get; set; }
		public int DaysCount { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }

		public DateTime ArchiveFirstDate
		{
			get { return ArchiveViewModel.ArchiveFirstDate; }
		}

		public DateTime NowDate
		{
			get { return DateTime.Now; }
		}

		protected override bool Save()
		{
			ArchiveDefaultState.ArchiveDefaultStateType = ArchiveDefaultStates.First(x => x.IsActive).ArchiveDefaultStateType;
			switch (ArchiveDefaultState.ArchiveDefaultStateType)
			{
				case ArchiveDefaultStateType.LastHours:
					ArchiveDefaultState.Count = HoursCount;
					break;

				case ArchiveDefaultStateType.LastDays:
					ArchiveDefaultState.Count = DaysCount;
					break;

				case ArchiveDefaultStateType.FromDate:
					ArchiveDefaultState.StartDate = StartDate;
					break;

				case ArchiveDefaultStateType.RangeDate:
					ArchiveDefaultState.StartDate = StartDate;
					ArchiveDefaultState.EndDate = EndDate;
					break;

				case ArchiveDefaultStateType.All:
				default:
					break;
			}
			ArchiveDefaultState.AdditionalColumns = new List<JournalColumnType>();
			foreach (var journalColumnTypeViewModel in AdditionalColumns)
			{
				if (journalColumnTypeViewModel.IsChecked)
					ArchiveDefaultState.AdditionalColumns.Add(journalColumnTypeViewModel.JournalColumnType);
			}
			return base.Save();
		}

		void OnArchiveDefaultStateCheckedEvent(ArchiveDefaultStateViewModel archiveDefaultState)
		{
			foreach (var defaultState in ArchiveDefaultStates.Where(x => x != archiveDefaultState))
			{
				defaultState.IsActive = false;
			}
			CheckedArchiveDefaultStateType = archiveDefaultState.ArchiveDefaultStateType;
		}
	}
}