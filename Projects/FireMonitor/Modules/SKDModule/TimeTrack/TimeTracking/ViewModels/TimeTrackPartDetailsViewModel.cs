using System.Globalization;
using System.Reactive.Linq;
using FiresecAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI;
using SKDModule.Model;
using SKDModule.Helpers;
using DayTimeTrackPart = SKDModule.Model.DayTimeTrackPart;
using TimeTrackZone = SKDModule.Model.TimeTrackZone;

namespace SKDModule.ViewModels
{
	public class TimeTrackPartDetailsViewModel: SaveCancelDialogViewModel
	{
		#region Fields
		readonly DayTimeTrack _dayTimeTrack;
		private readonly TimeTrackDetailsViewModel _parent;
		private IDisposable _subscriber;
		#endregion

		#region Properties

		public DayTimeTrackPart CurrentTimeTrackPart { get; set; }

		public List<TimeTrackZone> Zones { get; private set; }

		TimeTrackZone _selectedZone;
		public TimeTrackZone SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				OnPropertyChanged(() => SelectedZone);
			}
		}

		private bool _isEnabledTakeInCalculations;

		public bool IsEnabledTakeInCalculations
		{
			get { return _isEnabledTakeInCalculations; }
			set
			{
				if (_isEnabledTakeInCalculations == value) return;
				_isEnabledTakeInCalculations = value;
				OnPropertyChanged(() => IsEnabledTakeInCalculations);
			}
		}

		private bool _isDirty;

		public bool IsDirty
		{
			get { return _isDirty; }
			set
			{
				if (_isDirty == value) return;
				_isDirty = value;
				OnPropertyChanged(() => IsDirty);
			}
		}

		private bool _notTakeInCalculations;

		public bool NotTakeInCalculations
		{
			get { return _notTakeInCalculations; }
			set
			{
				if (_notTakeInCalculations == value) return;
				_notTakeInCalculations = value;
				OnPropertyChanged(() => NotTakeInCalculations);
			}
		}

		#endregion

		#region Constructors

		public TimeTrackPartDetailsViewModel(DayTimeTrack dayTimeTrack, ShortEmployee employee, TimeTrackDetailsViewModel parent, DayTimeTrackPart inputTimeTrackPart = null)
		{
			_dayTimeTrack = dayTimeTrack;
			_parent = parent;

			Zones = new List<TimeTrackZone>(TimeTrackingHelper.GetMergedZones(employee));

			BuidObservables();

			if (inputTimeTrackPart != null)
			{
				CurrentTimeTrackPart = inputTimeTrackPart;
				CurrentTimeTrackPart.EnterTime = inputTimeTrackPart.EnterTime;
				CurrentTimeTrackPart.ExitTime = inputTimeTrackPart.ExitTime;
				NotTakeInCalculations = inputTimeTrackPart.NotTakeInCalculations;
				Title = "Редактировать проход";
			}
			else
			{
				CurrentTimeTrackPart = new DayTimeTrackPart
				{
					UID = Guid.NewGuid(),
					EnterDateTime = dayTimeTrack.Date,
					ExitDateTime = dayTimeTrack.Date,
					IsManuallyAdded = true
				};

				Title = "Добавить проход";
			}

			SelectedZone = Zones.FirstOrDefault();
		}
		#endregion

		#region Commands

		protected override bool Save()
		{
			CurrentTimeTrackPart.TimeTrackZone = SelectedZone;
			CurrentTimeTrackPart.EnterDateTime = CurrentTimeTrackPart.EnterDateTime.GetValueOrDefault().Date + CurrentTimeTrackPart.EnterTime;
			CurrentTimeTrackPart.ExitDateTime = CurrentTimeTrackPart.ExitDateTime.GetValueOrDefault().Date + CurrentTimeTrackPart.ExitTime;
			CurrentTimeTrackPart.CorrectedBy = FiresecManager.CurrentUser.Name;
			CurrentTimeTrackPart.AdjustmentDate = DateTime.Now;
			CurrentTimeTrackPart.CorrectedDate = CurrentTimeTrackPart.AdjustmentDate.Value.ToString(CultureInfo.CurrentUICulture);
			CurrentTimeTrackPart.CorrectedByUID = FiresecManager.CurrentUser.UID;
			CurrentTimeTrackPart.NotTakeInCalculations = NotTakeInCalculations;

			return Validate();
		}

		protected override bool CanSave()
		{
			return SelectedZone != null && CurrentTimeTrackPart.IsValid;
		}

		#endregion

		#region Methods

		private void BuidObservables()
		{
			this.WhenAny(x => x.CurrentTimeTrackPart, x => x.Value)
				.Subscribe(value =>
				{
					if (_subscriber != null) _subscriber.Dispose();

					if (value == null) return;

					_subscriber = Observable.Merge(value.UIChanged).Subscribe(x =>
					{
						IsDirty = true;
						value.IsDirty = true;
					});
				});

			this.WhenAny(x => x.IsCancelled, x => x.Value)
				.Subscribe(value =>
				{
					if (!value) return;

					IsDirty = default(bool);
					CurrentTimeTrackPart.IsDirty = default(bool);
				});

			this.WhenAny(x => x.SelectedZone, x => x.Value)
				.Subscribe(value =>
				{
					if (value != null && !value.IsURV)
					{
						IsEnabledTakeInCalculations = false;
						NotTakeInCalculations = true;
					}
					else
					{
						IsEnabledTakeInCalculations = true;
						NotTakeInCalculations = default(bool);
					}
				});
		}

		public bool Validate()
		{
			var intersectionCollection = PassJournalHelper.GetIntersectionIntervals(CurrentTimeTrackPart.ToDTO(), _parent.ShortEmployee).Select(x => new DayTimeTrackPart(x));

			return !intersectionCollection.Any()
				|| DialogService.ShowModalWindow(new WarningIntersectionIntervalDialogWindowViewModel(CurrentTimeTrackPart, intersectionCollection));
		}

		public bool IsIntersection(TimeTrackDetailsViewModel timeTrackDetailsViewModel)
		{
			return timeTrackDetailsViewModel.DayTimeTrackParts.Any(x => x.UID != CurrentTimeTrackPart.UID && (x.EnterDateTime < CurrentTimeTrackPart.ExitDateTime && x.ExitDateTime > CurrentTimeTrackPart.EnterDateTime));
		}

		#endregion
	}
}
