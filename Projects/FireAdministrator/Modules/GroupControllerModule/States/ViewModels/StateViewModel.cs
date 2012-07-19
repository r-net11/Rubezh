using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;
using XFiresecAPI;
using System.Collections;

namespace GKModule.ViewModels
{
	public class StateViewModel : BaseViewModel
	{
		public StateViewModel(List<byte> bytes)
		{
			var deviceType = BytesHelper.SubstructShort(bytes, 0);
			Driver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverTypeNo == deviceType);
			ControllerAddress = BytesHelper.SubstructShort(bytes, 2);
			AddressOncontroller = BytesHelper.SubstructShort(bytes, 4);
			PhysicalAddress = BytesHelper.SubstructShort(bytes, 6);
			Description = BytesHelper.BytesToStringDescription(bytes.Skip(8).Take(32).ToList());
			SerialNo = BytesHelper.SubstructInt(bytes, 40);
			var state = BytesHelper.SubstructInt(bytes, 44);

			var stateStringBuilder = new StringBuilder();
			var bitArray = new BitArray(new int[1] { state });
			for (int j = 0; j < bitArray.Count; j++)
			{
				var b = bitArray[j];
				if (b)
					stateStringBuilder.Append(j + ", ");
			}
			States = stateStringBuilder.ToString();
		}

		public XDriver Driver { get; private set; }
		public short ControllerAddress { get; private set; }
		public short AddressOncontroller { get; private set; }
		public short PhysicalAddress { get; private set; }
		public string Description { get; private set; }
		public int SerialNo { get; private set; }
		public string States { get; private set; }
	}
}