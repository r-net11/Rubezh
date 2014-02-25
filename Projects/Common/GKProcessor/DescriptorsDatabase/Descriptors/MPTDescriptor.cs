using System;
using System.Collections.Generic;
using Common;
using XFiresecAPI;

namespace GKProcessor
{
	public class MPTDescriptor : BaseDescriptor
	{
		public MPTDescriptor(GkDatabase gkDatabase, XMPT mpt)
		{
			DatabaseType = DatabaseType.Gk;
			DescriptorType = DescriptorType.MPT;
			MPT = mpt;
			Build();
		}

		public override void Build()
		{
			DeviceType = BytesHelper.ShortToBytes((ushort)0x106);
			SetAddress((ushort)0);
			SetFormulaBytes();
			SetPropertiesBytes();
		}

        void SetFormulaBytes()
        {
            Formula = new FormulaBuilder();

			if (MPT.StartLogic.Clauses.Count > 0)
				Formula.AddClauseFormula(MPT.StartLogic.Clauses);

			var handStartDevices = new List<XDevice>();
			foreach (var mptDevice in MPT.MPTDevices)
			{
				if (mptDevice.MPTDeviceType == MPTDeviceType.HandStart)
					handStartDevices.Add(mptDevice.Device);
			}

			if (handStartDevices.Count > 0)
			{
				foreach (var handStartDevice in handStartDevices)
				{
					Formula.AddGetBit(XStateBit.On, handStartDevice);
					Formula.Add(FormulaOperationType.OR);
				}
			}

			Formula.AddGetBit(XStateBit.Norm, MPT);
			Formula.Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный НС");
			Formula.AddPutBit(XStateBit.TurnOn_InAutomatic, MPT);

            Formula.Add(FormulaOperationType.END);
            FormulaBytes = Formula.GetBytes();
        }

		void SetPropertiesBytes()
		{
			var binProperties = new List<BinProperty>();
			binProperties.Add(new BinProperty()
			{
				No = 0,
				Value = (ushort)MPT.Delay
			});
			binProperties.Add(new BinProperty()
			{
				No = 1,
				Value = (ushort)MPT.Hold
			});
			binProperties.Add(new BinProperty()
			{
				No = 2,
				Value = (ushort)MPT.DelayRegime
			});

			foreach (var binProperty in binProperties)
			{
				Parameters.Add(binProperty.No);
				Parameters.AddRange(BitConverter.GetBytes(binProperty.Value));
				Parameters.Add(0);
			}
		}
	}
}