using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using ReactiveUI;
using SKDModule.Events;
using SKDModule.Model;
using SKDModule.Properties;
using StrazhAPI.SKD;
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
		#region Fields
		private TimeTrackAttachedDocument _selectedDocument;
		private DayTimeTrackPart _selectedDayTimeTrackPart;
		private DayTimeTrackPart _selectedTimeTrackPartDetailsViewModel;
		bool _isDirty;
		private bool _isShowOnlyScheduledIntervals;
		private ICollectionView _dayTimeTrackPartsCollection;
		private IDisposable _subscriber;
		#endregion

		#region Properties
		public List<DayTimeTrackPart> RemovedDayTimeTrackParts { get; set; }

		public TimeTrackAttachedDocument SelectedDocument
		{
			get { return _selectedDocument; }
			set
			{
				_selectedDocument = value;
				OnPropertyChanged(() => SelectedDocument);
			}
		}

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

		public bool IsDirty
		{
			get { return _isDirty; }
			set
			{
				_isDirty = value;
				OnPropertyChanged(() => IsDirty);
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

		public TimeSpan DocumentAbsenceReasonableTimeSpan
		{
			get
			{
				if (DayTimeTrack == null || DayTimeTrack.Totals == null) return default(TimeSpan);
				var docAbsenceReason = DayTimeTrack.Totals.FirstOrDefault(x => x.TimeTrackType == TimeTrackType.DocumentAbsenceReasonable);

				return docAbsenceReason != null ? docAbsenceReason.TimeSpan : default(TimeSpan);
			}
		}

		public TimeSpan SlideTimeValue
		{
			get
			{
				if (DayTimeTrack == null) return default(TimeSpan);

				if (DayTimeTrack.SlideTime == TimeSpan.Zero)
					return DayTimeTrack.PlannedTimeTrackParts.Aggregate(default(TimeSpan),
						(accumulate, part) => accumulate + ((part.TimeTrackPartType == TimeTrackType.Break) ?  TimeSpan.Zero : part.Delta)); //TODO: Remove chosing time-value logic

				return DayTimeTrack.SlideTime;
			}
		}

		public bool IsShowOnlyScheduledIntervals
		{
			get { return _isShowOnlyScheduledIntervals; }
			set
			{
				_isShowOnlyScheduledIntervals = value;
				OnPropertyChanged(() => IsShowOnlyScheduledIntervals);
			}
		}

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
		#endregion

		#region Constructors
		public TimeTrackDetailsViewModel(DayTimeTrack dayTimeTrack, ShortEmployee shortEmployee)
		{
			AllowClose = false; //Need to fix SKDDEV-839
			CloseOnEscape = false;

			Title = "Время сотрудника " + shortEmployee.FIO + " в течение дня " + dayTimeTrack.Date.Date.ToString("yyyy-MM-dd");
			AddDocumentCommand = new RelayCommand(OnAddDocument, () => FiresecManager.CheckPermission(StrazhAPI.Models.PermissionType.Oper_SKD_TimeTrack_Documents_Edit));
			EditDocumentCommand = new RelayCommand(OnEditDocument, CanEditDocument);
			RemoveDocumentCommand = new RelayCommand(OnRemoveDocument, CanRemoveDocument);
			AddFileCommand = new RelayCommand(OnAddFile, CanAddFile);
			OpenFileCommand = new RelayCommand(OnOpenFile, CanOpenOrRemoveFile);
			RemoveFileCommand = new RelayCommand(OnRemoveFile, CanOpenOrRemoveFile);
			AddCustomPartCommand = new RelayCommand(OnAddCustomPart, () => FiresecManager.CheckPermission(StrazhAPI.Models.PermissionType.Oper_SKD_TimeTrack_Parts_Edit));
			RemovePartCommand = new RelayCommand(OnRemovePart, CanRemovePart);
			EditPartCommand = new RelayCommand(OnEditPart, CanEditPart);
			ForceClosingCommand = new RelayCommand(OnForceClosing, () => SelectedDayTimeTrackPart != null && SelectedDayTimeTrackPart.IsOpen);
			RemovedDayTimeTrackParts = new List<DayTimeTrackPart>();
			DayTimeTrack = dayTimeTrack;
			ShortEmployee = shortEmployee;

			DayTimeTrackParts = GetObservableCollection(DayTimeTrack.RealTimeTrackParts, x => new DayTimeTrackPart(x, ShortEmployee));
			DayTimeTrackPartsCollection = CollectionViewSource.GetDefaultView(DayTimeTrackParts);
			DayTimeTrackPartsCollection.Filter = FilterDayTimeTrackParts;
			Documents = GetObservableCollection(DayTimeTrack.Documents, x => new TimeTrackAttachedDocument(x));

			IsShowOnlyScheduledIntervals = true;

			this.WhenAny(x => x.SelectedDayTimeTrackPart, x => x.Value)
				.Subscribe(value =>
				{
					if (value == null) return;

					if(_subscriber != null)
					_subscriber.Dispose();

					_subscriber = Observable.Merge(value.UIChanged)
						.Subscribe(x =>
						{
							value.IsDirty = true;
							IsDirty = true;
						});
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

		public RelayCommand AddCustomPartCommand { get; private set; }
		public RelayCommand RemovePartCommand { get; private set; }
		public RelayCommand EditPartCommand { get; private set; }
		public RelayCommand AddDocumentCommand { get; private set; }
		public RelayCommand EditDocumentCommand { get; private set; }
		public RelayCommand RemoveDocumentCommand { get; private set; }
		public RelayCommand AddFileCommand { get; private set; }
		public RelayCommand OpenFileCommand { get; private set; }
		public RelayCommand RemoveFileCommand { get; private set; }
		public RelayCommand ForceClosingCommand { get; private set; }

		void OnAddCustomPart()
		{
			var timeTrackPartDetailsViewModel = new TimeTrackPartDetailsViewModel(DayTimeTrack, ShortEmployee, this);

			if (DialogService.ShowModalWindow(timeTrackPartDetailsViewModel))
			{
				timeTrackPartDetailsViewModel.CurrentTimeTrackPart.IsNew = true;
				timeTrackPartDetailsViewModel.CurrentTimeTrackPart.TimeTrackActions |= TimeTrackActions.Adding;

				SelectedTimeTrackPartDetailsViewModel = timeTrackPartDetailsViewModel.CurrentTimeTrackPart;

				if (timeTrackPartDetailsViewModel.CurrentTimeTrackPart.ExitDateTime.HasValue)
					DayTimeTrack.Date.Date.Add(timeTrackPartDetailsViewModel.CurrentTimeTrackPart.ExitDateTime.Value.TimeOfDay);

				IsDirty = true;
				ServiceFactoryBase.Events.GetEvent<EditTimeTrackPartEvent>().Publish(ShortEmployee.UID);

				DayTimeTrackParts.Add(timeTrackPartDetailsViewModel.CurrentTimeTrackPart);
			}
		}

		private void RemoveDayTimeTrack(DayTimeTrackPart removedDayTimeTrackPart)
		{
			removedDayTimeTrackPart.TimeTrackActions |= TimeTrackActions.Remove;
			RemovedDayTimeTrackParts.Add(removedDayTimeTrackPart);
			DayTimeTrackParts.Remove(removedDayTimeTrackPart);
			SelectedDayTimeTrackPart = DayTimeTrackParts.FirstOrDefault();
			IsDirty = true;
			ServiceFactoryBase.Events.GetEvent<EditTimeTrackPartEvent>().Publish(ShortEmployee.UID);
		}

		private bool CanOpenOrRemoveFile()
		{
			return SelectedDocument != null && SelectedDocument.HasFile && FiresecManager.CheckPermission(StrazhAPI.Models.PermissionType.Oper_SKD_TimeTrack_Documents_Edit);
		}

		void OnRemovePart()
		{
			RemoveDayTimeTrack(SelectedDayTimeTrackPart);
		}

		private bool CanAddFile()
		{
			return SelectedDocument != null && SelectedDocument.Document != null && !SelectedDocument.HasFile;
		}

		public bool CanRemovePart()
		{
			return SelectedDayTimeTrackPart != null
				&& SelectedDayTimeTrackPart.IsManuallyAdded
				&& FiresecManager.CheckPermission(StrazhAPI.Models.PermissionType.Oper_SKD_TimeTrack_Parts_Edit);
		}

		bool CanEditPart()
		{
			return SelectedDayTimeTrackPart != null
				&& FiresecManager.CheckPermission(StrazhAPI.Models.PermissionType.Oper_SKD_TimeTrack_Parts_Edit)
				&& !SelectedDayTimeTrackPart.IsOpen;
		}

		void OnEditPart()
		{
			var timeTrackPartDetailsViewModel = new TimeTrackPartDetailsViewModel(DayTimeTrack, ShortEmployee, this,
				SelectedDayTimeTrackPart);

			if (DialogService.ShowModalWindow(timeTrackPartDetailsViewModel))
			{
				SelectedTimeTrackPartDetailsViewModel = timeTrackPartDetailsViewModel.DayTimeTrackPart; //timeTrackPartDetailsViewModel.CurrentTimeTrackPart; //TODO:try to refactor

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
			return SelectedDocument != null && FiresecManager.CheckPermission(StrazhAPI.Models.PermissionType.Oper_SKD_TimeTrack_Documents_Edit);
		}

		void OnRemoveDocument()
		{
			if (!MessageBoxService.ShowQuestion("Вы уверены, что хотите удалить документ?")) return;

			var operationResult = FiresecManager.FiresecService.RemoveTimeTrackDocument(SelectedDocument.Document.UID);

			if (operationResult.HasError)
				MessageBoxService.ShowWarning(operationResult.Error);
			else
			{
				ServiceFactoryBase.Events.GetEvent<RemoveDocumentEvent>().Publish(SelectedDocument.Document);
				Documents.Remove(SelectedDocument);
				IsDirty = true;
			}
		}

		bool CanRemoveDocument()
		{
			return SelectedDocument != null && FiresecManager.CheckPermission(StrazhAPI.Models.PermissionType.Oper_SKD_TimeTrack_Documents_Edit);
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
				return base.Cancel();

			return true;
		}

		public bool CanCancel
		{
			get { return !IsDirty || MessageBoxService.ShowQuestion(Resources.CanCancelEditIntervals); }
		}

		protected override bool Save()
		{
			return PassJournalHelper.SaveAllTimeTracks(DayTimeTrackParts
									.Where(x => x.IsNew || x.IsDirty)
									.Select(el => el.ToDTO()),
									ShortEmployee, FiresecManager.CurrentUser, RemovedDayTimeTrackParts.Select(x => x.ToDTO()));
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
			SelectedDayTimeTrackPart.TimeTrackActions |= TimeTrackActions.ForceClose;
		}

		#endregion
	}
}