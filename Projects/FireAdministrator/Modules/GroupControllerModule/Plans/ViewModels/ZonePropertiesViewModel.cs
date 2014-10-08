using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using GKModule.Events;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Elements;

namespace GKModule.Plans.ViewModels
{
	public class ZonePropertiesViewModel : SaveCancelDialogViewModel
	{
		IElementZone IElementZone;
		ZonesViewModel _zonesViewModel;

		public ZonePropertiesViewModel(IElementZone iElementZone, ZonesViewModel zonesViewModel)
		{
			_zonesViewModel = zonesViewModel;
			IElementZone = iElementZone;
			CreateCommand = new RelayCommand(OnCreate);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			Title = "Свойства фигуры: Зона";
			var zones = GKManager.DeviceConfiguration.SortedZones;
			Zones = new ObservableCollection<ZoneViewModel>();
			foreach (var zone in zones)
			{
				var zoneViewModel = new ZoneViewModel(zone);
				Zones.Add(zoneViewModel);
			}
			if (iElementZone.ZoneUID != Guid.Empty)
				SelectedZone = Zones.FirstOrDefault(x => x.Zone.UID == iElementZone.ZoneUID);
		}

		public ObservableCollection<ZoneViewModel> Zones { get; private set; }

		ZoneViewModel _selectedZone;
		public ZoneViewModel SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				OnPropertyChanged("SelectedZone");
			}
		}

		public RelayCommand CreateCommand { get; private set; }
		private void OnCreate()
		{
			Guid zoneUID = IElementZone.ZoneUID;
			var createZoneEventArg = new CreateGKZoneEventArg();
			ServiceFactory.Events.GetEvent<CreateGKZoneEvent>().Publish(createZoneEventArg);
			if (createZoneEventArg.Zone != null)
			{
				GKPlanExtension.Instance.Cache.BuildSafe<GKZone>();
				GKPlanExtension.Instance.SetItem<GKZone>(IElementZone, createZoneEventArg.Zone.UID);
			}
			UpdateZones(zoneUID);
			if (!createZoneEventArg.Cancel)
				Close(true);
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			ServiceFactory.Events.GetEvent<EditGKZoneEvent>().Publish(SelectedZone.Zone.UID);
			SelectedZone.Update(SelectedZone.Zone);
		}
		private bool CanEdit()
		{
			return SelectedZone != null;
		}

		protected override bool Save()
		{
			Guid zoneUID = IElementZone.ZoneUID;
			GKPlanExtension.Instance.SetItem<GKZone>(IElementZone, SelectedZone == null ? null : SelectedZone.Zone);
			UpdateZones(zoneUID);
			return base.Save();
		}
		private void UpdateZones(Guid xzoneUID)
		{
			if (_zonesViewModel != null)
			{
				if (xzoneUID != IElementZone.ZoneUID)
					Update(xzoneUID);
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