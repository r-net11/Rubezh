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
			AdditionalStateProperties = new List<AdditionalXStateProperty>();
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
					case XDriverType.RSR2_KAU:
						var bitArray = new BitArray(new int[1] { additionalShortParameters[0] });
						if (bitArray[0])
							AdditionalStates.Add("Неисправность питания 1");
						if (bitArray[1])
							AdditionalStates.Add("Неисправность питания 2");
						if (bitArray[2])
							AdditionalStates.Add("Отказ АЛС 1 или 2");
						if (bitArray[3])
							AdditionalStates.Add("Отказ АЛС 3 или 4");
						if (bitArray[4])
							AdditionalStates.Add("Отказ АЛС 5 или 6");
						if (bitArray[5])
							AdditionalStates.Add("Отказ АЛС 7 или 8");
						if (bitArray[6])
							AdditionalStates.Add("Вскрытие");
						if (bitArray[8])
							AdditionalStates.Add("Короткое замыкание АЛС 1");
						if (bitArray[9])
							AdditionalStates.Add("Короткое замыкание АЛС 2");
						if (bitArray[10])
							AdditionalStates.Add("Короткое замыкание АЛС 3");
						if (bitArray[11])
							AdditionalStates.Add("Короткое замыкание АЛС 4");
						if (bitArray[12])
							AdditionalStates.Add("Короткое замыкание АЛС 5");
						if (bitArray[13])
							AdditionalStates.Add("Короткое замыкание АЛС 6");
						if (bitArray[14])
							AdditionalStates.Add("Короткое замыкание АЛС 7");
						if (bitArray[15])
							AdditionalStates.Add("Короткое замыкание АЛС 8");
						break;

					case XDriverType.GK:
						bitArray = new BitArray(new int[1] { additionalShortParameters[0] });
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

					case XDriverType.RSR2_MVK8:
						OnDelay = additionalShortParameters[0];
						HoldDelay = additionalShortParameters[1];
						OffDelay = additionalShortParameters[2];
						break;
				}
			}
			else
			{
				if (TypeNo == 0x106)
				{
					var property1 = new AdditionalXStateProperty()
					{
						Name = "Задержка",
						Value = additionalShortParameters[0]
					};
					AdditionalStateProperties.Add(property1);
					var property2 = new AdditionalXStateProperty()
					{
						Name = "Удержание",
						Value = additionalShortParameters[1]
					};
					AdditionalStateProperties.Add(property2);
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
		public List<AdditionalXStateProperty> AdditionalStateProperties { get; private set; }

		public int OnDelay { get; private set; }
		public int HoldDelay { get; private set; }
		public int OffDelay { get; private set; }
	}
}