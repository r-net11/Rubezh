using RubezhAPI.GK;
using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GKModule.ViewModels
{
	public class OPCItemViewModel : BaseViewModel
	{
		public OPCItemViewModel(GKBase device)
		{
			Device = device;
			var _device = device as GKDevice;
			if (_device != null)
			{
				Name = _device.Driver.ShortName;
				Address = _device.PresentationAddress;
			}
			else
			{
				Name = device.Name;
				Address = device.No.ToString();
			}
		}

		public GKBase Device { get; private set; }
		public string Name { get; private set; }
		public string Address { get; private set; }
	}
}