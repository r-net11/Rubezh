using RubezhAPI.GK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GKWebService.Models.Door
{
	public class DoorDevice
	{
		public DoorDevice(GKDevice device)
		{
				UID = device.UID;
				Name = device.PresentationName;
				ImageSource = device.ImageSource.Replace("/Controls;component/", "");
		}

		public Guid UID { get; set; }

		public string Name { get; set; }

		public string ImageSource { get; set; }
	}
}