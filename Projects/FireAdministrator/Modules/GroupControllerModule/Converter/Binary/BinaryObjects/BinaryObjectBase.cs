using System;
using System.Collections.Generic;
using System.Linq;
using XFiresecAPI;

namespace GKModule.Converter
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

		public bool IsGk { get; protected set; }
		public bool IsGKObject { get; protected set; }
		public Guid GKObjectNo { get; protected set; }

		public void InitializeAllBytes()
		{
			if (InputDependenses != null)
				InputDependensesCount = ToBytes((short)(InputDependenses.Count() / 2));
			else
				InputDependensesCount = new List<byte>();

			if (OutputDependenses != null)
				OutputDependensesCount = ToBytes((short)(OutputDependenses.Count() / 2));
			else
				OutputDependenses = new List<byte>();

			if (Parameters != null)
			{
				ParametersCount = ToBytes((short)(Parameters.Count() / 4));
				if (IsGk)
				{
					//Offset = null;
				}
				else
				{
					Offset = ToBytes((short)(8 + InputDependenses.Count() + Formula.Count()));
				}
			}
			else
			{
				ParametersCount = new List<byte>();
			}

			AllBytes = new List<byte>();
			AllBytes.AddRange(DeviceType);
			AllBytes.AddRange(Address);
			AllBytes.AddRange(Offset);
			AllBytes.AddRange(InputDependensesCount);
			AllBytes.AddRange(InputDependenses);
			if (IsGk)
			{
				AllBytes.AddRange(OutputDependensesCount);
				AllBytes.AddRange(OutputDependenses);
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