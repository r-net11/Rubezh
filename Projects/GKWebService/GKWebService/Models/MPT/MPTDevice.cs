using RubezhAPI.GK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GKWebService.Models
{
	public class MPTDevice
	{
		public Guid Uid { get; set; }
				
		public string  MPTDeviceType { get; set; }

		public string DottedPresentationAddress { get; set; }

		public string Description { get; set; }

	}
}