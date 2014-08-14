using System;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.TreeList;

namespace SKDModule.ViewModels
{
	public class ScheduleSchemeViewModel : TreeNodeViewModel<ScheduleSchemeViewModel>
	{
		private bool _isInitialized;
		public ScheduleSchemesViewModel ScheduleSchemesViewModel;
		public FiresecAPI.SKD.Organisation Organisation { get; private set; }
		public bool IsOrganisation { get; private set; }
		public ScheduleScheme ScheduleScheme { get; private set; }

		public ScheduleSchemeViewModel(ScheduleSchemesViewModel scheduleSchemesViewModel, FiresecAPI.SKD.Organisation organisation)
		{
			ScheduleSchemesViewModel = scheduleSchemesViewModel;
			Organisation = organisation;
			IsOrganisation = true;
			IsExpanded = true;
			_isInitialized = true;
		}
		public ScheduleSchemeViewModel(ScheduleSchemesViewModel scheduleSchemesViewModel, FiresecAPI.SKD.Organisation organisation, ScheduleScheme scheduleScheme)
		{
			AddCommand = new RelayCommand(OnAdd);
			DeleteCommand = new RelayCommand(OnDelete, CanDelete);

			ScheduleSchemesViewModel = scheduleSchemesViewModel;
			Organisation = organisation;
			ScheduleScheme = scheduleScheme;
			IsOrganisation = false;
			Update();

			_isInitialized = false;
		}

		public void Initialize()
		{
			if (!_isInitialized)
			{
				_isInitialized = true;
				if (!IsOrganisation)
					SheduleDayIntervals = new SortableObservableCollection<SheduleDayIntervalViewModel>(ScheduleScheme.DayIntervals.Select(item => new SheduleDayIntervalViewModel(this, item)));
			}
			OnPropertyChanged(() => DayIntervals);
		}
		public void Update()
		{
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => Description);
		}
		public string Name
		{
			get { return IsOrganisation ? Organisation.Name : ScheduleScheme.Name; }
		}
		public string Description
		{
			get { return IsOrganisation ? Organisation.Description : ScheduleScheme.Description; }
		}

		public SortableObservableCollection<SheduleDayIntervalViewModel> SheduleDayIntervals { get; private set; }
		public ObservableCollection<DayInterval> DayIntervals
		{
			get { return ScheduleSchemesViewModel.GetDayIntervals(Organisation.UID); }
		}

		public bool IsSlide
		{
			get { return ScheduleScheme != null && ScheduleScheme.Type == ScheduleSchemeType.SlideDay; }
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
				Number = ScheduleScheme.DayIntervals.Count,
				ScheduleSchemeUID = ScheduleScheme.UID,
				DayIntervalUID = Guid.Empty,
			};
			if (SheduleDayIntervalHelper.Save(dayInterval))
			{
				ScheduleScheme.DayIntervals.Add(dayInterval);
				var viewModel = new SheduleDayIntervalViewModel(this, dayInterval);
				SheduleDayIntervals.Add(viewModel);
				Sort();
				SelectedSheduleDayInterval = viewModel;
			}
		}
		bool CanAdd()
		{
			return IsSlide;
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			var number = SelectedSheduleDayInterval.Model.Number;
			if (SheduleDayIntervalHelper.MarkDeleted(SelectedSheduleDayInterval.Model))
			{
				for (int i = number + 1; i < ScheduleScheme.DayIntervals.Count; i++)
					ScheduleScheme.DayIntervals[i].Number--;
				ScheduleScheme.DayIntervals.Remove(SelectedSheduleDayInterval.Model);
				SheduleDayIntervals.Remove(SelectedSheduleDayInterval);
				SheduleDayIntervals.ForEach(item => item.Update());
				SelectedSheduleDayInterval = number < SheduleDayIntervals.Count ? SheduleDayIntervals[number] : SheduleDayIntervals.LastOrDefault();
			}
		}
		bool CanDelete()
		{
			return IsSlide && SelectedSheduleDayInterval != null && SheduleDayIntervals.Count > 1;
		}

		void Sort()
		{
			SheduleDayIntervals.Sort(item => item.Model.Number);
		}
	}
}