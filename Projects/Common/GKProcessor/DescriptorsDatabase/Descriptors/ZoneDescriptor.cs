using FiresecAPI.GK;

namespace GKProcessor
{
	public class ZoneDescriptor : BaseDescriptor
	{
		GKZone Zone { get; set; }

		public ZoneDescriptor(GKZone zone)
			: base(zone)
		{
			DescriptorType = DescriptorType.Zone;
			Zone = zone;
		}

		public override void Build()
		{
			DeviceType = BytesHelper.ShortToBytes((ushort)0x100);
			SetAddress((ushort)0);
		}

		public override void BuildFormula()
		{
			Formula = new FormulaBuilder();
			if ((DatabaseType == DatabaseType.Gk && Zone.IsLogicOnKau) || (DatabaseType == DatabaseType.Kau && !Zone.IsLogicOnKau))
			{
				Formula.Add(FormulaOperationType.END);
				return;
			}

			var fire1Count = AddDeviceFire1();
			AddDeviceFire2();
			Formula.Add(FormulaOperationType.CONST, 0, (ushort)Zone.Fire2Count, comment:"Количество устройств для формирования Пожар2");
			Formula.Add(FormulaOperationType.MUL);
			Formula.Add(FormulaOperationType.ADD);
			Formula.Add(FormulaOperationType.DUP);
			Formula.Add(FormulaOperationType.CONST, 0, (ushort)Zone.Fire2Count, comment: "Количество устройств для формирования Пожар2");
			Formula.Add(FormulaOperationType.GE);
			Formula.AddGetBit(GKStateBit.Fire2, Zone);
			Formula.Add(FormulaOperationType.OR);
			Formula.AddPutBit(GKStateBit.Fire2, Zone);

			Formula.Add(FormulaOperationType.DUP);
			Formula.Add(FormulaOperationType.CONST, 0, (ushort)Zone.Fire1Count, comment: "Количество устройств для формирования Пожар1");
			Formula.Add(FormulaOperationType.GE);
			Formula.AddGetBit(GKStateBit.Fire1, Zone);
			Formula.Add(FormulaOperationType.OR);
			Formula.AddPutBit(GKStateBit.Fire1, Zone);

			Formula.Add(FormulaOperationType.CONST, 0, 1, comment: "Количество устройств для формирования Внимание");
			Formula.Add(FormulaOperationType.GE);
			Formula.AddPutBit(GKStateBit.Attention, Zone);

			Formula.Add(FormulaOperationType.END);
		}

		int AddDeviceFire1()
		{
			var count = 0;
			foreach (var device in Zone.Devices)
			{
				if (device.Driver.AvailableStateBits.Contains(GKStateBit.Fire1))
				{
					Formula.AddGetBitOff(GKStateBit.Fire1, device);

					if (count > 0)
					{
						Formula.Add(FormulaOperationType.ADD);
					}
					count++;
				}
			}
			if (count == 0)
				Formula.Add(FormulaOperationType.CONST, 0, 0, comment: "Нулевое количество устройств в состоянии Пожар1");

			return count;
		}
		int AddDeviceFire2()
		{
			var count = 0;
			foreach (var device in Zone.Devices)
			{
				if (device.Driver.AvailableStateBits.Contains(GKStateBit.Fire2))
				{
					Formula.AddGetBitOff(GKStateBit.Fire2, device);

					if (count > 0)
					{
						Formula.Add(FormulaOperationType.OR);
					}
					count++;
				}
			}
			if (count == 0)
				Formula.Add(FormulaOperationType.CONST, 0, 0, comment: "Количество устройств в состоянии Пожар2");

			return count;
		}
	}
}