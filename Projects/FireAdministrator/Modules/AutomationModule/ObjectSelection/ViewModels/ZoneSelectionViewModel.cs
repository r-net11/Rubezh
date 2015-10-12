﻿using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using RubezhClient;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ZoneSelectionViewModel : SaveCancelDialogViewModel
	{
		public ZoneSelectionViewModel(GKZone zone)
		{
			Title = "Выбор зоны";
			Zones = new ObservableCollection<ZoneViewModel>();
			GKManager.Zones.ForEach(x => Zones.Add(new ZoneViewModel(x)));
			if (zone != null)
				SelectedZone = Zones.FirstOrDefault(x => x.Zone.UID == zone.UID);
			if (SelectedZone == null)
				SelectedZone = Zones.FirstOrDefault();
		}

		public ObservableCollection<ZoneViewModel> Zones { get; private set; }

		ZoneViewModel _selectedZone;
		public ZoneViewModel SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				OnPropertyChanged(() => SelectedZone);
			}
		}
	}
}