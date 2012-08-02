using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.GK;
using FiresecClient;
using XFiresecAPI;

namespace Common.GK
{
	public class BinaryDeviceState
	{
		public BinaryDeviceState(List<byte> bytes, DatabaseType databaseType)
		{
			int state = 0;
			switch (databaseType)
			{
				case DatabaseType.Gk:
					ControllerAddress = BytesHelper.SubstructShort(bytes, 2);
					AddressOncontroller = BytesHelper.SubstructShort(bytes, 4);
					PhysicalAddress = BytesHelper.SubstructShort(bytes, 6);
					Description = BytesHelper.BytesToStringDescription(bytes.Skip(8).Take(32).ToList());
					SerialNo = BytesHelper.SubstructInt(bytes, 40);
					state = BytesHelper.SubstructInt(bytes, 44);
					break;

				case DatabaseType.Kau:
					AddressOncontroller = BytesHelper.SubstructShort(bytes, 2);
					SerialNo = BytesHelper.SubstructInt(bytes, 4);
					state = BytesHelper.SubstructInt(bytes, 8);
					break;
			}
			var deviceType = BytesHelper.SubstructShort(bytes, 0);
			Driver = XManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverTypeNo == deviceType);

			var stateStringBuilder = new StringBuilder();
			var bitArray = new BitArray(new int[1] { state });
			for (int j = 0; j < bitArray.Count; j++)
			{
				var b = bitArray[j];
				if (b)
					stateStringBuilder.Append(j + ", ");
			}
			StringStates = stateStringBuilder.ToString();

			States = new List<XStateType>();
			for (int bitIndex = 0; bitIndex < bitArray.Count; bitIndex++)
			{
				var b = bitArray[bitIndex];
				if (b)
				{
					var stateTupe = (XStateType)bitIndex;
					States.Add(stateTupe);
				}
			}
		}

		public XDriver Driver { get; private set; }
		public short ControllerAddress { get; private set; }
		public short AddressOncontroller { get; private set; }
		public short PhysicalAddress { get; private set; }
		public string Description { get; private set; }
		public int SerialNo { get; private set; }
		public string StringStates { get; private set; }
		public List<XStateType> States { get; private set; }
	}
}