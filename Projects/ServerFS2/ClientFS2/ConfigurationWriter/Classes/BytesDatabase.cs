using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace ClientFS2.ConfigurationWriter
{
	public class BytesDatabase
	{
		public BytesDatabase(string name = null)
		{
			Name = name;
			ByteDescriptions = new List<ByteDescription>();
		}

		public string Name { get; set; }
		public List<ByteDescription> ByteDescriptions { get; set; }
		public bool IsBold { get; set; }
		static bool LastIsBold;

		public ByteDescription AddShort(short value, string description = null)
		{
			var bytes = BytesHelper.ShortToBytes(value);
			return AddBytes(bytes, description);
		}

		public void SetShort(ByteDescription byteDescription, short value)
		{
			var bytes = BytesHelper.ShortToBytes(value);
			for (int i = 0; i < bytes.Count; i++)
			{
				ByteDescriptions[byteDescription.ByteIndex + i].Value = bytes[i];
			}
		}

		public ByteDescription AddByte(byte value, string description = null)
		{
			return AddBytes(new List<byte>() { value }, description);
		}

		public void AddString(string value, string description = null, int lenght = 20)
		{
			var bytes = BytesHelper.StringToBytes(value, lenght);
			AddBytes(bytes, description);
		}

		public void AddReference(ByteDescription byteDescription, string description = null)
		{
			var byteDescriptions = AddBytes(new List<byte>() { 0, 0, 0 }, description);
			byteDescriptions.AddressReference = byteDescription;
		}

		public void AddReference(BytesDatabase bytesDatabase, string description = null)
		{
			var byteDescriptions = AddBytes(new List<byte>() { 0, 0, 0 }, description);
			if (bytesDatabase != null)
				byteDescriptions.AddressReference = bytesDatabase.ByteDescriptions.FirstOrDefault();
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
			byteDescriptions[0].ByteIndex = ByteDescriptions.Count;
			ByteDescriptions.AddRange(byteDescriptions);
			return byteDescriptions[0];
		}

		public ByteDescription AddReferenceToTable(TableBase tableBase, string description = null)
		{
			var byteDescriptions = AddBytes(new List<byte>() { 0, 0, 0 }, description);
			byteDescriptions.TableBaseReference = tableBase;
			return byteDescriptions;
		}

		public void Add(BytesDatabase bytesDatabase)
		{
			LastIsBold = !LastIsBold;
			bytesDatabase.IsBold = LastIsBold;

			foreach (var byteDescription in bytesDatabase.ByteDescriptions)
			{
				byteDescription.GroupName = bytesDatabase.Name;
				byteDescription.IsBold = bytesDatabase.IsBold;
			}
			foreach (var byteDescription in bytesDatabase.ByteDescriptions)
			{
				ByteDescriptions.Add(byteDescription);
			}
		}

		public void Order()
		{
			for (int i = 0; i < ByteDescriptions.Count; i++)
			{
				ByteDescriptions[i].Offset = i;
			}
		}

		public void ResolveTableReferences()
		{
			foreach (var byteDescription in ByteDescriptions)
			{
				if (byteDescription.TableBaseReference != null)
				{
					byteDescription.AddressReference = ByteDescriptions.FirstOrDefault(x => x.TableHeader != null && x.TableHeader.UID == byteDescription.TableBaseReference.UID);
				}
			}
		}

		public void ResolverReferences()
		{
			for (int i = 0; i < ByteDescriptions.Count; i++)
			{
				var byteDescription = ByteDescriptions[i];
				if (byteDescription.AddressReference != null)
				{
					var index = byteDescription.AddressReference.Offset;
					var bit3 = index >> 16;
					var bit2 = (index >> 8) % 256;
					var bit1 = index % 256;
					ByteDescriptions[i + 0].Value = bit3;
					ByteDescriptions[i + 1].Value = bit2;
					ByteDescriptions[i + 2].Value = bit1;

					//var offsetBytes = BitConverter.GetBytes(devicesGroup.Offset);
					//bytes1[offset + 0] = offsetBytes[1];
					//bytes1[offset + 1] = offsetBytes[2];
					//bytes1[offset + 2] = offsetBytes[3];
				}
			}
		}
	}
}