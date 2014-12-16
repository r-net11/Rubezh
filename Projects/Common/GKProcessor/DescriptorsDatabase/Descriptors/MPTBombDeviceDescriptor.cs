using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;

namespace GKProcessor
{
	public class MPTBombDeviceDescriptor : DeviceDescriptor
	{
		GKMPT ParentMPT { get; set; }

		public MPTBombDeviceDescriptor(GKDevice device, DatabaseType databaseType) : base(device, databaseType)
		{
		}

		protected override void CreateFormula()
		{
			Formula = new FormulaBuilder();
			Formula.AddGetBit(GKStateBit.On, ParentMPT, DatabaseType);
			Formula.AddGetBit(GKStateBit.Norm, Device, DatabaseType);
			Formula.Add(FormulaOperationType.AND, comment: "Смешивание с битом Дежурный устройства");
			Formula.AddPutBit(GKStateBit.TurnOn_InAutomatic, Device, DatabaseType);

			Formula.AddGetBit(GKStateBit.Off, ParentMPT, DatabaseType);
			Formula.AddGetBit(GKStateBit.Norm, Device, DatabaseType);
			Formula.Add(FormulaOperationType.AND);
			Formula.AddPutBit(GKStateBit.TurnOff_InAutomatic, Device, DatabaseType);

			Formula.Add(FormulaOperationType.END);
			FormulaBytes = Formula.GetBytes();
		}
	}
}