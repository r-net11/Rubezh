using System;
using System.Collections.Generic;
using FiresecClient;
using XFiresecAPI;
using System.Text;

namespace Common.GK
{
	public abstract class BinaryObjectBase
	{
		public DatabaseType DatabaseType { get; set; }
		public XZone Zone { get; protected set; }
		public XDevice Device { get; protected set; }
		public XDirection Direction { get; protected set; }
		public ObjectType ObjectType { get; protected set; }
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
		public List<byte> FormulaBytes { get; protected set; }
		public List<byte> ParametersCount { get; private set; }
		public List<byte> Parameters { get; protected set; }
		public List<byte> AllBytes { get; private set; }
		public FormulaBuilder Formula { get; protected set; }

		public BinaryObjectBase()
		{
			Formula = new FormulaBuilder();
		}

		protected void SetAddress(ushort address)
		{
			PhysicalAdress = address;
			Address = new List<byte>();

			switch (DatabaseType)
			{
				case DatabaseType.Gk:
					var binaryBase = BinaryBase;

					if (binaryBase.KauDatabaseParent != null)
					{
						ushort lineNo = XManager.GetKauLine(binaryBase.KauDatabaseParent);
						byte intAddress = binaryBase.KauDatabaseParent.IntAddress;
						ControllerAdress = (ushort)(lineNo * 256 + intAddress);
						AdressOnController = binaryBase.GetDatabaseNo(DatabaseType.Kau);
					}
					else
					{
						ControllerAdress = 0x200;
						AdressOnController = binaryBase.GetDatabaseNo(DatabaseType.Gk);
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
			var binaryBase = BinaryBase;

			InputDependenses = new List<byte>();
			OutputDependenses = new List<byte>();

			foreach (var inputBinaryBase in binaryBase.InputObjects)
			{
				var no = inputBinaryBase.GetDatabaseNo(DatabaseType);
				InputDependenses.AddRange(BitConverter.GetBytes(no));
			}
			foreach (var outputBinaryBase in binaryBase.OutputObjects)
			{
				var no = outputBinaryBase.GetDatabaseNo(DatabaseType);
				OutputDependenses.AddRange(BitConverter.GetBytes(no));
			}
		}

		public void InitializeDescription()
		{
			switch (DatabaseType)
			{
				case DatabaseType.Gk:
					Description = BytesHelper.StringDescriptionToBytes(BinaryBase.GetBinaryDescription());
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

		public XBinaryBase BinaryBase
		{
			get
			{
				switch (ObjectType)
				{
					case ObjectType.Device:
						return Device;

					case ObjectType.Zone:
						return Zone;

					case ObjectType.Direction:
						return Direction;
				}
				return null;
			}
		}

		public ushort GetNo()
		{
			return BinaryBase.GetDatabaseNo(DatabaseType);
		}

		public ushort KauDescriptorNo
		{
			get { return BinaryBase.GetDatabaseNo(DatabaseType.Kau); }
		}
		public ushort GkDescriptorNo
		{
			get { return BinaryBase.GetDatabaseNo(DatabaseType.Gk); }
		}

		public string StringFomula
		{
			get { return Formula.GetStringFomula(); }
		}

		public abstract void Build();
	}
}