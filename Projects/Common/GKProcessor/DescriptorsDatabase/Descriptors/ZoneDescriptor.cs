using System.Collections.Generic;
using FiresecAPI.Models;
using XFiresecAPI;

namespace GKProcessor
{
	public class ZoneDescriptor : BaseDescriptor
	{
		public ZoneDescriptor(XZone zone)
		{
			DatabaseType = DatabaseType.Gk;
			DescriptorType = DescriptorType.Zone;
			Zone = zone;
			Build();
		}

		public override void Build()
		{
			DeviceType = BytesHelper.ShortToBytes((ushort)0x100);
			SetAddress((ushort)0);
			SetFormulaBytes();
		}

		void SetFormulaBytes()
		{
			Formula = new FormulaBuilder();
			var fire1Count = AddDeviceFire1();
			AddDeviceFire2();

			Formula.Add(FormulaOperationType.CONST, 0, (ushort)Zone.Fire2Count, "Количество устройств для формирования Пожар2");
			Formula.Add(FormulaOperationType.MUL);
			Formula.Add(FormulaOperationType.ADD);
			Formula.Add(FormulaOperationType.DUP);
			Formula.Add(FormulaOperationType.CONST, 0, (ushort)Zone.Fire2Count, "Количество устройств для формирования Пожар2");
			Formula.Add(FormulaOperationType.GE);
			Formula.AddGetBit(XStateBit.Fire2, Zone);
			Formula.Add(FormulaOperationType.OR);
			Formula.AddPutBit(XStateBit.Fire2, Zone);

			Formula.Add(FormulaOperationType.DUP);
			Formula.Add(FormulaOperationType.CONST, 0, (ushort)Zone.Fire1Count, "Количество устройств для формирования Пожар1");
			Formula.Add(FormulaOperationType.GE);
			Formula.AddGetBit(XStateBit.Fire1, Zone);
			Formula.Add(FormulaOperationType.OR);
			Formula.AddPutBit(XStateBit.Fire1, Zone);

			Formula.Add(FormulaOperationType.CONST, 0, 1, "Количество устройств для формирования Внимание");
			Formula.Add(FormulaOperationType.GE);
			Formula.AddPutBit(XStateBit.Attention, Zone);

			Formula.Add(FormulaOperationType.END);
			FormulaBytes = Formula.GetBytes();
		}

		int AddDeviceFire1()
		{
			var count = 0;
			foreach (var device in Zone.Devices)
			{
				if (device.Driver.AvailableStateBits.Contains(XStateBit.Fire1))
				{
					Formula.AddGetBitOff(XStateBit.Fire1, device);

					if (count > 0)
					{
						Formula.Add(FormulaOperationType.ADD);
					}
					count++;
				}
			}
			if (count == 0)
				Formula.Add(FormulaOperationType.CONST, 0, 0, "Количество устройств в состоянии Пожар1");

			return count;
		}
		int AddDeviceFire2()
		{
			var count = 0;
			foreach (var device in Zone.Devices)
			{
				if (device.Driver.AvailableStateBits.Contains(XStateBit.Fire2))
				{
					Formula.AddGetBitOff(XStateBit.Fire2, device);

					if (count > 0)
					{
						Formula.Add(FormulaOperationType.OR);
					}
					count++;
				}
			}
			if (count == 0)
				Formula.Add(FormulaOperationType.CONST, 0, 0, "Количество устройств в состоянии Пожар2");

			return count;
		}
	}
}