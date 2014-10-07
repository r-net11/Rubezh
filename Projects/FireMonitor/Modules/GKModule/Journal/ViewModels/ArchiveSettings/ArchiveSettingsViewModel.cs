using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Models;

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

			var archiveDefaultStateViewModel = ArchiveDefaultStates.FirstOrDefault(x => x.ArchiveDefaultStateType == archiveDefaultState.ArchiveDefaultStateType);
			if (archiveDefaultStateViewModel != null)
				archiveDefaultStateViewModel.IsActive = true;
			else
				ArchiveDefaultStates.FirstOrDefault().IsActive = true;

			switch (archiveDefaultState.ArchiveDefaultStateType)
			{
				case ArchiveDefaultStateType.LastHours:
					HoursCount = archiveDefaultState.Count;
					break;

				case ArchiveDefaultStateType.LastDays:
					DaysCount = archiveDefaultState.Count;
					break;

				case ArchiveDefaultStateType.FromDate:
					StartDate = archiveDefaultState.StartDate;
					break;

				case ArchiveDefaultStateType.RangeDate:
					StartDate = archiveDefaultState.StartDate;
					EndDate = archiveDefaultState.EndDate;
					break;
			}

			AdditionalColumns = new List<JournalColumnTypeViewModel>();
			if (archiveDefaultState.XAdditionalColumns == null)
				archiveDefaultState.XAdditionalColumns = new List<XJournalColumnType>();
			foreach (XJournalColumnType journalColumnType in Enum.GetValues(typeof(XJournalColumnType)))
			{
				var journalColumnTypeViewModel = new JournalColumnTypeViewModel(journalColumnType);
				AdditionalColumns.Add(journalColumnTypeViewModel);
				if (archiveDefaultState.XAdditionalColumns.Any(x => x == journalColumnType))
				{
					journalColumnTypeViewModel.IsChecked = true;
				}
			}

			PageSize = archiveDefaultState.PageSize;
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
				OnPropertyChanged(() => CheckedArchiveDefaultStateType);
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

		int _pageSize;
		public int PageSize
		{
			get { return _pageSize; }
			set
			{
				_pageSize = value;
				OnPropertyChanged(() => PageSize);
			}
		}

		protected override bool Save()
		{
			ArchiveDefaultState.ArchiveDefaultStateType = ArchiveDefaultStates.FirstOrDefault(x => x.IsActive).ArchiveDefaultStateType;
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

				default:
					break;
			}
			ArchiveDefaultState.XAdditionalColumns = new List<XJournalColumnType>();
			foreach (var journalColumnTypeViewModel in AdditionalColumns)
			{
				if (journalColumnTypeViewModel.IsChecked)
					ArchiveDefaultState.XAdditionalColumns.Add(journalColumnTypeViewModel.JournalColumnType);
			}

			if (PageSize < 10)
				PageSize = 10;
			if (PageSize > 1000)
				PageSize = 1000;
			ArchiveDefaultState.PageSize = PageSize;
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