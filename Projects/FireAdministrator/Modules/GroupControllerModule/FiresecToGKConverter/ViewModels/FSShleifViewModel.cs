using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Models;
using XFiresecAPI;
using FiresecClient;

namespace GKModule.ViewModels
{
    public class FSShleifViewModel : BaseViewModel
    {
		public FSShleifViewModel()
		{
			KAUDevices = new List<XDevice>();
			foreach (var device in XManager.DeviceConfiguration.Devices)
			{
				if (device.Driver.DriverType == XDriverType.KAU)
					KAUDevices.Add(device);
			}

			KAUShleifNos = new List<int>();
			KAUShleifNos.Add(1);
			KAUShleifNos.Add(2);
			KAUShleifNos.Add(3);
			KAUShleifNos.Add(4);
			KAUShleifNos.Add(5);
			KAUShleifNos.Add(6);
			KAUShleifNos.Add(7);
			KAUShleifNos.Add(8);
		}

		public Device FSDevice { get; set; }
        public int FSShleifNo { get; set; }
		public XDevice KAUDevice { get; set; }
        public int KAUShleifNo { get; set; }

		public List<XDevice> KAUDevices { get; private set; }
		public List<int> KAUShleifNos { get; private set; }
    }
}