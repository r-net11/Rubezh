using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GroupControllerModule.Models;

namespace GroupControllerModule.Converter
{
    public static class BinConverter
    {
        public static void Convert()
        {
            foreach (var device in XManager.DeviceConfiguration.Devices)
            {
                short type = 0x100;
                short address = 0x01;
                short initializationTableOffset = 0;
                List<byte> objectOutDependencesBytes = GetObjectOutDependencesBytes();
                short outDependensesCount = (short)(objectOutDependencesBytes.Count() / 2);
                List<byte> formulaBytes = GetFormulaBytes();
                List<byte> propertiesBytes = GetPropertiesBytes();
                short propertiesBytesCount = (short)(propertiesBytes.Count() / 4);

                var deviceBytes = new List<byte>();
                deviceBytes.AddRange(BitConverter.GetBytes(type));
                deviceBytes.AddRange(BitConverter.GetBytes(address));
                deviceBytes.AddRange(BitConverter.GetBytes(initializationTableOffset));
                deviceBytes.AddRange(BitConverter.GetBytes(outDependensesCount));
                deviceBytes.AddRange(objectOutDependencesBytes);
                deviceBytes.AddRange(formulaBytes);
                deviceBytes.AddRange(BitConverter.GetBytes(propertiesBytesCount));
                deviceBytes.AddRange(propertiesBytes);
            }
        }

        static List<byte> GetObjectOutDependencesBytes()
        {
            var bytes = new List<byte>();
            for (int i = 0; i < 10; i++)
            {
                short objectNo = (short)i;
                bytes.AddRange(BitConverter.GetBytes(objectNo));
            }
            return bytes;
        }

        static List<byte> GetFormulaBytes()
        {
            var bytes = new List<byte>();

            var formulaOperationType = FormulaOperationType.END;
            byte operationType = (byte)formulaOperationType;

            byte firstOperand = 0;
            short secondOperand = 0;

            bytes.Add(operationType);
            bytes.Add(firstOperand);
            bytes.AddRange(BitConverter.GetBytes(secondOperand));

            return bytes;
        }

        static List<byte> GetPropertiesBytes()
        {
            var bytes = new List<byte>();

            byte parameterNo = 0;
            short parameterValue = 0;

            bytes.Add(parameterNo);
            bytes.AddRange(BitConverter.GetBytes(parameterValue));
            bytes.Add(0);

            return bytes;
        }
    }
}