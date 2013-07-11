using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.ViewModels;
using System.Collections.ObjectModel;
using FiresecClient;
using Infrastructure;

namespace DevicesModule.ViewModels
{
	public class SimulationViewModel : MenuViewPartViewModel
	{
		public SimulationViewModel()
		{
			Menu = new SimulationMenuViewModel(this);
		}

		public void Initialize()
		{
			Zones = new ObservableCollection<SimulationZoneViewModel>();
			foreach (var zone in FiresecManager.Zones)
			{
				var simulationZoneViewModel = new SimulationZoneViewModel(zone);
				Zones.Add(simulationZoneViewModel);
			}
			SelectedZone = Zones.FirstOrDefault();
		}

		ObservableCollection<SimulationZoneViewModel> _zones;
		public ObservableCollection<SimulationZoneViewModel> Zones
		{
			get { return _zones; }
			set
			{
				_zones = value;
				OnPropertyChanged("Zones");
			}
		}

		SimulationZoneViewModel _selectedZone;
		public SimulationZoneViewModel SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				if (value != null)
					value.Initialize();
				OnPropertyChanged("SelectedZone");
			}
		}

		int FSChangesCount;
		public override void OnShow()
		{
			base.OnShow();
			if (ServiceFactory.SaveService.FSChangesCount > FSChangesCount)
			{
				FSChangesCount = ServiceFactory.SaveService.FSChangesCount;
				Initialize();
			}
		}

		public override void OnHide()
		{
			base.OnHide();
		}
	}
}