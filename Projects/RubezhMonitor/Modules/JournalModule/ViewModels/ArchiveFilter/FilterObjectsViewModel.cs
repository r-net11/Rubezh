using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using RubezhAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using RubezhClient;
using RubezhAPI;
using RubezhClient.SKDHelpers;

namespace JournalModule.ViewModels
{
	public class FilterObjectsViewModel : BaseViewModel
	{
		public FilterObjectsViewModel(JournalFilter filter)
		{
			BuildTree();
			Initialize(filter);
		}

		public void Initialize(JournalFilter filter)
		{
			AllFilters.ForEach(x => x.IsChecked = false);

			foreach (var journalObjectType in filter.JournalObjectTypes)
			{
				var filterNameViewModel = AllFilters.FirstOrDefault(x => x.FilterObjectType == FilterObjectType.ObjectType && x.JournalObjectType == journalObjectType);
				if (filterNameViewModel != null)
				{
					filterNameViewModel.IsChecked = true;
				}
			}
			foreach (var uid in filter.ObjectUIDs)
			{
				if (uid != Guid.Empty)
				{
					var filterNameViewModel = AllFilters.FirstOrDefault(x => x.UID == uid);
					if (filterNameViewModel != null)
					{
						filterNameViewModel.IsChecked = true;
						filterNameViewModel.ExpandToThis();
					}
				}
			}
		}

		public JournalFilter GetModel()
		{
			var filter = new JournalFilter();
			foreach (var filterObject in RootFilters.SelectMany(x => x.GetAllChildren()).Where(x => x.IsChecked))
			{
				switch (filterObject.FilterObjectType)
				{
					case FilterObjectType.ObjectType:
						filter.JournalObjectTypes.Add(filterObject.JournalObjectType);
						break;
					case FilterObjectType.Object:
					case FilterObjectType.Camera:
					case FilterObjectType.HR:
						filter.ObjectUIDs.Add(filterObject.UID);
						break;
					case FilterObjectType.Subsystem:	
					default:
						break;
				}
			}
			return filter;
		}

		public List<FilterObjectViewModel> AllFilters;

		public ObservableCollection<FilterObjectViewModel> RootFilters { get; private set; }

		FilterObjectViewModel _selectedFilter;
		public FilterObjectViewModel SelectedFilter
		{
			get { return _selectedFilter; }
			set
			{
				_selectedFilter = value;
				OnPropertyChanged(() => SelectedFilter);
			}
		}

		void BuildTree()
		{
			RootFilters = new ObservableCollection<FilterObjectViewModel>();
			AllFilters = new List<FilterObjectViewModel>();

			var gkViewModel = new FilterObjectViewModel(JournalSubsystemType.GK);
			gkViewModel.IsExpanded = true;
			RootFilters.Add(gkViewModel);

			var gkDevicesViewModel = new FilterObjectViewModel(JournalObjectType.GKDevice);
			AddChild(gkViewModel, gkDevicesViewModel);
			foreach (var childDevice in GKManager.DeviceConfiguration.RootDevice.Children)
			{
				AddGKDeviceInternal(childDevice, gkDevicesViewModel);
			}

			var gkZonesViewModel = new FilterObjectViewModel(JournalObjectType.GKZone);
			AddChild(gkViewModel, gkZonesViewModel);
			foreach (var zone in GKManager.Zones)
			{
				var filterObjectViewModel = new FilterObjectViewModel(zone);
				AddChild(gkZonesViewModel, filterObjectViewModel);
			}

			var gkDirectionsViewModel = new FilterObjectViewModel(JournalObjectType.GKDirection);
			AddChild(gkViewModel, gkDirectionsViewModel);
			foreach (var direction in GKManager.Directions)
			{
				var filterObjectViewModel = new FilterObjectViewModel(direction);
				AddChild(gkDirectionsViewModel, filterObjectViewModel);
			}

			var gkMPTsViewModel = new FilterObjectViewModel(JournalObjectType.GKMPT);
			AddChild(gkViewModel, gkMPTsViewModel);
			foreach (var mpt in GKManager.MPTs)
			{
				var filterObjectViewModel = new FilterObjectViewModel(mpt);
				AddChild(gkMPTsViewModel, filterObjectViewModel);
			}

			var gkPumpStationsViewModel = new FilterObjectViewModel(JournalObjectType.GKPumpStation);
			AddChild(gkViewModel, gkPumpStationsViewModel);
			foreach (var pumpStation in GKManager.PumpStations)
			{
				var filterObjectViewModel = new FilterObjectViewModel(pumpStation);
				AddChild(gkPumpStationsViewModel, filterObjectViewModel);
			}

			var gkDelaysViewModel = new FilterObjectViewModel(JournalObjectType.GKDelay);
			AddChild(gkViewModel, gkDelaysViewModel);
			foreach (var delay in GKManager.Delays)
			{
				var filterObjectViewModel = new FilterObjectViewModel(delay);
				AddChild(gkDelaysViewModel, filterObjectViewModel);
			}

			var gkGuardZonesViewModel = new FilterObjectViewModel(JournalObjectType.GKGuardZone);
			AddChild(gkViewModel, gkGuardZonesViewModel);
			foreach (var guardZone in GKManager.GuardZones)
			{
				var filterObjectViewModel = new FilterObjectViewModel(guardZone);
				AddChild(gkGuardZonesViewModel, filterObjectViewModel);
			}

			var gkSKDZonesViewModel = new FilterObjectViewModel(JournalObjectType.GKSKDZone);
			AddChild(gkViewModel, gkSKDZonesViewModel);
			foreach (var skdZone in GKManager.SKDZones)
			{
				var filterObjectViewModel = new FilterObjectViewModel(skdZone);
				AddChild(gkSKDZonesViewModel, filterObjectViewModel);
			}

			var gkDoorsViewModel = new FilterObjectViewModel(JournalObjectType.GKDoor);
			AddChild(gkViewModel, gkDoorsViewModel);
			foreach (var door in GKManager.Doors)
			{
				var filterObjectViewModel = new FilterObjectViewModel(door);
				AddChild(gkDoorsViewModel, filterObjectViewModel);
			}

			var gkPIMsViewModel = new FilterObjectViewModel(JournalObjectType.GKPim);
			AddChild(gkViewModel, gkPIMsViewModel);
			foreach (var pim in GKManager.GlobalPims)
			{
				var filterObjectViewModel = new FilterObjectViewModel(pim);
				AddChild(gkPIMsViewModel, filterObjectViewModel);
			}

			var gkUsersViewModel = new FilterObjectViewModel(JournalObjectType.GKUser);
			AddChild(gkViewModel, gkUsersViewModel);
			AllFilters.Add(gkUsersViewModel);

			var videoViewModel = new FilterObjectViewModel(JournalSubsystemType.Video);
			videoViewModel.IsExpanded = true;
			RootFilters.Add(videoViewModel);

			var videoDevicesViewModel = new FilterObjectViewModel(JournalObjectType.Camera);
			AddChild(videoViewModel, videoDevicesViewModel);
			foreach (var camera in ClientManager.SystemConfiguration.Cameras)
			{
				var filterObjectViewModel = new FilterObjectViewModel(camera);
				AddChild(videoDevicesViewModel, filterObjectViewModel);
			}

			var skdViewModel = new FilterObjectViewModel(JournalSubsystemType.SKD);
			skdViewModel.IsExpanded = true;
			RootFilters.Add(skdViewModel);

			var organisationsViewModel = new FilterObjectViewModel(JournalObjectType.Organisation);
			AddChild(skdViewModel, organisationsViewModel);
			var organisations = OrganisationHelper.Get(new OrganisationFilter(), false);
			if (organisations != null)
				foreach (var organisation in organisations)
					AddChild(organisationsViewModel, new FilterObjectViewModel(organisation));

			var employeesViewModel = new FilterObjectViewModel(JournalObjectType.Employee);
			AddChild(skdViewModel, employeesViewModel);
			var employees = EmployeeHelper.Get(new EmployeeFilter(), false);
			if (employees != null)
				foreach (var employee in employees)
					AddChild(employeesViewModel, new FilterObjectViewModel(employee));

			var positionsViewModel = new FilterObjectViewModel(JournalObjectType.Position);
			AddChild(skdViewModel, positionsViewModel);
			var positions = PositionHelper.Get(new PositionFilter(), false);
			if (positions != null)
				foreach (var position in positions)
					AddChild(positionsViewModel, new FilterObjectViewModel(position));

			var departmentsViewModel = new FilterObjectViewModel(JournalObjectType.Department);
			AddChild(skdViewModel, departmentsViewModel);
			var departments = DepartmentHelper.Get(new DepartmentFilter(), false);
			if (departments != null)
				foreach (var department in departments)
					AddChild(departmentsViewModel, new FilterObjectViewModel(department));

			var cardsViewModel = new FilterObjectViewModel(JournalObjectType.Card);
			AddChild(skdViewModel, cardsViewModel);
			var cards = CardHelper.Get(new CardFilter(), false);
			if (cards != null)
				foreach (var card in cards)
					AddChild(cardsViewModel, new FilterObjectViewModel(card));

			var passCardTemplatesViewModel = new FilterObjectViewModel(JournalObjectType.PassCardTemplate);
			AddChild(skdViewModel, passCardTemplatesViewModel);
			var passCardTemplates = PassCardTemplateHelper.Get(new PassCardTemplateFilter(), false);
			if (passCardTemplates != null)
				foreach (var passCardTemplate in passCardTemplates)
					AddChild(passCardTemplatesViewModel, new FilterObjectViewModel(passCardTemplate));

			var accessTemplatesViewModel = new FilterObjectViewModel(JournalObjectType.AccessTemplate);
			AddChild(skdViewModel, accessTemplatesViewModel);
			var accessTemplates = AccessTemplateHelper.Get(new AccessTemplateFilter(), false);
			if (accessTemplates != null)
				foreach (var accessTemplate in accessTemplates)
					AddChild(accessTemplatesViewModel, new FilterObjectViewModel(accessTemplate));

			var additionalColumnsViewModel = new FilterObjectViewModel(JournalObjectType.AdditionalColumn);
			AddChild(skdViewModel, additionalColumnsViewModel);
			var additionalColumns = AdditionalColumnTypeHelper.Get(new AdditionalColumnTypeFilter(), false);
			if (additionalColumns != null)
				foreach (var additionalColumn in additionalColumns)
					AddChild(additionalColumnsViewModel, new FilterObjectViewModel(additionalColumn));

			var dayIntervalsViewModel = new FilterObjectViewModel(JournalObjectType.DayInterval);
			AddChild(skdViewModel, dayIntervalsViewModel);
			var dayIntervals = DayIntervalHelper.Get(new DayIntervalFilter(), false);
			if (dayIntervals != null)
				foreach (var dayInterval in dayIntervals)
					AddChild(dayIntervalsViewModel, new FilterObjectViewModel(dayInterval));

			var scheduleSchemesViewModel = new FilterObjectViewModel(JournalObjectType.ScheduleScheme);
			AddChild(skdViewModel, scheduleSchemesViewModel);
			var scheduleSchemes = ScheduleSchemeHelper.Get(new ScheduleSchemeFilter(), false);
			if (scheduleSchemes != null)
				foreach (var scheduleScheme in scheduleSchemes)
					AddChild(scheduleSchemesViewModel, new FilterObjectViewModel(scheduleScheme));

			var schedulesViewModel = new FilterObjectViewModel(JournalObjectType.Schedule);
			AddChild(skdViewModel, schedulesViewModel);
			var schedules = ScheduleHelper.Get(new ScheduleFilter(), false);
			if (schedules != null)
				foreach (var schedule in schedules)
					AddChild(schedulesViewModel, new FilterObjectViewModel(schedule));

			var holidaysViewModel = new FilterObjectViewModel(JournalObjectType.Holiday);
			AddChild(skdViewModel, holidaysViewModel);
			var holidays = HolidayHelper.Get(new HolidayFilter(), false);
			if (holidays != null)
				foreach (var holiday in holidays)
					AddChild(holidaysViewModel, new FilterObjectViewModel(holiday));

			var noneViewModel = new FilterObjectViewModel(JournalObjectType.None);
			RootFilters.Add(noneViewModel);
			AllFilters.Add(noneViewModel);
		}

		FilterObjectViewModel AddGKDeviceInternal(GKDevice device, FilterObjectViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new FilterObjectViewModel(device);
			if (parentDeviceViewModel != null)
				AddChild(parentDeviceViewModel, deviceViewModel);

			foreach (var childDevice in device.Children)
			{
				AddGKDeviceInternal(childDevice, deviceViewModel);
			}
			return deviceViewModel;
		}

		void AddChild(FilterObjectViewModel parentDeviceViewModel, FilterObjectViewModel childDeviceViewModel)
		{
			parentDeviceViewModel.AddChild(childDeviceViewModel);
			AllFilters.Add(childDeviceViewModel);
		}
	}
}