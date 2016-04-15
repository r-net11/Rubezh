using System;
using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using GKModule.Events;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Elements;
using RubezhAPI;

namespace GKModule.Plans.ViewModels
{
	public class SKDZonePropertiesViewModel : SaveCancelDialogViewModel
	{
		IElementZone IElementZone;
		SKDZonesViewModel _skdZonesViewModel;

		public SKDZonePropertiesViewModel(IElementZone iElementZone, SKDZonesViewModel skdZonesViewModel)
		{
			_skdZonesViewModel = skdZonesViewModel;
			IElementZone = iElementZone;
			CreateCommand = new RelayCommand(OnCreate);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
			Title = "Свойства фигуры: СКД Зона";
			var zones = GKManager.SKDZones;
			Zones = new ObservableCollection<SKDZoneViewModel>();
			foreach (var zone in zones)
			{
				var zoneViewModel = new SKDZoneViewModel(zone);
				Zones.Add(zoneViewModel);
			}
			if (iElementZone.ZoneUID != Guid.Empty)
				SelectedZone = Zones.FirstOrDefault(x => x.Zone.UID == iElementZone.ZoneUID);
		}

		public ObservableCollection<SKDZoneViewModel> Zones { get; private set; }

		SKDZoneViewModel _selectedZone;
		public SKDZoneViewModel SelectedZone
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
			var createSKDZoneEventArg = new CreateGKSKDZoneEventArg();
			ServiceFactory.Events.GetEvent<CreateGKSKDZoneEvent>().Publish(createSKDZoneEventArg);
			if (createSKDZoneEventArg.Zone != null)
			{
				GKPlanExtension.Instance.Cache.BuildSafe<GKSKDZone>();
				GKPlanExtension.Instance.SetItem<GKSKDZone>(IElementZone, createSKDZoneEventArg.Zone.UID);
			}
			UpdateZones(zoneUID);
			if (!createSKDZoneEventArg.Cancel)
			    Close(true);
		}

		public RelayCommand EditCommand { get; private set; }
		private void OnEdit()
		{
			ServiceFactory.Events.GetEvent<EditGKSKDZoneEvent>().Publish(SelectedZone.Zone.UID);
			SelectedZone.Update();
		}
		private bool CanEdit()
		{
			return SelectedZone != null;
		}

		protected override bool Save()
		{
			Guid zoneUID = IElementZone.ZoneUID;
			GKPlanExtension.Instance.SetItem<GKSKDZone>(IElementZone, SelectedZone == null ? null : SelectedZone.Zone);
			UpdateZones(zoneUID);
			return base.Save();
		}
		private void UpdateZones(Guid zoneUID)
		{
			if (_skdZonesViewModel != null)
			{
				if (zoneUID != IElementZone.ZoneUID)
					Update(zoneUID);
				Update(IElementZone.ZoneUID);
				_skdZonesViewModel.LockedSelect(IElementZone.ZoneUID);
			}
		}
		private void Update(Guid zoneUID)
		{
			var zone = _skdZonesViewModel.Zones.FirstOrDefault(x => x.Zone.UID == zoneUID);
			if (zone != null)
				zone.Update();
		}
	}
}