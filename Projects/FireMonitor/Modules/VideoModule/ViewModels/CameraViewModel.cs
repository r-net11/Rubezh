using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace VideoModule.ViewModels
{
	public class CameraViewModel : BaseViewModel
	{
		public Camera Camera { get; set; }

		public CameraViewModel(Camera camera)
		{
			Camera = camera;
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
		}

		public string PresenrationZones
		{
			get
			{
				var zones = new List<XZone>();
				foreach (var zoneUID in Camera.ZoneUIDs)
				{
					var zone = XManager.Zones.FirstOrDefault(x => x.UID == zoneUID);
					if (zone != null)
						zones.Add(zone);
				}
				var presentationZones = XManager.GetCommaSeparatedZones(zones);
				return presentationZones;
			}
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
		}
	}
}