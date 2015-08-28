using System;
using System.Collections.Generic;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using DayTimeTrackPart = SKDModule.Model.DayTimeTrackPart;

namespace SKDModule.ViewModels
{
	public class ResetAdjustmentsConflictDialogWindowViewModel : OverridedSaveCancelDialogViewModel
	{
		#region Properties

		public bool IsCheckedSave { get; private set; }
		public bool IsCheckedCancel { get; private set; }

		public DayTimeTrackPart ResetedDayTimeTrackPart { get; set; }

		private string _conflictZone;

		public string ConflictZone
		{
			get { return _conflictZone; }
			set
			{
				if (string.Equals(_conflictZone, value)) return;
				_conflictZone = value;
				OnPropertyChanged(() => ConflictZone);
			}
		}

		private DateTime? _conflictEnterDate;

		public DateTime? ConflictEnterDate
		{
			get { return _conflictEnterDate; }
			set
			{
				if (_conflictEnterDate == value) return;
				_conflictEnterDate = value;
				OnPropertyChanged(() => ConflictEnterDate);
			}
		}

		private DateTime? _conflictExitDate;

		public DateTime? ConflictExitDate
		{
			get { return _conflictExitDate; }
			set
			{
				if (_conflictExitDate == value) return;
				_conflictExitDate = value;
				OnPropertyChanged(() => ConflictExitDate);
			}
		}

		private List<DayTimeTrackPart> _conflictingIntervals;

		public List<DayTimeTrackPart> ConflictingIntervals
		{
			get { return _conflictingIntervals; }
			set
			{
				_conflictingIntervals = value;
				OnPropertyChanged(() => ConflictingIntervals);
			}
		}

		private string _saveCaption;
		public override string SaveCaption
		{
			get { return _saveCaption; }
			set
			{
				if (string.Equals(_saveCaption, value)) return;
				_saveCaption = value;
				OnPropertyChanged(() => SaveCaption);
			}
		}

		private string _cancelCaption;
		public override string CancelCaption
		{
			get { return _cancelCaption; }
			set
			{
				if (string.Equals(_cancelCaption, value)) return;
				_cancelCaption = value;
				OnPropertyChanged(() => CancelCaption);
			}
		}

		private bool _applyToAll;

		public bool ApplyToAll
		{
			get { return _applyToAll; }
			set
			{
				if (_applyToAll == value) return;
				_applyToAll = value;
				OnPropertyChanged(() => ApplyToAll);
			}
		}

		#endregion

		#region Constructors

		public ResetAdjustmentsConflictDialogWindowViewModel()
		{
			RoundIntervalCommand = new RelayCommand(OnRoundingInterval);
			RemoveIntervalCommand = new RelayCommand(OnRemovingInverval);
		}

		#endregion

		#region Methods

		public void SetValues(KeyValuePair<DayTimeTrackPart, List<DayTimeTrackPart>> conflictTimeTrackParts)
		{
			ResetedDayTimeTrackPart = conflictTimeTrackParts.Key;
			ConflictingIntervals = conflictTimeTrackParts.Value;
		}

		#endregion

		public RelayCommand RoundIntervalCommand { get; set; }
		public RelayCommand RemoveIntervalCommand { get; set; }

		//protected override bool Save()
		//{
		//	if (ApplyToAll)
		//		IsCheckedSave = true;
		//	return base.Save();
		//}

		public void OnRoundingInterval()
		{
			if (ApplyToAll)
				IsCheckedSave = true;
			bool result = Save();
			if (result)
				Close(true);
		}

		public void OnRemovingInverval()
		{
			if (ApplyToAll)
				IsCheckedCancel = true;
			Close(null);
			//return base.Cancel();
		}

		//protected override bool Cancel()
		//{
		//	if (ApplyToAll)
		//		IsCheckedCancel = true;
		//	return base.Cancel();
		//}
	}
}
