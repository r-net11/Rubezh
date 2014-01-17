using System;
using System.Collections.Generic;
using FiresecClient;
using XFiresecAPI;

namespace GKProcessor
{
	public abstract class BaseDescriptor
	{
		public DatabaseType DatabaseType { get; set; }
		public DescriptorType DescriptorType { get; protected set; }
		public XZone Zone { get; protected set; }
		public XDevice Device { get; protected set; }
		public XDirection Direction { get; protected set; }
		public XPumpStation PumpStation { get; protected set; }
		public XDelay Delay { get; protected set; }
		public XPim Pim { get; protected set; }
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

		public BaseDescriptor()
		{
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
					if (XBase.KauDatabaseParent != null)
					{
						ushort lineNo = XManager.GetKauLine(XBase.KauDatabaseParent);
						ControllerAdress = (ushort)(lineNo * 256 + XBase.KauDatabaseParent.IntAddress);
						AdressOnController = XBase.KAUDescriptorNo;
					}
					else
					{
						ControllerAdress = 0x200;
						AdressOnController = XBase.GKDescriptorNo;
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
				foreach (var inputXBase in XBase.InputXBases)
				{
					var no = inputXBase.GKDescriptorNo;
					InputDependenses.AddRange(BitConverter.GetBytes(no));
				}
				foreach (var outputXBase in XBase.OutputXBases)
				{
					var no = outputXBase.GKDescriptorNo;
					OutputDependenses.AddRange(BitConverter.GetBytes(no));
				}
			}
		}

		public void InitializeDescription()
		{
			switch (DatabaseType)
			{
				case DatabaseType.Gk:
					Description = BytesHelper.StringDescriptionToBytes(XBase.PresentationName);
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

		public XBase XBase
		{
			get
			{
				switch (DescriptorType)
				{
					case DescriptorType.Device:
						return Device;

					case DescriptorType.Zone:
						return Zone;

					case DescriptorType.Direction:
						return Direction;

					case DescriptorType.PumpStation:
						return PumpStation;

					case DescriptorType.Delay:
						return Delay;

					case DescriptorType.Pim:
						return Pim;
				}
				return null;
			}
		}

		public ushort GetDescriptorNo()
		{
			switch (DatabaseType)
			{
				case DatabaseType.Gk:
					return XBase.GKDescriptorNo;

				case DatabaseType.Kau:
					return XBase.KAUDescriptorNo;
			}
			return 0;
		}

		public abstract void Build();
	}
}