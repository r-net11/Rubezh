using System.Collections.Generic;
using System.Linq;
using FiresecAPI.XModels;
using FiresecClient;
using XFiresecAPI;
using System.Collections;

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

			TypeNo = BytesHelper.SubstructShort(bytes, 0);
			Driver = XManager.DriversConfiguration.XDrivers.FirstOrDefault(x => x.DriverTypeNo == TypeNo);

			States = XStatesHelper.StatesFromInt(state);
			SetAdditionalParameters(bytes);
		}

		void SetAdditionalParameters(List<byte> bytes)
		{
			AdditionalStates = new List<string>();
			var additionalShortParameters = new List<ushort>();
			for (int i = 0; i < 10; i++)
			{
				var additionalShortParameter = BytesHelper.SubstructShort(bytes, 48 + i * 2);
				additionalShortParameters.Add(additionalShortParameter);
			}

			if (Driver != null)
			{
				switch (Driver.DriverType)
				{
					case XDriverType.KAU:
						AdditionalStates.Add("Питание 1: " + additionalShortParameters[2].ToString());
						AdditionalStates.Add("Питание 2: " + additionalShortParameters[3].ToString());
						break;

					case XDriverType.GK:
						var bitArray = new BitArray(new int[1] { additionalShortParameters[0] });
						if (bitArray[0])
							AdditionalStates.Add("Неисправность питания 1");
						if (bitArray[1])
							AdditionalStates.Add("Неисправность питания 2");
						if (bitArray[2])
							AdditionalStates.Add("Неисправность ОЛС");
						if (bitArray[3])
							AdditionalStates.Add("Неисправность РЛС");
						if (bitArray[6])
							AdditionalStates.Add("Вскрытие");
						break;
				}
			}
			else
			{
				if (TypeNo == 0x106)
				{
					AdditionalStates.Add("Задержка: " + additionalShortParameters[0].ToString());
					AdditionalStates.Add("Удержание: " + additionalShortParameters[1].ToString());
				}
			}
		}

		public XDriver Driver { get; private set; }
		public ushort ControllerAddress { get; private set; }
		public ushort AddressOncontroller { get; private set; }
		public ushort PhysicalAddress { get; private set; }
		public string Description { get; private set; }
		public int SerialNo { get; private set; }
		public ushort TypeNo { get; private set; }
		public List<XStateType> States { get; private set; }
		public List<string> AdditionalStates { get; private set; }
	}
}