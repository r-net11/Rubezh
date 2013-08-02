using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace VideoModule.ViewModels
{
	public class CameraViewModel : BaseViewModel
	{
		public Camera Camera { get; set; }

		public CameraViewModel(Camera camera)
		{
			Camera = camera;
		}

		public string PresenrationZones
		{
			get
			{
				var zones = new List<Zone>();
				foreach (var zoneUID in Camera.ZoneUIDs)
				{
					var zone = FiresecManager.Zones.FirstOrDefault(x => x.UID == zoneUID);
					if (zone != null)
						zones.Add(zone);
				}
				var presentationZones = FiresecManager.FiresecConfiguration.DeviceConfiguration.GetCommaSeparatedZones(zones);
				return presentationZones;
			}
		}

		public void Update()
		{
			OnPropertyChanged("Camera");
			OnPropertyChanged("PresenrationZones");
		}
	}
}