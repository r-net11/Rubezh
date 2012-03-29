using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XFiresecAPI
{
    public class BinaryFormatterBase
    {
        public List<byte> DeviceType { get; protected set; }
        public List<byte> Address { get; protected set; }
        public List<byte> Offset { get; protected set; }
        public List<byte> OutDependensesCount { get; protected set; }
        public List<byte> OutDependenses { get; protected set; }
        public List<byte> Formula { get; protected set; }
        public List<byte> ParametersCount { get; protected set; }
        public List<byte> Parameters { get; protected set; }
        public List<byte> AllBytes { get; protected set; }
        public List<FormulaOperation> FormulaOperations { get; protected set; }

        public void InitializeAllBytes()
        {
            AllBytes = new List<byte>();
            AllBytes.AddRange(DeviceType);
            AllBytes.AddRange(Address);
            AllBytes.AddRange(Offset);
            AllBytes.AddRange(OutDependensesCount);
            AllBytes.AddRange(OutDependenses);
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