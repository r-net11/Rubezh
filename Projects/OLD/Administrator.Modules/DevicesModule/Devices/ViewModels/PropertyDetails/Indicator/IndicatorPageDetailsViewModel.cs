using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class IndicatorPageDetailsViewModel : SaveCancelDialogViewModel
	{
		Device Device;
		List<Guid> Zones;

		public IndicatorPageDetailsViewModel(Device device)
		{
			Title = "Свойства страницы индикаторов. Автоматическая привязка к зонам";
			ShowZonesCommand = new RelayCommand(OnShowZones);
			Device = device;
			InitializeZones();
		}

		void InitializeZones()
		{
			Zones = new List<Guid>();
			var flag = false;
			foreach (var child in Device.Children)
			{
				if (child.IndicatorLogic.ZoneUIDs.Count == 1)
					Zones.Add(child.IndicatorLogic.ZoneUIDs[0]);
				if (child.IndicatorLogic.ZoneUIDs.Count != 1 && !flag)
				{
					flag = true;
					continue;
				}
				if (flag && (child.IndicatorLogic.DeviceUID != Guid.Empty || child.IndicatorLogic.ZoneUIDs.Count != 0))
				{
					Zones = new List<Guid>();
					return;
				}
			}
		}

		public RelayCommand ShowZonesCommand { get; private set; }
		void OnShowZones()
		{
			var zonesSelectionViewModel = new ZonesSelectionViewModel(Device, Zones);
			if (DialogService.ShowModalWindow(zonesSelectionViewModel))
			{
				Zones = zonesSelectionViewModel.Zones;
				Zones = Zones.Take(50).ToList();
				OnPropertyChanged(() => PresenrationZones);
			}
		}

		public string PresenrationZones
		{
			get
			{
				string presenrationZones = "";
				for (int i = 0; i < Zones.Count; i++)
				{
					var zone = FiresecManager.Zones.FirstOrDefault(x => x.UID == Zones[i]);
					if (i > 0)
						presenrationZones += ", ";
					presenrationZones += zone.PresentationName;
				}
				return presenrationZones;
			}
		}

		protected override bool Save()
		{
			try
			{
				for (int i = 0; i < Device.Children.Count; i++)
				{
					var device = Device.Children[i];
					if (i < Zones.Count)
					{
						var zoneUID = Zones[i];
						var zone = FiresecManager.Zones.FirstOrDefault(x => x.UID == zoneUID);
						device.IndicatorLogic = new IndicatorLogic()
						{
							IndicatorLogicType = IndicatorLogicType.Zone,
							ZoneUIDs = new List<Guid>() { zoneUID },
							Zones = new List<Zone>() { zone }
						};
					}
					else
					{
						device.IndicatorLogic = new IndicatorLogic();
					}
				}
				return true;
			}
			catch (Exception e)
			{
				Logger.Error(e, "IndicatorPageDetailsViewModel.Save");
				return false;
			}
		}
	}
}