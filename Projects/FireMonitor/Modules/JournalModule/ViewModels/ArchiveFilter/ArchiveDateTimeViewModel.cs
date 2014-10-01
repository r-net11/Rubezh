using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Models;
using JournalModule.Events;

namespace JournalModule.ViewModels
{
	public class ArchiveDateTimeViewModel : BaseViewModel
	{
		public ArchiveDateTimeViewModel(ArchiveDefaultState archiveDefaultState)
		{
			ArchiveDefaultStates = new ObservableCollection<ArchiveDefaultStateViewModel>();
			foreach (ArchiveDefaultStateType item in Enum.GetValues(typeof(ArchiveDefaultStateType)))
			{
				ArchiveDefaultStates.Add(new ArchiveDefaultStateViewModel(item));
			}

			HoursCount = 1;
			DaysCount = 1;
			StartDate = ArchiveFirstDate;
			EndDate = NowDate;

			var archiveDefaultStateViewModel = ArchiveDefaultStates.FirstOrDefault(x => x.ArchiveDefaultStateType == archiveDefaultState.ArchiveDefaultStateType);
			if (archiveDefaultStateViewModel != null)
				archiveDefaultStateViewModel.IsActive = true;
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

				default:
					break;
			}
		}

		public ObservableCollection<ArchiveDefaultStateViewModel> ArchiveDefaultStates { get; private set; }

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

		public ArchiveDefaultState GetModel()
		{
			var archiveDefaultState = new ArchiveDefaultState();
			archiveDefaultState.ArchiveDefaultStateType = ArchiveDefaultStates.First(x => x.IsActive).ArchiveDefaultStateType;
			switch (archiveDefaultState.ArchiveDefaultStateType)
			{
				case ArchiveDefaultStateType.LastHours:
					archiveDefaultState.Count = HoursCount;
					break;

				case ArchiveDefaultStateType.LastDays:
					archiveDefaultState.Count = DaysCount;
					break;

				case ArchiveDefaultStateType.FromDate:
					archiveDefaultState.StartDate = StartDate;
					break;

				case ArchiveDefaultStateType.RangeDate:
					archiveDefaultState.StartDate = StartDate;
					archiveDefaultState.EndDate = EndDate;
					break;

				default:
					break;
			}
			return archiveDefaultState;
		}
	}
}