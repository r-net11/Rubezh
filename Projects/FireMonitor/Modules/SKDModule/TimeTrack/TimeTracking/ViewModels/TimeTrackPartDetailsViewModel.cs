using StrazhAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ReactiveUI;
using SKDModule.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using DayTimeTrackPart = SKDModule.Model.DayTimeTrackPart;
using TimeTrackZone = SKDModule.Model.TimeTrackZone;

namespace SKDModule.ViewModels
{
	public class TimeTrackPartDetailsViewModel: SaveCancelDialogViewModel
	{
		#region Fields
		private readonly TimeTrackDetailsViewModel _parent;
		private IDisposable _subscriber;
		#endregion

		#region Properties

		public DayTimeTrackPart DayTimeTrackPart { get; set; }

		private TimeTrackZone CurrentZone { get; set; }

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

		private bool _isNew;

		public bool IsNew
		{
			get { return _isNew; }
			set
			{
				if (_isNew == value) return;
				_isNew = value;
				OnPropertyChanged(() => IsNew);
			}
		}

		#endregion

		#region Constructors

		public TimeTrackPartDetailsViewModel(DayTimeTrack dayTimeTrack, ShortEmployee employee, TimeTrackDetailsViewModel parent, DayTimeTrackPart inputTimeTrackPart = null)
		{
			_parent = parent;

			Zones = new List<TimeTrackZone>(TimeTrackingHelper.GetAllZones(employee));

			BuidObservables();
			DayTimeTrackPart = inputTimeTrackPart;

			if (inputTimeTrackPart != null)
			{
				CurrentTimeTrackPart = new DayTimeTrackPart
				{
					UID = inputTimeTrackPart.UID,
					EnterDateTime = inputTimeTrackPart.EnterDateTime.GetValueOrDefault().Date,
					ExitDateTime = inputTimeTrackPart.ExitDateTime.GetValueOrDefault().Date,
					EnterTime = inputTimeTrackPart.EnterTime,
					ExitTime = inputTimeTrackPart.ExitTime,
					TimeTrackZone = inputTimeTrackPart.TimeTrackZone,
					TimeTrackActions = inputTimeTrackPart.TimeTrackActions
				};

				NotTakeInCalculations = inputTimeTrackPart.NotTakeInCalculations;
				IsEnabledTakeInCalculations = inputTimeTrackPart.TimeTrackZone.IsURV;
				CurrentZone = CurrentTimeTrackPart.TimeTrackZone;
				Title = "Редактировать проход";
			}
			else
			{
				CurrentTimeTrackPart = new DayTimeTrackPart
				{
					UID = Guid.NewGuid(),
					EnterDateTime = dayTimeTrack.Date,
					ExitDateTime = dayTimeTrack.Date,
					IsManuallyAdded = true,
					TimeTrackActions = TimeTrackActions.Adding
				};

				Title = "Добавить проход";
				IsNew = true;
			}

			this.WhenAny(x => x.SelectedZone, x => x.Value)
				.Subscribe(value =>
				{
					if(IsNew)
						CurrentZone = value;
				});
		}
		#endregion

		#region Commands

		protected override bool Save()
		{
			CurrentTimeTrackPart.TimeTrackZone = CurrentZone;
			CurrentTimeTrackPart.EnterDateTime = CurrentTimeTrackPart.EnterDateTime.GetValueOrDefault().Date + CurrentTimeTrackPart.EnterTime;
			CurrentTimeTrackPart.ExitDateTime = CurrentTimeTrackPart.ExitDateTime.GetValueOrDefault().Date + CurrentTimeTrackPart.ExitTime;
			CurrentTimeTrackPart.CorrectedBy = FiresecManager.CurrentUser.Name;
			CurrentTimeTrackPart.AdjustmentDate = DateTime.Now;
			CurrentTimeTrackPart.CorrectedByUID = FiresecManager.CurrentUser.UID;
			CurrentTimeTrackPart.NotTakeInCalculations = NotTakeInCalculations;

			if (!Validate()) return false;

			if (IsNew) return true;

			DayTimeTrackPart.TimeTrackZone = CurrentZone;

			DayTimeTrackPart.CorrectedBy = FiresecManager.CurrentUser.Name;
			DayTimeTrackPart.AdjustmentDate = DateTime.Now;
			DayTimeTrackPart.CorrectedByUID = FiresecManager.CurrentUser.UID;

			if (DayTimeTrackPart.NotTakeInCalculations != NotTakeInCalculations)
			{
				DayTimeTrackPart.NotTakeInCalculations = NotTakeInCalculations;

				if (NotTakeInCalculations)
					DayTimeTrackPart.TimeTrackActions |= TimeTrackActions.TurnOnCalculation;
				else
					DayTimeTrackPart.TimeTrackActions |= TimeTrackActions.TurnOffCalculation;
			}

			if ((DayTimeTrackPart.EnterDateTime != CurrentTimeTrackPart.EnterDateTime.GetValueOrDefault().Date + CurrentTimeTrackPart.EnterTime) ||
				(DayTimeTrackPart.ExitDateTime != CurrentTimeTrackPart.ExitDateTime.GetValueOrDefault().Date + CurrentTimeTrackPart.ExitTime))
			{
				DayTimeTrackPart.EnterDateTime = CurrentTimeTrackPart.EnterDateTime.GetValueOrDefault().Date +
											 CurrentTimeTrackPart.EnterTime;
				DayTimeTrackPart.ExitDateTime = CurrentTimeTrackPart.ExitDateTime.GetValueOrDefault().Date +
												CurrentTimeTrackPart.ExitTime;
				DayTimeTrackPart.TimeTrackActions |= TimeTrackActions.EditBorders;
			}

			DayTimeTrackPart.IsNeedAdjustment = default(bool);

			return true;
		}

		protected override bool CanSave()
		{
			return CurrentTimeTrackPart.IsValid && CurrentZone != null;
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

		private List<DayTimeTrackPart> FilterFromDuplicates(IEnumerable<DayTimeTrackPart> firstCollection, IEnumerable<DayTimeTrackPart> secondCollection)
		{
			var numerableCollection = firstCollection.Union(secondCollection);
			var resultCollection = new List<DayTimeTrackPart>();
			foreach (var dayTimeTrackPart in numerableCollection.Where(dayTimeTrackPart => resultCollection.All(x => x.UID != dayTimeTrackPart.UID)))
			{
				resultCollection.Add(dayTimeTrackPart);
			}

			return resultCollection;
		}

		public bool Validate()
		{
			var intersectionCollectionFromServer = PassJournalHelper.GetIntersectionIntervals(CurrentTimeTrackPart.ToDTO(), _parent.ShortEmployee).Select(x => new DayTimeTrackPart(x));
			var intersectionCollectionFromUI = GetIntersectionIntervals(_parent);

			var resultCollection = FilterFromDuplicates(intersectionCollectionFromServer, intersectionCollectionFromUI);

			if (!resultCollection.Any()) return true;

			DialogService.ShowModalWindow(new WarningIntersectionIntervalDialogWindowViewModel(CurrentTimeTrackPart, resultCollection));
			return false;
		}

		public List<DayTimeTrackPart> GetIntersectionIntervals(TimeTrackDetailsViewModel timeTrackDetailsViewModel)
		{
			if (timeTrackDetailsViewModel == null) return null;

			var linkedIntervals = timeTrackDetailsViewModel.DayTimeTrackParts
				.Where(x => x.UID != CurrentTimeTrackPart.UID)
				.Where(x =>
						CurrentTimeTrackPart.ExitDateTime > x.EnterDateTime
						&& CurrentTimeTrackPart.EnterDateTime < x.ExitDateTime);

			return linkedIntervals.ToList();
		}

		#endregion
	}
}
