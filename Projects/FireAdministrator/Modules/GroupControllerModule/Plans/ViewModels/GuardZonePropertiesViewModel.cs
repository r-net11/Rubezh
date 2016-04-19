using GKModule.Events;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Plans.Elements;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace GKModule.Plans.ViewModels
{
	public class GuardZonePropertiesViewModel : SaveCancelDialogViewModel
	{
		IElementZone IElementZone;
		GuardZonesViewModel _zonesViewModel;

		public GuardZonePropertiesViewModel(IElementZone iElementZone, GuardZonesViewModel zonesViewModel)
		{
			_zonesViewModel = zonesViewModel;
			IElementZone = iElementZone;
			CreateCommand = new RelayCommand(OnCreate);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			Title = "Свойства фигуры: Охранная зона";
			Zones = new ObservableCollection<GuardZoneViewModel>();
			foreach (var zone in GKManager.DeviceConfiguration.GuardZones)
			{
				var zoneViewModel = new GuardZoneViewModel(zone);
				Zones.Add(zoneViewModel);
			}
			if (iElementZone.ZoneUID != Guid.Empty)
				SelectedZone = Zones.FirstOrDefault(x => x.Zone.UID == iElementZone.ZoneUID);
		}

		public ObservableCollection<GuardZoneViewModel> Zones { get; private set; }

		GuardZoneViewModel _selectedZone;
		public GuardZoneViewModel SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				OnPropertyChanged(() => SelectedZone);
			}
		}

		public RelayCommand CreateCommand { get; private set; }
		private void OnCreate()
		{
			Guid zoneUID = IElementZone.ZoneUID;
			var createZoneEventArg = new CreateGKGuardZoneEventArg();
			ServiceFactory.Events.GetEvent<CreateGKGuardZoneEvent>().Publish(createZoneEventArg);
			if (createZoneEventArg.Zone != null)
			{
				GKPlanExtension.Instance.Cache.BuildSafe<GKGuardZone>();
				GKPlanExtension.Instance.SetItem<GKGuardZone>(IElementZone, createZoneEventArg.Zone.UID);
			}
			UpdateZones(zoneUID);
			if (!createZoneEventArg.Cancel)
				Close(true);
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			ServiceFactory.Events.GetEvent<EditGKGuardZoneEvent>().Publish(SelectedZone.Zone.UID);
			SelectedZone.Update();
		}
		private bool CanEdit()
		{
			return SelectedZone != null;
		}

		protected override bool Save()
		{
			Guid zoneUID = IElementZone.ZoneUID;
			GKPlanExtension.Instance.SetItem<GKGuardZone>(IElementZone, SelectedZone == null ? null : SelectedZone.Zone);
			UpdateZones(zoneUID);
			return base.Save();
		}
		private void UpdateZones(Guid xguardZoneUID)
		{
			if (_zonesViewModel != null)
			{
				if (xguardZoneUID != IElementZone.ZoneUID)
					Update(xguardZoneUID);
				Update(IElementZone.ZoneUID);
				_zonesViewModel.LockedSelect(IElementZone.ZoneUID);
			}
		}
		private void Update(Guid zoneUID)
		{
			var zone = _zonesViewModel.Zones.FirstOrDefault(x => x.Zone.UID == zoneUID);
			if (zone != null)
				zone.Update();
		}
	}
}