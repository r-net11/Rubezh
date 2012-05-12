using System;
using System.Collections.Generic;
using System.Linq;
using XFiresecAPI;

namespace GKModule.Database
{
	public class BinaryObjectBase
	{
		public List<byte> DeviceType { get; protected set; }
		public List<byte> Address { get; protected set; }
		public List<byte> Offset { get; protected set; }
		public List<byte> InputDependensesCount { get; private set; }
		public List<byte> InputDependenses { get; protected set; }
		public List<byte> OutputDependensesCount { get; private set; }
		public List<byte> OutputDependenses { get; protected set; }
		public List<byte> Formula { get; protected set; }
		public List<byte> ParametersCount { get; private set; }
		public List<byte> Parameters { get; protected set; }
		public List<byte> AllBytes { get; private set; }
		public List<FormulaOperation> FormulaOperations { get; protected set; }

		public DatabaseType DatabaseType { get; protected set; }
		public XZone Zone { get; protected set; }
		public XDevice Device { get; protected set; }
		XBinaryBase GetBinaryBase()
		{
			XBinaryBase binaryBase = null;
			if (Zone != null)
				binaryBase = Zone;
			if (Device != null)
				binaryBase = Device;
			return binaryBase;
		}

		protected void SetAddress(short address)
		{
			Address = new List<byte>();

			if (DatabaseType == DatabaseType.Gk)
			{
				var binaryBase = GetBinaryBase();

				short controllerAddress = 0;
				if (binaryBase.KauDatabaseParent != null)
				{
					int lineNo = 0;
					var modeProperty = binaryBase.KauDatabaseParent.Properties.FirstOrDefault(x=>x.Name == "Mode");
					if (modeProperty != null)
					{
						var propertyParameter = binaryBase.KauDatabaseParent.Driver.Properties.FirstOrDefault(x => x.Name == "Mode");
						lineNo = (int)propertyParameter.Parameters.FirstOrDefault(x => x.Name == modeProperty.Value).Value;
					}

					byte intAddress = binaryBase.KauDatabaseParent.IntAddress;
					controllerAddress = (short)(lineNo * 256 + intAddress);
				}
				else
				{
					controllerAddress = 0x200;
				}
				Address.AddRange(ToBytes(controllerAddress));

				var no = binaryBase.GetDatabaseNo(DatabaseType);
				Address.AddRange(ToBytes(no));
			}
			Address.AddRange(ToBytes(address));
		}

		void InitializeInputOutputDependences()
		{
			var binaryBase = GetBinaryBase();

			InputDependenses = new List<byte>();
			OutputDependenses = new List<byte>();

			foreach (var device in binaryBase.InputDevices)
			{
				var no = device.GetDatabaseNo(DatabaseType);
				InputDependenses.AddRange(BitConverter.GetBytes(no));
			}
			foreach (var device in binaryBase.OutputDevices)
			{
				var no = device.GetDatabaseNo(DatabaseType);
				OutputDependenses.AddRange(BitConverter.GetBytes(no));
			}
			foreach (var zone in binaryBase.InputZones)
			{
				var no = zone.GetDatabaseNo(DatabaseType);
				InputDependenses.AddRange(BitConverter.GetBytes(no));
			}
		}

		public void InitializeAllBytes()
		{
			InitializeInputOutputDependences();

			InputDependensesCount = ToBytes((short)(InputDependenses.Count() / 2));
			OutputDependensesCount = ToBytes((short)(OutputDependenses.Count() / 2));
			ParametersCount = ToBytes((short)(Parameters.Count() / 4));

			Offset = ToBytes((short)(8 + InputDependenses.Count() + Formula.Count()));

			AllBytes = new List<byte>();
			AllBytes.AddRange(DeviceType);
			AllBytes.AddRange(Address);
			AllBytes.AddRange(Offset);
			AllBytes.AddRange(OutputDependensesCount);
			AllBytes.AddRange(OutputDependenses);
			if (DatabaseType == DatabaseType.Gk)
			{
				AllBytes.AddRange(InputDependensesCount);
				AllBytes.AddRange(InputDependenses);
			}
			AllBytes.AddRange(Formula);
			AllBytes.AddRange(ParametersCount);
			AllBytes.AddRange(Parameters);
		}

		public List<byte> ToBytes(short shortValue)
		{
			return BitConverter.GetBytes(shortValue).ToList();
		}
	}
}