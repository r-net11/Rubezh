using System.Collections.Generic;
using System.Linq;
using XFiresecAPI;
using FiresecClient;

namespace Common.GK
{
	public class BinaryObjectState
	{
		public BinaryObjectState(List<byte> bytes)
		{
			ControllerAddress = BytesHelper.SubstructShort(bytes, 2);
			AddressOncontroller = BytesHelper.SubstructShort(bytes, 4);
			PhysicalAddress = BytesHelper.SubstructShort(bytes, 6);
			Description = BytesHelper.BytesToStringDescription(bytes.Skip(8).Take(32).ToList());
			SerialNo = BytesHelper.SubstructInt(bytes, 40);
			int state = BytesHelper.SubstructInt(bytes, 44);

			var deviceType = BytesHelper.SubstructShort(bytes, 0);
			Driver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverTypeNo == deviceType);

			States = StatesHelper.StatesFromInt(state);
		}

		public XDriver Driver { get; private set; }
		public ushort ControllerAddress { get; private set; }
		public ushort AddressOncontroller { get; private set; }
		public ushort PhysicalAddress { get; private set; }
		public string Description { get; private set; }
		public int SerialNo { get; private set; }
		public List<XStateType> States { get; private set; }
	}
}