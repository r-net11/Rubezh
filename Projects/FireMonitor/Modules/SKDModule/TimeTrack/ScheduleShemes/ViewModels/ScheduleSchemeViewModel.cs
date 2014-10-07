using System;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using FiresecAPI;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
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

		public SortableObservableCollection<SheduleDayIntervalViewModel> SheduleDayIntervals { get; private set; }
		public ObservableCollection<DayInterval> DayIntervals
		{
			get { return ScheduleSchemesViewModel.GetDayIntervals(Organisation.UID); }
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
			if (SheduleDayIntervalHelper.Save(dayInterval))
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
			return IsSlide && !IsDeleted;
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			var number = SelectedSheduleDayInterval.Model.Number;
			if (SheduleDayIntervalHelper.Remove(SelectedSheduleDayInterval.Model))
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
			return IsSlide && SelectedSheduleDayInterval != null && SheduleDayIntervals.Count > 1 && !IsDeleted;
		}

		void Sort()
		{
			SheduleDayIntervals.Sort(item => item.Model.Number);
		}

		void UpdateDaysCount()
		{
			Model.DaysCount = Model.DayIntervals.Count;
			ScheduleSchemaHelper.Save(Model);
		}
	}
}