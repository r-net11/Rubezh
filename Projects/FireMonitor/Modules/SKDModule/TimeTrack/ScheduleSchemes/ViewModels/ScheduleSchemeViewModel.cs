using System.Collections.ObjectModel;
using System.Linq;
using Common;
using RubezhAPI;
using RubezhAPI.SKD;
using RubezhClient;
using RubezhClient.SKDHelpers;
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
			var dayInterval = DayIntervals.FirstOrDefault(x=>x.Name=="Выходной");
			var scheduleDayInterval = new ScheduleDayInterval()
			{
				Number = Model.DayIntervals.Count,
				ScheduleSchemeUID = Model.UID,
				DayIntervalName = dayInterval.Name,
				DayIntervalUID = dayInterval.UID
			};
			if (AddSave(scheduleDayInterval))
			{
				var viewModel = new SheduleDayIntervalViewModel(this, scheduleDayInterval);
				SheduleDayIntervals.Add(viewModel);
				Sort();
				SelectedSheduleDayInterval = viewModel;
			}
		}
		bool CanAdd()
		{
			return IsSlide && 
				!IsDeleted && 
				ClientManager.CheckPermission(RubezhAPI.Models.PermissionType.Oper_SKD_TimeTrack_ScheduleSchemes_Edit) && 
				IsConnected;
		}

		public RelayCommand DeleteCommand { get; private set; }
		void OnDelete()
		{
			var number = SelectedSheduleDayInterval.Model.Number;
			if (DeleteSave(SelectedSheduleDayInterval.Model))
			{
				for (int i = number + 1; i < Model.DayIntervals.Count; i++)
					Model.DayIntervals[i].Number--;
				SheduleDayIntervals.Remove(SelectedSheduleDayInterval);
				SheduleDayIntervals.ForEach(item => item.Update());
				SelectedSheduleDayInterval = number < SheduleDayIntervals.Count ? SheduleDayIntervals[number] : SheduleDayIntervals.LastOrDefault();
			}
		}
		bool CanDelete()
		{
			return IsSlide && 
				SelectedSheduleDayInterval != null && 
				SheduleDayIntervals.Count > 1 && !IsDeleted && 
				ClientManager.CheckPermission(RubezhAPI.Models.PermissionType.Oper_SKD_TimeTrack_ScheduleSchemes_Edit) && 
				IsConnected;
		}

		void Sort()
		{
			SheduleDayIntervals.Sort(item => item.Model.Number);
		}

		public bool AddSave(ScheduleDayInterval scheduleDay)
		{
			Model.DayIntervals.Add(scheduleDay);
			UpdateDaysCount();
			return ScheduleSchemeHelper.Save(Model, false);
		}

		public bool DeleteSave(ScheduleDayInterval scheduleDay)
		{
			Model.DayIntervals.Remove(scheduleDay);
			UpdateDaysCount();
			return ScheduleSchemeHelper.Save(Model, false);
		}

		public bool EditSave(ScheduleDayInterval scheduleDay)
		{
			Model.DayIntervals.RemoveAll(x => x.UID == scheduleDay.UID);
			Model.DayIntervals.Add(scheduleDay);
			UpdateDaysCount();
			return ScheduleSchemeHelper.Save(Model, false);
		}

		void UpdateDaysCount()
		{
			Model.DaysCount = Model.DayIntervals.Count;
			ScheduleSchemeHelper.Save(Model, false);
		}

		public bool IsConnected
		{
			get { return ((SafeFiresecService)ClientManager.FiresecService).IsConnected; }
		}
	}
}