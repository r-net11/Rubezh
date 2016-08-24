using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using EntitiesValidation;
using Localization.SKD.ViewModels;
using StrazhAPI;
using StrazhAPI.SKD;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class ScheduleSchemeViewModel : OrganisationElementViewModel<ScheduleSchemeViewModel, ScheduleScheme>
	{
		bool _isInitialized;
		public ScheduleSchemesViewModel ScheduleSchemesViewModel;
		
		public override void InitializeOrganisation(Organisation organisation, ViewPartViewModel parentViewModel)
		{
			base.InitializeOrganisation(organisation, parentViewModel);
			ScheduleSchemesViewModel = (parentViewModel as ScheduleSchemesViewModel);
			_isInitialized = true;
		}

		public override void InitializeModel(Organisation organisation, ScheduleScheme model, ViewPartViewModel parentViewModel)
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);
			ScheduleSchemesViewModel = (parentViewModel as ScheduleSchemesViewModel);
			base.InitializeModel(organisation, model, parentViewModel);
			_isInitialized = false;
		}

		public void Initialize()
		{
			if (!_isInitialized)
			{
				_isInitialized = true;
				if (!IsOrganisation)
					SheduleDayIntervals = new SortableObservableCollection<SheduleDayIntervalViewModel>(Model.DayIntervals.Select(item => new SheduleDayIntervalViewModel(this, item)));
			}
			OnPropertyChanged(() => DayIntervals);
		}

		public void UpdateDayIntervals(DayInterval dayInterval)
		{
			var dayIntervals = SheduleDayIntervals.Where(x => x.SelectedDayInterval.UID == dayInterval.UID);

		}

		public SortableObservableCollection<SheduleDayIntervalViewModel> SheduleDayIntervals { get; private set; }
		public ObservableCollection<DayInterval> DayIntervals
		{
			get { return ScheduleSchemesViewModel.GetDayIntervals(Organisation.UID, Model.Type); }
		}

		public bool IsSlide
		{
			get { return Model != null && Model.Type == ScheduleSchemeType.SlideDay; }
		}

		public string Type
		{
			get { return IsOrganisation? "" : Model.Type.ToDescription(); }
		}

		SheduleDayIntervalViewModel _selectedDayInterval;
		public SheduleDayIntervalViewModel SelectedSheduleDayInterval
		{
			get { return _selectedDayInterval; }
			set
			{
				_selectedDayInterval = value;
				OnPropertyChanged(() => SelectedSheduleDayInterval);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var dayInterval = new ScheduleDayInterval()
			{
				Number = Model.DayIntervals.Count,
				ScheduleSchemeUID = Model.UID,
				DayIntervalUID = Guid.Empty,
			};
			if (SheduleDayIntervalHelper.Save(dayInterval, Model.Name))
			{
				Model.DayIntervals.Add(dayInterval);
				var viewModel = new SheduleDayIntervalViewModel(this, dayInterval);
				SheduleDayIntervals.Add(viewModel);
				Sort();
				SelectedSheduleDayInterval = viewModel;
				UpdateDaysCount();
			}
		}
		bool CanAdd()
		{
			return IsSlide && !IsDeleted && FiresecManager.CheckPermission(StrazhAPI.Models.PermissionType.Oper_SKD_TimeTrack_ScheduleSchemes_Edit);
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			// Выполняем валидацию схемы графика работ с типом "Сменный"
			if (Model.Type == ScheduleSchemeType.SlideDay)
			{
				var validationResult = ValidateDayIntervalsIntersection();
				if (validationResult.HasError)
				{
					MessageBoxService.ShowWarning(String.Format("{0}\n\n{1}", CommonViewModels.CanNotDeleteDaySchedule, validationResult.Error), null, 420, 200);
					return;
				}
			}

			var number = SelectedSheduleDayInterval.Model.Number;
			if (SheduleDayIntervalHelper.Remove(SelectedSheduleDayInterval.Model, Model.Name))
			{
				for (int i = number + 1; i < Model.DayIntervals.Count; i++)
					Model.DayIntervals[i].Number--;
				Model.DayIntervals.Remove(SelectedSheduleDayInterval.Model);
				SheduleDayIntervals.Remove(SelectedSheduleDayInterval);
				SheduleDayIntervals.ForEach(item => item.Update());
				SelectedSheduleDayInterval = number < SheduleDayIntervals.Count ? SheduleDayIntervals[number] : SheduleDayIntervals.LastOrDefault();
				UpdateDaysCount();
			}
		}
		bool CanDelete()
		{
			return IsSlide && SelectedSheduleDayInterval != null && SheduleDayIntervals.Count > 1 && !IsDeleted && FiresecManager.CheckPermission(StrazhAPI.Models.PermissionType.Oper_SKD_TimeTrack_ScheduleSchemes_Edit);
		}

		void Sort()
		{
			SheduleDayIntervals.Sort(item => item.Model.Number);
		}

		void UpdateDaysCount()
		{
			Model.DaysCount = Model.DayIntervals.Count;
			ScheduleSchemeHelper.Save(Model, false);
		}

		private OperationResult ValidateDayIntervalsIntersection()
		{
			var dayIntervals = (
				from scheduleDayInterval in SheduleDayIntervals
				where scheduleDayInterval != SelectedSheduleDayInterval
				select scheduleDayInterval.SelectedDayInterval).ToList();

			var validationResult = ScheduleSchemeValidator.ValidateDayIntervalsIntersecion(dayIntervals);
			if (validationResult.HasError)
				return new OperationResult(validationResult.Error);

			// Нет пересечений
			return new OperationResult();
		}
	}
}