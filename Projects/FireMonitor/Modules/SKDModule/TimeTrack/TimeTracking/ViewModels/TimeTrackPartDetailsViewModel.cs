using System.Reactive.Linq;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Org.BouncyCastle.Crypto.Engines;
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

		#endregion

		#region Constructors

		public TimeTrackPartDetailsViewModel(DayTimeTrack dayTimeTrack, ShortEmployee employee, TimeTrackDetailsViewModel parent, DayTimeTrackPart inputTimeTrackPart = null)
		{
			_dayTimeTrack = dayTimeTrack;
			_parent = parent;
			if (inputTimeTrackPart != null)
			{
				CurrentTimeTrackPart = inputTimeTrackPart;
				CurrentTimeTrackPart.EnterTime = inputTimeTrackPart.EnterTime;
				CurrentTimeTrackPart.ExitTime = inputTimeTrackPart.ExitTime;
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

			Zones = new List<TimeTrackZone>(TimeTrackingHelper.GetMergedZones(employee));
			SelectedZone = Zones.FirstOrDefault();

			this.WhenAny(x => x.CurrentTimeTrackPart, x => x.Value)
				.Subscribe(value =>
				{
					if(_subscriber != null) _subscriber.Dispose();

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
						CurrentTimeTrackPart.NotTakeInCalculations = true;
					}
					else
					{
						IsEnabledTakeInCalculations = true;
						CurrentTimeTrackPart.NotTakeInCalculations = false;
					}
				});
		}
		#endregion

		#region Commands

		protected override bool Save()
		{
			CurrentTimeTrackPart.TimeTrackZone = SelectedZone;
			CurrentTimeTrackPart.EnterTimeOriginal = CurrentTimeTrackPart.EnterDateTime + CurrentTimeTrackPart.EnterTime;
			CurrentTimeTrackPart.ExitTimeOriginal = CurrentTimeTrackPart.ExitDateTime + CurrentTimeTrackPart.ExitTime;
			CurrentTimeTrackPart.CorrectedBy = FiresecManager.CurrentUser.Name;
			CurrentTimeTrackPart.CorrectedByUID = FiresecManager.CurrentUser.UID;
			CurrentTimeTrackPart.AdjustmentDate = DateTime.Now;

			return Validate();
		}

		protected override bool CanSave()
		{
			return SelectedZone != null && CurrentTimeTrackPart.IsValid;
		}

		#endregion

		#region Methods

		public bool Validate()
		{
			if (CurrentTimeTrackPart.EnterDateTime == null || CurrentTimeTrackPart.ExitDateTime == null)
			{
				MessageBoxService.Show("Выберите дату начала и дату конца интервала");
				return false;
			}

			if (_parent == null || !IsIntersection(_parent)) return true;
			MessageBoxService.Show("Невозможно добавить пересекающийся интервал");
			return false;
		}

		public bool IsIntersection(TimeTrackDetailsViewModel timeTrackDetailsViewModel)
		{
			return timeTrackDetailsViewModel.DayTimeTrackParts.Any(x => x.UID != CurrentTimeTrackPart.UID &&
																		(x.EnterDateTime < (CurrentTimeTrackPart.EnterDateTime + CurrentTimeTrackPart.EnterTime) &&
																		x.ExitDateTime > (CurrentTimeTrackPart.EnterDateTime + CurrentTimeTrackPart.EnterTime) ||
																		x.EnterDateTime < (CurrentTimeTrackPart.ExitDateTime + CurrentTimeTrackPart.ExitTime) &&
																		x.ExitDateTime > (CurrentTimeTrackPart.ExitDateTime + CurrentTimeTrackPart.EnterTime)));
		}

		#endregion
	}
}
