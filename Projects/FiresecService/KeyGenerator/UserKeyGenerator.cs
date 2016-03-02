using System;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace KeyGenerator
{
	internal sealed class UserKeyGenerator
	{
		private static string RunQuery(string tableName, string methodName)
		{
			var mos = new ManagementObjectSearcher("Select * from Win32_" + tableName);
			foreach (ManagementObject mo in mos.Get())
			{
				var result = mo[methodName];
				return result == null ? string.Empty : mo[methodName].ToString();
			}

			return string.Empty;
		}

		private static string GetSerialKey()
		{
			var resultKey = string.Empty;
			resultKey += RunQuery("BaseBoard", "Product");
			resultKey += RunQuery("BIOS", "Version");
			resultKey += RunQuery("OperatingSystem", "SerialNumber");
			resultKey += RunQuery("Processor", "ProcessorId");

			return resultKey;
		}

		public string GenerateUID()
		{
			//Combine the IDs and get bytes
			var id = GetSerialKey();
			var byteIds = Encoding.UTF8.GetBytes(id);

			//Use SHA512 to get the fixed length checksum of the ID string
			var sha512 = new SHA512CryptoServiceProvider();
			var checksum = sha512.ComputeHash(byteIds);

			//Convert checksum into 4 ulong parts and use BASE36 to encode both
			var part1Id = BASE36.Encode(BitConverter.ToUInt32(checksum, 0));
			var part2Id = BASE36.Encode(BitConverter.ToUInt32(checksum, 4));
			var part3Id = BASE36.Encode(BitConverter.ToUInt32(checksum, 8));
			var part4Id = BASE36.Encode(BitConverter.ToUInt32(checksum, 12));

			return string.Format("{0}-{1}-{2}-{3}", part1Id, part2Id, part3Id, part4Id);
		}
	}
}
