using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Elements;

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
			Title = "Свойства фигуры: Зона";
			var zones = GKManager.SKDZones;
			Zones = new ObservableCollection<SKDZoneViewModel>();
			foreach (var zone in zones)
			{
				var zoneViewModel = new SKDZoneViewModel(zone);
				Zones.Add(zoneViewModel);
			}
		}

		public ObservableCollection<SKDZoneViewModel> Zones { get; private set; }

		protected override bool Save()
		{
			Guid zoneUID = IElementZone.ZoneUID;
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