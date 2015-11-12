using Common;
using FiresecAPI;
using FiresecAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ReactiveUI;
using SKDModule.Events;
using SKDModule.Helpers;
using SKDModule.Model;
using SKDModule.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Data;
using DayTimeTrackPart = SKDModule.Model.DayTimeTrackPart;

namespace SKDModule.ViewModels
{
	public class TimeTrackDetailsViewModel : SaveCancelDialogViewModel
	{
		#region Properties

		public List<DayTimeTrackPart> RemovedDayTimeTrackParts { get; set; }

		private TimeTrackAttachedDocument _selectedDocument;
		public TimeTrackAttachedDocument SelectedDocument
		{
			get { return _selectedDocument; }
			set
			{
				_selectedDocument = value;
				OnPropertyChanged(() => SelectedDocument);
			}
		}

		private DayTimeTrackPart _selectedDayTimeTrackPart;
		public DayTimeTrackPart SelectedDayTimeTrackPart
		{
			get { return _selectedDayTimeTrackPart; }
			set
			{
				_selectedDayTimeTrackPart = value;
				OnPropertyChanged(() => SelectedDayTimeTrackPart);
			}
		}

		public ObservableCollection<TimeTrackAttachedDocument> Documents { get; private set; }

		public ObservableCollection<DayTimeTrackPart> DayTimeTrackParts { get; private set; }

		public DayTimeTrack DayTimeTrack { get; private set; }

		public ShortEmployee ShortEmployee { get; private set; }

		private DayTimeTrackPart _selectedTimeTrackPartDetailsViewModel;

		public DayTimeTrackPart SelectedTimeTrackPartDetailsViewModel
		{
			get { return _selectedTimeTrackPartDetailsViewModel; }
			set
			{
				if (_selectedTimeTrackPartDetailsViewModel == value) return;
				_selectedTimeTrackPartDetailsViewModel = value;
				OnPropertyChanged(() => SelectedTimeTrackPartDetailsViewModel);
			}
		}

		bool _isDirty;
		public bool IsDirty
		{
			get { return _isDirty; }
			set
			{
				_isDirty = value;
				OnPropertyChanged(() => IsDirty);
			}
		}

		private bool _canDoChanges;
		public bool CanDoChanges
		{
			get { return _canDoChanges; }
			set
			{
				if (_canDoChanges == value) return;
				_canDoChanges = value;
				OnPropertyChanged(() => CanDoChanges);
			}
		}

		public TimeSpan BalanceTimeSpan
		{
			get
			{
				if (DayTimeTrack == null || DayTimeTrack.Totals == null) return default(TimeSpan);
				var balance = DayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.Balance);
				return balance != null ? balance.TimeSpan : default(TimeSpan);
			}
		}

		public TimeSpan PresentTimeSpan
		{
			get
			{
				if (DayTimeTrack == null || DayTimeTrack.Totals == null) return default(TimeSpan);
				var present = DayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.Presence);
				return present != null ? present.TimeSpan : default(TimeSpan);
			}
		}

		public TimeSpan AbsenceTimeSpan
		{
			get
			{
				if (DayTimeTrack == null || DayTimeTrack.Totals == null) return default(TimeSpan);
				var absence = DayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.Absence);
				return absence != null ? absence.TimeSpan : default(TimeSpan);
			}
		}

		public TimeSpan AbsenceInsidePlanTimeSpan
		{
			get
			{
				if (DayTimeTrack == null || DayTimeTrack.Totals == null) return default(TimeSpan);
				var absenceInsidePlan = DayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.AbsenceInsidePlan);
				return absenceInsidePlan != null ? absenceInsidePlan.TimeSpan : default(TimeSpan);
			}
		}

		public TimeSpan LateTimeSpan
		{
			get
			{
				if (DayTimeTrack == null || DayTimeTrack.Totals == null) return default(TimeSpan);
				var late = DayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.Late);
				return late != null ? late.TimeSpan : default(TimeSpan);
			}
		}

		public TimeSpan EarlyLeaveTimeSpan
		{
			get
			{
				if (DayTimeTrack == null || DayTimeTrack.Totals == null) return default(TimeSpan);
				var earlyLeave = DayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.EarlyLeave);
				return earlyLeave != null ? earlyLeave.TimeSpan : default(TimeSpan);
			}
		}

		public TimeSpan OvertimeTimeSpan
		{
			get
			{
				if (DayTimeTrack == null || DayTimeTrack.Totals == null) return default(TimeSpan);
				var overtime = DayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.Overtime);
				return overtime != null ? overtime.TimeSpan : default(TimeSpan);
			}
		}

		public TimeSpan NightTimeSpan
		{
			get
			{
				if (DayTimeTrack == null || DayTimeTrack.Totals == null) return default(TimeSpan);
				var night = DayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.Night);
				return night != null ? night.TimeSpan : default(TimeSpan);
			}
		}

		public TimeSpan DocumentOvertimeTimeSpan
		{
			get
			{
				if (DayTimeTrack == null || DayTimeTrack.Totals == null) return default(TimeSpan);
				var docOvertime = DayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.DocumentOvertime);
				return docOvertime != null ? docOvertime.TimeSpan : default(TimeSpan);
			}
		}

		public TimeSpan DocumentPresenceTimeSpan
		{
			get
			{
				if (DayTimeTrack == null || DayTimeTrack.Totals == null) return default(TimeSpan);
				var docPresence = DayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.DocumentPresence);
				return docPresence != null ? docPresence.TimeSpan : default(TimeSpan);
			}
		}

		public TimeSpan DocumentAbsenceTimeSpan
		{
			get
			{
				if (DayTimeTrack == null || DayTimeTrack.Totals == null) return default(TimeSpan);
				var docAbsence = DayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.DocumentAbsence);
				return docAbsence != null ? docAbsence.TimeSpan : default(TimeSpan);
			}
		}

		private bool _isShowOnlyScheduledIntervals;

		public bool IsShowOnlyScheduledIntervals
		{
			get { return _isShowOnlyScheduledIntervals; }
			set
			{
				_isShowOnlyScheduledIntervals = value;
				OnPropertyChanged(() => IsShowOnlyScheduledIntervals);
			}
		}

		private ICollectionView _dayTimeTrackPartsCollection;

		public ICollectionView DayTimeTrackPartsCollection
		{
			get { return _dayTimeTrackPartsCollection; }
			set
			{
				if (_dayTimeTrackPartsCollection == value) return;

				_dayTimeTrackPartsCollection = value;
				OnPropertyChanged(() => DayTimeTrackPartsCollection);
			}
		}

		private IDisposable _subscriber;

		#endregion

		#region Constructors
		public TimeTrackDetailsViewModel(DayTimeTrack dayTimeTrack, ShortEmployee shortEmployee)
		{
			//if (string.IsNullOrEmpty(dayTimeTrack.Error))
			//	dayTimeTrack.Calculate();
			Title = "Время сотрудника " + shortEmployee.FIO + " в течение дня " + dayTimeTrack.Date.Date.ToString("yyyy-MM-dd");
			AddDocumentCommand = new RelayCommand(OnAddDocument, CanAddDocument);
			EditDocumentCommand = new RelayCommand(OnEditDocument, CanEditDocument);
			RemoveDocumentCommand = new RelayCommand(OnRemoveDocument, CanRemoveDocument);
			AddFileCommand = new RelayCommand(OnAddFile);
			OpenFileCommand = new RelayCommand(OnOpenFile);
			RemoveFileCommand = new RelayCommand(OnRemoveFile);
			AddCustomPartCommand = new RelayCommand(OnAddCustomPart, CanAddPart);
			RemovePartCommand = new RelayCommand(OnRemovePart, CanRemovePart);
			EditPartCommand = new RelayCommand(OnEditPart, CanEditPart);
			ResetAdjustmentsCommand = new RelayCommand(OnResetAdjustments);
			ForceClosingCommand = new RelayCommand(OnForceClosing, CanForceClosing);
			RemovedDayTimeTrackParts = new List<DayTimeTrackPart>();
			DayTimeTrack = dayTimeTrack;
			ShortEmployee = shortEmployee;

			DayTimeTrackParts = GetObservableCollection(DayTimeTrack.RealTimeTrackParts, x => new DayTimeTrackPart(x, ShortEmployee));
			DayTimeTrackPartsCollection = CollectionViewSource.GetDefaultView(DayTimeTrackParts);
			DayTimeTrackPartsCollection.Filter = new Predicate<object>(FilterDayTimeTrackParts);
			Documents = GetObservableCollection(DayTimeTrack.Documents, x => new TimeTrackAttachedDocument(x));

			this.WhenAny(x => x.SelectedDayTimeTrackPart, x => x.Value)
				.Subscribe(value =>
				{
					if (value == null) return;

					if(_subscriber != null)
					_subscriber.Dispose();

					_subscriber = Observable.Merge(value.UIChanged).Subscribe(x =>
					{
						value.IsDirty = true;
						IsDirty = true;
					});
				});

			this.WhenAny(x => x.SelectedDocument, x => x.Value)
				.Subscribe(value =>
				{
					CanDoChanges = value != null && !value.HasFile;
				});

			this.WhenAny(x => x.IsShowOnlyScheduledIntervals, x => x.Value)
				.Subscribe(value => DayTimeTrackPartsCollection.Refresh());
		}

		#endregion

		#region Methods

		private bool FilterDayTimeTrackParts(object item)
		{
			var dayTimeTrackPart = item as DayTimeTrackPart;
			if (dayTimeTrackPart == null) return false;

			if (IsShowOnlyScheduledIntervals)
			{
				return dayTimeTrackPart.TimeTrackZone.IsURV;
			}
			return true;
		}

		private static ObservableCollection<T> GetObservableCollection<T, TU>(IEnumerable<TU> elements, Func<TU, T> del)
			where T : new()
		{
			var result = new ObservableCollection<T>();

			foreach (TU element in elements)
			{
				result.Add(del(element));
			}

			return result;
		}

		#endregion

		#region Commands

		public event EventHandler RefreshGridHandler;

		public RelayCommand AddCustomPartCommand { get; private set; }
		public RelayCommand RemovePartCommand { get; private set; }
		public RelayCommand EditPartCommand { get; private set; }
		public RelayCommand AddDocumentCommand { get; private set; }
		public RelayCommand EditDocumentCommand { get; private set; }
		public RelayCommand RemoveDocumentCommand { get; private set; }
		public RelayCommand AddFileCommand { get; private set; }
		public RelayCommand OpenFileCommand { get; private set; }
		public RelayCommand RemoveFileCommand { get; private set; }
		public RelayCommand ResetAdjustmentsCommand { get; private set; }
		public RelayCommand ForceClosingCommand { get; private set; }

		/// <summary>
		/// Функция создания прохода
		/// </summary>
		void OnAddCustomPart()
		{
			var timeTrackPartDetailsViewModel = new TimeTrackPartDetailsViewModel(DayTimeTrack, ShortEmployee, this);

			if (DialogService.ShowModalWindow(timeTrackPartDetailsViewModel))
			{
				timeTrackPartDetailsViewModel.CurrentTimeTrackPart.IsNew = true;
				timeTrackPartDetailsViewModel.CurrentTimeTrackPart.EnterTimeOriginal =
					timeTrackPartDetailsViewModel.CurrentTimeTrackPart.EnterDateTime;
				timeTrackPartDetailsViewModel.CurrentTimeTrackPart.ExitTimeOriginal =
					timeTrackPartDetailsViewModel.CurrentTimeTrackPart.ExitDateTime;


				SelectedTimeTrackPartDetailsViewModel = timeTrackPartDetailsViewModel.CurrentTimeTrackPart;

				if (timeTrackPartDetailsViewModel.CurrentTimeTrackPart.ExitDateTime.HasValue)
					DayTimeTrack.Date.Date.Add(timeTrackPartDetailsViewModel.CurrentTimeTrackPart.ExitDateTime.Value.TimeOfDay);

				IsDirty = true;
				ServiceFactoryBase.Events.GetEvent<EditTimeTrackPartEvent>().Publish(ShortEmployee.UID);

				if (RefreshGridHandler != null)
				{
					RefreshGridHandler(this, EventArgs.Empty);
				}

				DayTimeTrackParts.Add(timeTrackPartDetailsViewModel.CurrentTimeTrackPart);
			}
		}

		bool CanAddPart()
		{
			return FiresecManager.CheckPermission(FiresecAPI.Models.PermissionType.Oper_SKD_TimeTrack_Parts_Edit);
		}

		private void RemoveDayTimeTrack(DayTimeTrackPart removedDayTimeTrackPart)
		{
			RemovedDayTimeTrackParts.Add(removedDayTimeTrackPart);
			DayTimeTrackParts.Remove(removedDayTimeTrackPart);
			SelectedDayTimeTrackPart = DayTimeTrackParts.FirstOrDefault();
			IsDirty = true;
			ServiceFactoryBase.Events.GetEvent<EditTimeTrackPartEvent>().Publish(ShortEmployee.UID);
		}

		void OnRemovePart()
		{
			RemoveDayTimeTrack(SelectedDayTimeTrackPart);
		}

		public bool CanRemovePart()
		{
			return SelectedDayTimeTrackPart != null
				&& SelectedDayTimeTrackPart.IsManuallyAdded
				&& FiresecManager.CheckPermission(FiresecAPI.Models.PermissionType.Oper_SKD_TimeTrack_Parts_Edit);
		}

		bool CanEditPart()
		{
			return SelectedDayTimeTrackPart != null
				&& FiresecManager.CheckPermission(FiresecAPI.Models.PermissionType.Oper_SKD_TimeTrack_Parts_Edit)
				&& !SelectedDayTimeTrackPart.IsOpen;
		}

		void OnEditPart()
		{
			var timeTrackPartDetailsViewModel = new TimeTrackPartDetailsViewModel(DayTimeTrack, ShortEmployee, this,
				SelectedDayTimeTrackPart);

			if (DialogService.ShowModalWindow(timeTrackPartDetailsViewModel))
			{
				SelectedTimeTrackPartDetailsViewModel = timeTrackPartDetailsViewModel.DayTimeTrackPart; //timeTrackPartDetailsViewModel.CurrentTimeTrackPart;

				timeTrackPartDetailsViewModel.CurrentTimeTrackPart.IsNew = default(bool);

				IsDirty = true;
				ServiceFactoryBase.Events.GetEvent<EditTimeTrackPartEvent>().Publish(ShortEmployee.UID);
			}
		}

		void OnAddDocument()
		{
			var documentDetailsViewModel = new DocumentDetailsViewModel(false, ShortEmployee.OrganisationUID, ShortEmployee.UID)
			{
				StartDateTime = DayTimeTrack.Date.Date,
				EndDateTime = DayTimeTrack.Date.Date,
				EndTime = new TimeSpan(23, 59, 59),
				DocumentDateTime = DayTimeTrack.Date.Date
			};

			if (DialogService.ShowModalWindow(documentDetailsViewModel))
			{
				var operationResult = FiresecManager.FiresecService.AddTimeTrackDocument(documentDetailsViewModel.TimeTrackDocument);
				if (operationResult.HasError)
				{
					MessageBoxService.ShowWarning(operationResult.Error);
				}
				else
				{
					var documentViewModel = new TimeTrackAttachedDocument(documentDetailsViewModel.TimeTrackDocument);
					Documents.Add(documentViewModel);
					SelectedDocument = documentViewModel;
					IsDirty = true;
					ServiceFactoryBase.Events.GetEvent<EditDocumentEvent>().Publish(documentDetailsViewModel.TimeTrackDocument);
				}
			}
		}

		bool CanAddDocument()
		{
			return FiresecManager.CheckPermission(FiresecAPI.Models.PermissionType.Oper_SKD_TimeTrack_Documents_Edit);
		}

		void OnEditDocument()
		{
			var documentDetailsViewModel = new DocumentDetailsViewModel(false, ShortEmployee.OrganisationUID, ShortEmployee.UID,
				SelectedDocument.Document)
			{
				SelectedDocumentType = SelectedDocument.Document.TimeTrackDocumentType.DocumentType
			};
			if (DialogService.ShowModalWindow(documentDetailsViewModel))
			{
				var operationResult = FiresecManager.FiresecService.EditTimeTrackDocument(documentDetailsViewModel.TimeTrackDocument);
				if (operationResult.HasError)
				{
					MessageBoxService.ShowWarning(operationResult.Error);
				}
				ServiceFactoryBase.Events.GetEvent<EditDocumentEvent>().Publish(documentDetailsViewModel.TimeTrackDocument);
				SelectedDocument.Update();
				IsDirty = true;
			}
		}

		bool CanEditDocument()
		{
			return SelectedDocument != null && SelectedDocument.Document.StartDateTime.Date == DayTimeTrack.Date.Date && FiresecManager.CheckPermission(FiresecAPI.Models.PermissionType.Oper_SKD_TimeTrack_Documents_Edit);
		}

		void OnRemoveDocument()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить документ?"))
			{
				var operationResult = FiresecManager.FiresecService.RemoveTimeTrackDocument(SelectedDocument.Document.UID);
				if (operationResult.HasError)
				{
					MessageBoxService.ShowWarning(operationResult.Error);
				}
				else
				{
					ServiceFactoryBase.Events.GetEvent<RemoveDocumentEvent>().Publish(SelectedDocument.Document);
					Documents.Remove(SelectedDocument);
					IsDirty = true;
				}
			}
		}

		bool CanRemoveDocument()
		{
			return SelectedDocument != null && FiresecManager.CheckPermission(FiresecAPI.Models.PermissionType.Oper_SKD_TimeTrack_Documents_Edit);
		}

		void OnAddFile()
		{
			SelectedDocument.AddFile();
		}

		void OnOpenFile()
		{
			SelectedDocument.OpenFile();
		}

		void OnRemoveFile()
		{
			SelectedDocument.RemoveFile();
		}

		protected override bool CanSave()
		{
			return true;
		}

		protected override bool Cancel()
		{
			if (CanCancel)
			{
				AllowClose = true;
				return base.Cancel();
			}

			AllowClose = false;
			return true;
		}

		public bool CanCancel
		{
			get { return !IsDirty || MessageBoxService.ShowQuestion(Resources.CanCancelEditIntervals); }
		}

		protected override bool Save()
		{
			return PassJournalHelper.SaveAllTimeTracks(DayTimeTrackParts.Where(x => x.IsNew || x.IsDirty).Select(el => el.ToDTO()), ShortEmployee, FiresecManager.CurrentUser, RemovedDayTimeTrackParts.Select(x => x.ToDTO()));
		}

		private static bool ShowResetAdjustmentsWarning()
		{
			return MessageBoxService.ShowQuestion(Resources.ResetAdjustmentsQuestion);
		}

		public void OnResetAdjustments()
		{
			if (!ShowResetAdjustmentsWarning()) return;

			ClearIntervalsData(DayTimeTrackParts);
			var missedIntervals = PassJournalHelper.GetMissedIntervals(DayTimeTrack.Date, ShortEmployee).Select(x => new DayTimeTrackPart(x)).ToList();
			foreach (var dayTimeTrackPart in missedIntervals)
			{
				dayTimeTrackPart.TimeTrackZone =
					TimeTrackingHelper.GetMergedZones(ShortEmployee).FirstOrDefault(x => x.UID == dayTimeTrackPart.TimeTrackZone.UID);

				DayTimeTrackParts.Add(dayTimeTrackPart);
			}

			if (!DayTimeTrackParts.Any()) return;

			List<DayTimeTrackPart> collection = DayTimeTrackParts.Where(x => !x.IsForceClosed).ToList();

			var conflictIntervals = PassJournalHelper.FindConflictIntervals(collection.Select(dayTimeTrackPart => dayTimeTrackPart.ToDTO()).ToList(), ShortEmployee.UID, DayTimeTrack.Date);

			if (!conflictIntervals.IsNotNullOrEmpty())
			{
				DayTimeTrackParts.Where(x => !x.IsForceClosed).ForEach(ResetInputInterval);
				return;
			}

			Dictionary<DayTimeTrackPart, List<DayTimeTrackPart>> clienDictionary = conflictIntervals
				.ToDictionary(dayTimeTrackPart => new DayTimeTrackPart(dayTimeTrackPart.Key),
													dayTimeTrackPart => dayTimeTrackPart.Value.Select(timeTrackPart => new DayTimeTrackPart(timeTrackPart)).ToList()
							);

			foreach (var dayTimeTrackPart in DayTimeTrackParts.Where(x => !x.IsForceClosed))
			{
				DayTimeTrackPart part = dayTimeTrackPart;
				var conflictedIntervals = clienDictionary.FirstOrDefault(x => x.Key.UID == part.UID);

				if (conflictedIntervals.Key == null)
				{
					ResetInputInterval(dayTimeTrackPart);
				}
				else
				{
					ResolveConflicts(conflictedIntervals, dayTimeTrackPart);
				}
			}
		}

		private static void ResolveConflicts(KeyValuePair<DayTimeTrackPart, List<DayTimeTrackPart>> conflictedIntervals, DayTimeTrackPart originalDayTimeTrackPart)
		{
			var conflictViewModel = new ResetAdjustmentsConflictDialogWindowViewModel();
			conflictViewModel.SetValues(conflictedIntervals);

			DialogService.ShowModalWindow(conflictViewModel);

			switch (conflictViewModel.DialogResult)
			{
				case true:
					TimeTrackingHelper.ResolveConflictWithSettingBorders(originalDayTimeTrackPart, conflictedIntervals.Value);
					break;
				case false:
					originalDayTimeTrackPart.IsRemoveAllIntersections = true;
					originalDayTimeTrackPart.EnterDateTime = originalDayTimeTrackPart.EnterTimeOriginal;
					originalDayTimeTrackPart.ExitDateTime = originalDayTimeTrackPart.ExitTimeOriginal;
					originalDayTimeTrackPart.IsNeedAdjustment = originalDayTimeTrackPart.IsNeedAdjustmentOriginal;
					originalDayTimeTrackPart.NotTakeInCalculations = originalDayTimeTrackPart.TimeTrackZone != null && originalDayTimeTrackPart.TimeTrackZone.IsURV
																	? originalDayTimeTrackPart.NotTakeInCalculationsOriginal
																	: originalDayTimeTrackPart.NotTakeInCalculations;
					originalDayTimeTrackPart.AdjustmentDate = null;
					originalDayTimeTrackPart.CorrectedBy = null;
					originalDayTimeTrackPart.CorrectedByUID = null;
					originalDayTimeTrackPart.IsNew = default(bool);
					break;
			}
		}

		private void ResetInputInterval(DayTimeTrackPart inputInterval)
		{
			inputInterval.AdjustmentDate = null;
			inputInterval.CorrectedByUID = null;
			inputInterval.CorrectedBy = null;
			inputInterval.EnterDateTime = inputInterval.EnterTimeOriginal;
			inputInterval.ExitDateTime = inputInterval.ExitTimeOriginal;
			inputInterval.IsNeedAdjustment = inputInterval.IsNeedAdjustmentOriginal;
			inputInterval.NotTakeInCalculations = inputInterval.TimeTrackZone != null && inputInterval.TimeTrackZone.IsURV
												? inputInterval.NotTakeInCalculationsOriginal
												: inputInterval.NotTakeInCalculations;
			inputInterval.IsDirty = true;
		}

		private void ClearIntervalsData(ObservableCollection<DayTimeTrackPart> dayTimeTrackParts)
		{
			var searchCollection = new List<DayTimeTrackPart>(dayTimeTrackParts);
			foreach (var dayTimeTrackPart in searchCollection.Where(x => !x.IsForceClosed))
			{
				if (dayTimeTrackPart.IsManuallyAdded)
				{
					RemoveDayTimeTrack(dayTimeTrackPart);
				}
				else
				{
					IsDirty = true;
					dayTimeTrackPart.IsDirty = true;
					dayTimeTrackPart.AdjustmentDate = null;
					dayTimeTrackPart.CorrectedBy = null;
					dayTimeTrackPart.CorrectedByUID = null;
					dayTimeTrackPart.NotTakeInCalculations = dayTimeTrackPart.TimeTrackZone != null && dayTimeTrackPart.TimeTrackZone.IsURV
															? dayTimeTrackPart.NotTakeInCalculationsOriginal
															: dayTimeTrackPart.NotTakeInCalculations;
				}
			}
		}

		private void ResetAdjustmentsNoConflict()
		{
			foreach (var dayTimeTrack in DayTimeTrackParts)
			{
				dayTimeTrack.AdjustmentDate = null;
				dayTimeTrack.CorrectedByUID = null;
				dayTimeTrack.CorrectedBy = null;
				dayTimeTrack.EnterDateTime = dayTimeTrack.EnterTimeOriginal;
				dayTimeTrack.ExitDateTime = dayTimeTrack.ExitTimeOriginal;
				dayTimeTrack.IsNeedAdjustment = dayTimeTrack.IsNeedAdjustmentOriginal;
				dayTimeTrack.IsDirty = true;
			}
		}

		public void OnForceClosing()
		{
			if (!DialogService.ShowModalWindow(new ForceClosingQuestionDialogWindowViewModel())
			    || !PassJournalHelper.CheckForCanForseCloseInterval(SelectedDayTimeTrackPart.UID))
			{
				MessageBoxService.ShowConfirmation(Resources.ForceClosingFailedMessage);
				return;
			}

			var nowDateTime = DateTime.Now;
			SelectedDayTimeTrackPart.ExitDateTime = nowDateTime;
			SelectedDayTimeTrackPart.AdjustmentDate = nowDateTime;
			SelectedDayTimeTrackPart.IsForceClosed = true;
			SelectedDayTimeTrackPart.CorrectedBy = FiresecManager.CurrentUser.Name;
			SelectedDayTimeTrackPart.CorrectedByUID = FiresecManager.CurrentUser.UID;
			SelectedDayTimeTrackPart.NotTakeInCalculations = !SelectedDayTimeTrackPart.TimeTrackZone.IsURV;
		}

		public bool CanForceClosing()
		{
			return SelectedDayTimeTrackPart != null && SelectedDayTimeTrackPart.IsOpen;
		}

		#endregion
	}
}