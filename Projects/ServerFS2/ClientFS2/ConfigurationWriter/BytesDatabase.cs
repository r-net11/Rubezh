using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientFS2.ConfigurationWriter
{
	public class BytesDatabase
	{
		public BytesDatabase()
		{
			ByteDescriptions = new List<ByteDescription>();
		}

		public List<ByteDescription> ByteDescriptions { get; set; }

		public void AddShort(short value, string description = null)
		{
			var bytes = BytesHelper.ShortToBytes(value);
			AddBytes(bytes, description);
		}

		public void AddByte(byte value, string description = null)
		{
			AddBytes(new List<byte>() { value }, description);
		}

		public void AddString(string value, string description = null)
		{
			var bytes = BytesHelper.StringToBytes(value);
			AddBytes(bytes, description);
		}

		public void AddReference(ByteDescription byteDescription, string description = null)
		{
			var byteDescriptions = AddBytes(new List<byte>() { 0, 0, 0 }, description);
			byteDescriptions.AddressReference = byteDescription;
		}

		public ByteDescription AddBytes(List<byte> bytes, string description = null)
		{
			var byteDescriptions = new List<ByteDescription>();
			foreach (var b in bytes)
			{
				var byteDescription = new ByteDescription()
				{
					Value = b
				};
				byteDescriptions.Add(byteDescription);
			}
			byteDescriptions[0].Description = description;
			ByteDescriptions.AddRange(byteDescriptions);
			return byteDescriptions[0];
		}
	}
}