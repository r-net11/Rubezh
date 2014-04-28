using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class CardZonesViewModel : BaseViewModel
	{
		public List<CardZone> CardZones { get; private set; }
		HashSet<SKDZone> AllParentZones;

		public CardZonesViewModel(List<CardZone> cardZones)
		{
			Update(cardZones);
		}

		public void Update(List<CardZone> cardZones)
		{
			CardZones = cardZones;
			InitializeAllParentZones();
			AllZones = new List<AccessZoneViewModel>();
			RootZone = AddZoneInternal(SKDManager.SKDConfiguration.RootZone, null);
			OnPropertyChanged("RootZones");

			foreach (var zone in AllZones)
			{
				if (zone.IsChecked)
					zone.ExpandToThis();
			}
			SelectedZone = AllZones.FirstOrDefault(x => x.IsChecked);
		}

		void InitializeAllParentZones()
		{
			AllParentZones = new HashSet<SKDZone>();
			foreach (var cardZone in CardZones)
			{
				var zone = SKDManager.Zones.FirstOrDefault(x => x.UID == cardZone.ZoneUID);
				if (zone != null)
				{
					AddAllParents(zone);
				}
			}
		}
		void AddAllParents(SKDZone zone)
		{
			AllParentZones.Add(zone);
			if (zone.Parent != null)
				AddAllParents(zone.Parent);
		}

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
			var zoneViewModel = new AccessZoneViewModel(zone, CardZones, x => { SelectedZone = x; });
			AllZones.Add(zoneViewModel);
			if (parentZoneViewModel != null)
				parentZoneViewModel.AddChild(zoneViewModel);

			foreach (var childZone in zone.Children)
			{
				if (AllParentZones.Any(x => x.UID == childZone.UID))
					AddZoneInternal(childZone, zoneViewModel);
			}
			return zoneViewModel;
		}
	}
}