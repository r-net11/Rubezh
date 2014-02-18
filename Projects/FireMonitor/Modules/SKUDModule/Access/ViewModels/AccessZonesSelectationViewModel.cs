using System.Collections.Generic;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Linq;

namespace SKDModule.ViewModels
{
	public class AccessZonesSelectationViewModel : BaseViewModel
	{
		public List<CardZone> CardZones { get; private set; }

		public AccessZonesSelectationViewModel(List<CardZone> cardZones)
		{
			CardZones = cardZones;
			AllZones = new List<AccessZoneViewModel>();
			RootZone = AddZoneInternal(SKDManager.SKDConfiguration.RootZone, null);
			SelectedZone = RootZone;

			foreach (var zone in AllZones)
			{
				zone.ExpandToThis();
			}
		}

		#region Zones
		public List<AccessZoneViewModel> AllZones;

		AccessZoneViewModel _selectedZone;
		public AccessZoneViewModel SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				if (value != null)
					value.ExpandToThis();
				OnPropertyChanged("SelectedZone");
			}
		}

		AccessZoneViewModel _rootZone;
		public AccessZoneViewModel RootZone
		{
			get { return _rootZone; }
			private set
			{
				_rootZone = value;
				OnPropertyChanged("RootZone");
			}
		}

		public AccessZoneViewModel[] RootZones
		{
			get { return new AccessZoneViewModel[] { RootZone }; }
		}

		AccessZoneViewModel AddZoneInternal(SKDZone zone, AccessZoneViewModel parentZoneViewModel)
		{
			var zoneViewModel = new AccessZoneViewModel(zone, CardZones);
			AllZones.Add(zoneViewModel);
			if (parentZoneViewModel != null)
				parentZoneViewModel.AddChild(zoneViewModel);

			foreach (var childZone in zone.Children)
			{
				AddZoneInternal(childZone, zoneViewModel);
			}
			return zoneViewModel;
		}
		#endregion

		public List<CardZone> GetCardZones()
		{
			CardZones = new List<CardZone>();
			foreach (var zone in AllZones)
			{
				if (zone.IsChecked)
				{
					var cardZone = new CardZone()
					{
						ZoneUID = zone.Zone.UID,
						IsAntiPassback = zone.IsAntiPassback,
						IsComission = zone.IsComission,
						IntervalType = zone.SelectedTimeCreteria.IntervalType,
						IntervalUID = zone.SelectedTimeType.UID
					};
					CardZones.Add(cardZone);
				}
			}
			return CardZones;
		}
	}
}