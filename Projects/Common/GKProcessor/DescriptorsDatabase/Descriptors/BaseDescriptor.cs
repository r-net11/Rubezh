using System;
using System.Collections.Generic;
using FiresecAPI.GK;
using FiresecClient;

namespace GKProcessor
{
	public abstract class BaseDescriptor
	{
		public DatabaseType DatabaseType { get; set; }
		public DescriptorType DescriptorType { get; protected set; }
		public GKBase GKBase { get; private set; }

		public ushort ControllerAdress { get; protected set; }
		public ushort AdressOnController { get; protected set; }
		public ushort PhysicalAdress { get; protected set; }

		public List<byte> DeviceType { get; protected set; }
		public List<byte> Address { get; protected set; }
		public List<byte> Description { get; protected set; }
		public List<byte> Offset { get; protected set; }
		public List<byte> InputDependensesCount { get; private set; }
		public List<byte> InputDependenses { get; protected set; }
		public List<byte> OutputDependensesCount { get; private set; }
		public List<byte> OutputDependenses { get; protected set; }
		public List<byte> FormulaBytes { get; set; }
		public List<byte> ParametersCount { get; private set; }
		public List<byte> Parameters { get; protected set; }
		public List<byte> AllBytes { get; private set; }
		public FormulaBuilder Formula { get; set; }

		public BaseDescriptor(GKBase gkBase, DatabaseType databaseType)
		{
			GKBase = gkBase;
			DatabaseType = databaseType;
			Formula = new FormulaBuilder();
			Parameters = new List<byte>();
		}

		protected void SetAddress(ushort address)
		{
			PhysicalAdress = address;
			Address = new List<byte>();

			switch (DatabaseType)
			{
				case DatabaseType.Gk:
					if (GKBase.KauDatabaseParent != null)
					{
						ushort lineNo = GKManager.GetKauLine(GKBase.KauDatabaseParent);
						ControllerAdress = (ushort)(lineNo * 256 + GKBase.KauDatabaseParent.IntAddress);
						AdressOnController = GKBase.KAUDescriptorNo;
					}
					else
					{
						ControllerAdress = 0x200;
						AdressOnController = GKBase.GKDescriptorNo;
					}
					Address.AddRange(BytesHelper.ShortToBytes(ControllerAdress));
					Address.AddRange(BytesHelper.ShortToBytes(AdressOnController));
					Address.AddRange(BytesHelper.ShortToBytes(PhysicalAdress));
					break;

				case DatabaseType.Kau:
					Address.AddRange(BytesHelper.ShortToBytes(PhysicalAdress));
					break;
			}
		}

		void InitializeInputOutputDependences()
		{
			InputDependenses = new List<byte>();
			OutputDependenses = new List<byte>();

			if (DatabaseType == DatabaseType.Gk)
			{
				if (GKBase.InputGKBases != null)
				foreach (var inputGKBase in GKBase.InputGKBases)
				{
					var no = inputGKBase.GKDescriptorNo;
					InputDependenses.AddRange(BitConverter.GetBytes(no));
				}
				if (GKBase.OutputGKBases != null)
				foreach (var outputGKBase in GKBase.OutputGKBases)
				{
					var no = outputGKBase.GKDescriptorNo;
					OutputDependenses.AddRange(BitConverter.GetBytes(no));
				}
			}

			if (DatabaseType == DatabaseType.Kau)
			{
				foreach (var inputGKBase in GKBase.InputGKBases)
				{
					if (inputGKBase.KauDatabaseParent != GKBase.KauDatabaseParent)
						continue;
					var no = inputGKBase.GKDescriptorNo;
					InputDependenses.AddRange(BitConverter.GetBytes(no));
				}
				foreach (var outputGKBase in GKBase.OutputGKBases)
				{
					//if ((outputGKBase is GKGuardZone) && (outputGKBase as GKGuardZone).GuardZoneEnterMethod != GKGuardZoneEnterMethod.GlobalOnly)
					//	return;
					if (outputGKBase.KauDatabaseParent != GKBase.KauDatabaseParent)
						continue;
					var no = outputGKBase.KAUDescriptorNo;
					OutputDependenses.AddRange(BitConverter.GetBytes(no));
				}
			}
		}

		public void InitializeDescription()
		{
			switch (DatabaseType)
			{
				case DatabaseType.Gk:
					Description = BytesHelper.StringDescriptionToBytes(GKBase.GetGKDescription(GKManager.DeviceConfiguration.GKNameGenerationType));
					break;

				case DatabaseType.Kau:
					Description = new List<byte>();
					break;
			}
		}

		public void InitializeOffset()
		{
			int offsetToParameters = 0;
			switch (DatabaseType)
			{
				case DatabaseType.Gk:
					offsetToParameters = 2 + 6 + 32 + 2 + 2 + InputDependenses.Count + 2 + OutputDependenses.Count + FormulaBytes.Count;
					break;

				case DatabaseType.Kau:
					offsetToParameters = 2 + 2 + 2 + 2 + OutputDependenses.Count + FormulaBytes.Count;
					break;
			}
			Offset = BytesHelper.ShortToBytes((ushort)offsetToParameters);
		}

		public void InitializeAllBytes()
		{
			InitializeDescription();
			InitializeInputOutputDependences();

			InputDependensesCount = BytesHelper.ShortToBytes((ushort)(InputDependenses.Count / 2));
			OutputDependensesCount = BytesHelper.ShortToBytes((ushort)(OutputDependenses.Count / 2));
			ParametersCount = BytesHelper.ShortToBytes((ushort)(Parameters.Count / 4));

			InitializeOffset();

			AllBytes = new List<byte>();
			AllBytes.AddRange(DeviceType);
			AllBytes.AddRange(Address);
			AllBytes.AddRange(Description);
			AllBytes.AddRange(Offset);
			if (DatabaseType == DatabaseType.Gk)
			{
				AllBytes.AddRange(InputDependensesCount);
				AllBytes.AddRange(InputDependenses);
			}
			AllBytes.AddRange(OutputDependensesCount);
			AllBytes.AddRange(OutputDependenses);
			AllBytes.AddRange(FormulaBytes);
			AllBytes.AddRange(ParametersCount);
			AllBytes.AddRange(Parameters);
		}

		//public GKBase GKBase
		//{
		//	get
		//	{
		//		switch (DescriptorType)
		//		{
		//			case DescriptorType.Device:
		//				return Device;

		//			case DescriptorType.Zone:
		//				return Zone;

		//			case DescriptorType.Direction:
		//				return Direction;

		//			case DescriptorType.PumpStation:
		//				return PumpStation;

		//			case DescriptorType.MPT:
		//				return MPT;

		//			case DescriptorType.Delay:
		//				return Delay;

		//			case DescriptorType.Pim:
		//				return Pim;

		//			case DescriptorType.GuardZone:
		//				return GuardZone;

		//			case DescriptorType.Code:
		//				return Code;

		//			case DescriptorType.Door:
		//				return Door;
		//		}
		//		return null;
		//	}
		//}

		public ushort GetDescriptorNo()
		{
			switch (DatabaseType)
			{
				case DatabaseType.Gk:
					return GKBase.GKDescriptorNo;

				case DatabaseType.Kau:
					return GKBase.KAUDescriptorNo;
			}
			return 0;
		}

		public abstract void Build();
	}
}