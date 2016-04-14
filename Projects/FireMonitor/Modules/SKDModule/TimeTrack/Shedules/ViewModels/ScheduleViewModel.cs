using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using RubezhAPI;
using RubezhAPI.SKD;
using RubezhClient;
using RubezhClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class ScheduleViewModel : OrganisationElementViewModel<ScheduleViewModel, Schedule>, IDoorsParent
	{
		private bool _isInitialized;
		
		public ScheduleViewModel() { }
		
		public override void InitializeOrganisation(Organisation organisation, ViewPartViewModel parentViewModel)
		{
			base.InitializeOrganisation(organisation, parentViewModel);
			_isInitialized = false;
		}
		
		public override void InitializeModel(Organisation organisation, Schedule model, ViewPartViewModel parentViewModel)
		{
			EditCommand = new RelayCommand(OnEdit, CanEdit);

			base.InitializeModel(organisation, model, parentViewModel);
			_isInitialized = false;
			Update();
		}

		public void Initialize()
		{
			if (!_isInitialized)
			{
				_isInitialized = true;
				if (!IsOrganisation)
				{
					ScheduleZones = new SortableObservableCollection<ScheduleZoneViewModel>();
					foreach (var employeeScheduleZone in Model.Zones)
					{
						var scheduleZoneViewModel = new ScheduleZoneViewModel(employeeScheduleZone);
						ScheduleZones.Add(scheduleZoneViewModel);
					}
					ScheduleZones.Sort(x => x.No);
					SelectedScheduleZone = ScheduleZones.FirstOrDefault();
				}
			}
		}
		public override void Update()
		{
			base.Update();
			OnPropertyChanged(() => Scheme);
		}

		public string Scheme
		{
			get
			{
				if (!IsOrganisation)
				{
					var schemes = ScheduleSchemeHelper.Get(new ScheduleSchemeFilter
					{
						OrganisationUIDs = new List<Guid>() { Organisation.UID },
						Type = ScheduleSchemeType.Month | ScheduleSchemeType.SlideDay | ScheduleSchemeType.Week,
						WithDays = false,
					}, showError: false);
					if (schemes != null)
					{
						var scheme = schemes.FirstOrDefault(item => item.UID == Model.ScheduleSchemeUID);
						if (scheme != null)
						{
							return scheme.Name + " (" + scheme.Type.ToDescription() + ")";
						}
					}
				}
				return null;
			}
		}

		public SortableObservableCollection<ScheduleZoneViewModel> ScheduleZones { get; private set; }

		private ScheduleZoneViewModel _selectedScheduleZone;
		public ScheduleZoneViewModel SelectedScheduleZone
		{
			get { return _selectedScheduleZone; }
			set
			{
				_selectedScheduleZone = value;
				OnPropertyChanged(() => SelectedScheduleZone);
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var scheduleZoneDetailsViewModel = new ScheduleZoneDetailsViewModel(Model, Organisation);
			if (DialogService.ShowModalWindow(scheduleZoneDetailsViewModel) && EditSave(scheduleZoneDetailsViewModel.ScheduleZone))
			{
				ScheduleZones.Clear();
				foreach (var scheduleZone in scheduleZoneDetailsViewModel.ScheduleZone)
				{
					ScheduleZones.Add(new ScheduleZoneViewModel(scheduleZone));
				}
				ScheduleZones.Sort(x => x.No);
				SelectedScheduleZone = ScheduleZones.FirstOrDefault();
			}
		}
		bool CanEdit()
		{
			return !IsDeleted && ClientManager.CheckPermission(RubezhAPI.Models.PermissionType.Oper_SKD_TimeTrack_Schedules_Edit);
		}

		public void UpdateCardDoors(IEnumerable<Guid> doorUIDs)
		{
			if (ScheduleZones == null)
				return;
			var zonesToRemove = ScheduleZones.Where(x => !doorUIDs.Any(y => y == x.Model.DoorUID)).ToList();
			if (DeleteManySave(zonesToRemove.Select(x => x.Model)))
			{
				foreach (var item in zonesToRemove)
				{
					ScheduleZones.Remove(item);
				}
			}
		}

		public bool EditSave(List<ScheduleZone> zone)
		{
			Model.Zones = new List<ScheduleZone>(zone);
			return ScheduleHelper.Save(Model, false);
		}

		public bool DeleteManySave(IEnumerable<ScheduleZone> zones)
		{
			Model.Zones.RemoveAll(x => zones.Any(y => y.UID == x.UID));
			return ScheduleHelper.Save(Model, false);
		}
	}
}