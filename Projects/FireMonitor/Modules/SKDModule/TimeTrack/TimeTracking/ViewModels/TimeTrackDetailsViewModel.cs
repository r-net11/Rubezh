using System.Reactive.Linq;
using FiresecAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ReactiveUI;
using SKDModule.Events;
using SKDModule.Helpers;
using SKDModule.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using DayTimeTrackPart = SKDModule.Model.DayTimeTrackPart;

namespace SKDModule.ViewModels
{
	public class TimeTrackDetailsViewModel : SaveCancelDialogViewModel
	{
		#region Properties

		TimeTrackAttachedDocument _selectedDocument;
		public TimeTrackAttachedDocument SelectedDocument
		{
			get { return _selectedDocument; }
			set
			{
				_selectedDocument = value;
				OnPropertyChanged(() => SelectedDocument);
			}
		}

		DayTimeTrackPart _selectedDayTimeTrackPart; //TODO: remove it and save all DayTimeTrackParts
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

		public TimeSpan PresenceInBreakTimeSpan
		{
			get
			{
				if (DayTimeTrack == null || DayTimeTrack.Totals == null) return default(TimeSpan);
				var presInBreak = DayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.PresenceInBrerak);
				return presInBreak != null ? presInBreak.TimeSpan : default(TimeSpan);
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
			RemovePartCommand = new RelayCommand(OnRemovePart, CanEditRemovePart);
			EditPartCommand = new RelayCommand(OnEditPart, CanEditRemovePart);
			ResetAdjustmentsCommand = new RelayCommand(OnResetAdjustments);
			DayTimeTrack = dayTimeTrack;
			ShortEmployee = shortEmployee;

			DayTimeTrackParts = GetObservableCollection(DayTimeTrack.RealTimeTrackParts, x => new DayTimeTrackPart(x, ShortEmployee));
			DayTimeTrackPartsCollection = CollectionViewSource.GetDefaultView(DayTimeTrackParts);
			DayTimeTrackPartsCollection.Filter = new Predicate<object>(FilterDayTimeTrackParts);
			Documents = GetObservableCollection(DayTimeTrack.Documents, x => new TimeTrackAttachedDocument(x));

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

		/// <summary>
		/// Функция создания прохода
		/// </summary>
		void OnAddCustomPart()
		{
			var timeTrackPartDetailsViewModel = new TimeTrackPartDetailsViewModel(DayTimeTrack, ShortEmployee, this);

			if (DialogService.ShowModalWindow(timeTrackPartDetailsViewModel))
			{
				timeTrackPartDetailsViewModel.CurrentTimeTrackPart.IsNew = true;
				DayTimeTrackParts.Add(timeTrackPartDetailsViewModel.CurrentTimeTrackPart);
				SelectedTimeTrackPartDetailsViewModel = timeTrackPartDetailsViewModel.CurrentTimeTrackPart;

				if (timeTrackPartDetailsViewModel.CurrentTimeTrackPart.ExitDateTime.HasValue)
					DayTimeTrack.Date.Date.Add(timeTrackPartDetailsViewModel.CurrentTimeTrackPart.ExitDateTime.Value.TimeOfDay);

				IsDirty = true;
				ServiceFactoryBase.Events.GetEvent<EditTimeTrackPartEvent>().Publish(ShortEmployee.UID);

				if (RefreshGridHandler != null)
				{
					RefreshGridHandler(this, EventArgs.Empty);
				}
			}
		}

		bool CanAddPart()
		{
			return FiresecManager.CheckPermission(FiresecAPI.Models.PermissionType.Oper_SKD_TimeTrack_Parts_Edit);
		}

		void OnRemovePart()
		{
		//	var result = PassJournalHelper.DeleteAllPassJournalItems(SelectedDayTimeTrackPart.UID, DayTimeTrack.Date + SelectedDayTimeTrackPart.EnterTimeSpan, DayTimeTrack.Date + SelectedDayTimeTrackPart.ExitTimeSpan);
			var result = PassJournalHelper.DeleteAllPassJournalItems(SelectedDayTimeTrackPart.UID, SelectedDayTimeTrackPart.EnterDateTime.GetValueOrDefault(), SelectedDayTimeTrackPart.ExitDateTime.GetValueOrDefault());
			if (result)
			{
				DayTimeTrackParts.Remove(SelectedDayTimeTrackPart);
				SelectedDayTimeTrackPart = DayTimeTrackParts.FirstOrDefault();
				IsDirty = true;
				ServiceFactory.Events.GetEvent<EditTimeTrackPartEvent>().Publish(ShortEmployee.UID);
			}
		}
		bool CanEditRemovePart()
		{
			return SelectedDayTimeTrackPart != null && FiresecManager.CheckPermission(FiresecAPI.Models.PermissionType.Oper_SKD_TimeTrack_Parts_Edit);
		}

		void OnEditPart()
		{
			var timeTrackPartDetailsViewModel = new TimeTrackPartDetailsViewModel(DayTimeTrack, ShortEmployee, this,
				SelectedDayTimeTrackPart);

			if (DialogService.ShowModalWindow(timeTrackPartDetailsViewModel))
			{
				SelectedTimeTrackPartDetailsViewModel = timeTrackPartDetailsViewModel.CurrentTimeTrackPart;
				timeTrackPartDetailsViewModel.CurrentTimeTrackPart.IsNew = default(bool);
				//SelectedDayTimeTrackPart
				//	.Update(
				//	timeTrackPartDetailsViewModel.CurrentTimeTrackPart.EnterDateTime + timeTrackPartDetailsViewModel.EnterTime,
				//	timeTrackPartDetailsViewModel.ExitDateTime + timeTrackPartDetailsViewModel.ExitTime,
				//	timeTrackPartDetailsViewModel.SelectedZone,
				//	timeTrackPartDetailsViewModel.NotTakeInCalculations,
				//	timeTrackPartDetailsViewModel.IsManuallyAdded,
				//	DateTime.Now.ToString(CultureInfo.CurrentUICulture),
				//	FiresecManager.CurrentUser.Name);

				IsDirty = true;
				ServiceFactory.Events.GetEvent<EditTimeTrackPartEvent>().Publish(ShortEmployee.UID);
			}
		}

		void OnAddDocument()
		{
			var timeTrackDocument = new TimeTrackDocument
			{
				StartDateTime = DayTimeTrack.Date.Date,
				EndDateTime = DayTimeTrack.Date.Date + new TimeSpan(23, 59, 59)
			};

			var documentDetailsViewModel = new DocumentDetailsViewModel(false, ShortEmployee.OrganisationUID, timeTrackDocument);

			if (DialogService.ShowModalWindow(documentDetailsViewModel))
			{
				documentDetailsViewModel.TimeTrackDocument.EmployeeUID = ShortEmployee.UID;

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
					ServiceFactory.Events.GetEvent<EditDocumentEvent>().Publish(documentDetailsViewModel.TimeTrackDocument);
				}
			}
		}

		bool CanAddDocument()
		{
			return FiresecManager.CheckPermission(FiresecAPI.Models.PermissionType.Oper_SKD_TimeTrack_Documents_Edit);
		}

		void OnEditDocument()
		{
			var documentDetailsViewModel = new DocumentDetailsViewModel(false, ShortEmployee.OrganisationUID, SelectedDocument.Document);
			if (DialogService.ShowModalWindow(documentDetailsViewModel))
			{
				var document = documentDetailsViewModel.TimeTrackDocument;
				var operationResult = FiresecManager.FiresecService.EditTimeTrackDocument(document);
				if (operationResult.HasError)
				{
					MessageBoxService.ShowWarning(operationResult.Error);
				}
				ServiceFactory.Events.GetEvent<EditDocumentEvent>().Publish(document);
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
					ServiceFactory.Events.GetEvent<RemoveDocumentEvent>().Publish(SelectedDocument.Document);
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

		protected override bool Save() //TODO: Save all TimeTracks without refresh
		{
			var serverCollection = new List<FiresecAPI.SKD.DayTimeTrackPart>();
			foreach (var el in DayTimeTrackParts.Where(x => x.IsNew || x.IsDirty))
			{
				serverCollection.Add(el.ToDTO());
				//serverCollection.Add(new FiresecAPI.SKD.DayTimeTrackPart
				//{
				//	CorrectedBy = el.CorrectedBy,
				//	CorrectedByUID = el.CorrectedByUID,
				//	CorrectedDate = el.CorrectedDate,
				//	EnterDateTime = el.EnterDateTime,
				//	EnterTime = el.EnterTime,
				//	EnterTimeOriginal = el.EnterTimeOriginal,
				//	ExitDateTime = el.ExitDateTime,
				//	AdjustmentDate = el.AdjustmentDate,
				//	ExitTime = el.ExitTime,
				//	ExitTimeOriginal = el.ExitTimeOriginal,
				//	IsDirty = el.IsDirty,
				//	IsForceClosed = el.IsForceClosed,
				//	IsManuallyAdded = el.IsManuallyAdded,
				//	IsNeedAdjustment = el.IsNeedAdjustment,
				//	IsNew = el.IsNew,
				//	IsOpen = el.IsOpen,
				//	IsRemoveAllIntersections = el.IsRemoveAllIntersections,
				//	NotTakeInCalculations = el.NotTakeInCalculations,
				//	UID = el.UID,
				//	TimeTrackZone = new FiresecAPI.SKD.TimeTrackZone
				//	{
				//		Description = el.TimeTrackZone.Description,
				//		IsURV = el.TimeTrackZone.IsURV,
				//		Name = el.TimeTrackZone.Name,
				//		No = el.TimeTrackZone.No,
				//		SKDZone = el.TimeTrackZone.SKDZone,
				//		UID = el.TimeTrackZone.UID
				//	}
				//});
			}

			PassJournalHelper.SaveAllTimeTracks(serverCollection, ShortEmployee);

			return base.Save();
		}

		private static bool ShowResetAdjustmentsWarning()
		{
			return MessageBoxService.ShowQuestion(
				"При выполнении сброса корректировок все введенные вручную данные будут удалены. Продолжить?");
		}

		public void OnResetAdjustments()
		{
			if (!ShowResetAdjustmentsWarning()) return;

			var resultCollection = DayTimeTrackParts.Where(x => !x.IsManuallyAdded).ToList();
			List<DayTimeTrackPart> collection = DayTimeTrackParts.Where(x => !string.IsNullOrEmpty(x.CorrectedBy) && !x.IsForceClosed).ToList();
			//TODO: remove all manuall added intervals
			var serverCollection = new List<FiresecAPI.SKD.DayTimeTrackPart>();
			foreach (var dayTimeTrackPart in collection)
			{
				serverCollection.Add(dayTimeTrackPart.ToDTO());
			}

			var conflictIntervals = PassJournalHelper.FindConflictIntervals(serverCollection, ShortEmployee.UID, DayTimeTrack.Date);
			Dictionary<DayTimeTrackPart, List<DayTimeTrackPart>> clienDictionary = new Dictionary<DayTimeTrackPart, List<DayTimeTrackPart>>();

			if (conflictIntervals == null) return;

			foreach (KeyValuePair<FiresecAPI.SKD.DayTimeTrackPart, List<FiresecAPI.SKD.DayTimeTrackPart>> dayTimeTrackPart in conflictIntervals)
			{
				var key = new DayTimeTrackPart(dayTimeTrackPart.Key);

				List<DayTimeTrackPart> values = new List<DayTimeTrackPart>();

				foreach (var timeTrackPart in dayTimeTrackPart.Value)
				{
					values.Add(new DayTimeTrackPart(timeTrackPart));
				}
				clienDictionary.Add(key, values);
			}

			var conflictViewModel = new ResetAdjustmentsConflictDialogWindowViewModel();
			var resolvedConflictCollection = new List<DayTimeTrackPart>();
			foreach (KeyValuePair<DayTimeTrackPart, List<DayTimeTrackPart>> el in clienDictionary)
			{
				conflictViewModel.SetValues(el);

				if (conflictViewModel.IsCheckedSave)
				{
					resolvedConflictCollection.AddRange(resultCollection
														.Where(dayTimeTrackPart => dayTimeTrackPart.UID == el.Key.UID)
														.Select(dayTimeTrackPart => TimeTrackingHelper.ResolveConflictWithSettingBorders(dayTimeTrackPart, el.Value)));
					//TODO:Set borders without intersection
					continue;
				}

				if (conflictViewModel.IsCheckedCancel)
				{
					resolvedConflictCollection.AddRange(resultCollection
														.Where(dayTimeTrackPart => dayTimeTrackPart.UID == el.Key.UID)
														.Select(
															dayTimeTrackPart =>
															{
																dayTimeTrackPart.IsRemoveAllIntersections = true;
																dayTimeTrackPart.EnterDateTime = dayTimeTrackPart.EnterTimeOriginal;
																dayTimeTrackPart.ExitDateTime = dayTimeTrackPart.ExitTimeOriginal;
																dayTimeTrackPart.IsNew = default(bool);
																return dayTimeTrackPart;
															})
														);
					//TODO:Remove manually added interval
					continue;
				}

				if (DialogService.ShowModalWindow(conflictViewModel))
				{
					//TODO:Set borders without intersection
					resolvedConflictCollection.AddRange(resultCollection
														.Where(dayTimeTrackPart => dayTimeTrackPart.UID == el.Key.UID)
														.Select(dayTimeTrackPart => TimeTrackingHelper.ResolveConflictWithSettingBorders(dayTimeTrackPart, el.Value)));
				}
				else
				{
					//TODO:Remove manually added interval
					resolvedConflictCollection.AddRange(resultCollection
														.Where(dayTimeTrackPart => dayTimeTrackPart.UID == el.Key.UID)
														.Select(
															dayTimeTrackPart =>
															{
																dayTimeTrackPart.IsRemoveAllIntersections = true;
																dayTimeTrackPart.EnterDateTime = dayTimeTrackPart.EnterTimeOriginal;
																dayTimeTrackPart.ExitDateTime = dayTimeTrackPart.ExitTimeOriginal;
																dayTimeTrackPart.IsNew = default(bool);
																return dayTimeTrackPart;
															})
														);
				}
			}

			var hyperResultCollection = new List<DayTimeTrackPart>();
			foreach (var dayTimeTrackPart in DayTimeTrackParts)
			{
				var tmp = resolvedConflictCollection.FirstOrDefault(x => x.UID == dayTimeTrackPart.UID);
				if (tmp != null)
				{
					dayTimeTrackPart.EnterDateTime = tmp.EnterDateTime;
					dayTimeTrackPart.ExitDateTime = tmp.ExitDateTime;
					dayTimeTrackPart.IsDirty = true;
				}

				hyperResultCollection.Add(dayTimeTrackPart);
			}

			DayTimeTrackParts = new ObservableCollection<DayTimeTrackPart>(hyperResultCollection);
		}

		#endregion
	}
}