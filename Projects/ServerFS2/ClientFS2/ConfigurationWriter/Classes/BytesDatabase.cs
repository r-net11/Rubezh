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

		public ByteDescription AddShort(int value, string description = null, bool isReadOnly = false, bool ignoreUnequal = false)
		{
			var bytes = BytesHelper.ShortToBytes((short)value);
			var byteDescription = AddBytes(bytes, description, isReadOnly, ignoreUnequal);
			byteDescription.RealValue = value.ToString();
			return byteDescription;
		}

		public ByteDescription AddInt(int value, string description = null, bool isReadOnly = false, bool ignoreUnequal = false)
		{
			var bytes = BytesHelper.IntToBytes(value);
			var byteDescription = AddBytes(bytes, description, isReadOnly, ignoreUnequal);
			byteDescription.RealValue = value.ToString();
			return byteDescription;
		}

		public void SetShort(ByteDescription byteDescription, int value)
		{
			var bytes = BytesHelper.ShortToBytes((short)value);
			for (int i = 0; i < bytes.Count; i++)
			{
				var index = ByteDescriptions.IndexOf(byteDescription);
				ByteDescriptions[index + i].Value = bytes[i];
			}
			byteDescription.RealValue = value.ToString();
		}

		public ByteDescription AddByte(int value, string description = null, bool isReadOnly = false, bool ignoreUnequal = false)
		{
			var byteDescription = AddBytes(new List<byte>() { (byte)value }, description, isReadOnly, ignoreUnequal);
			return byteDescription;
		}

		public void AddString(string value, string description = null, int lenght = 20)
		{
			value = value.Replace("№", "N");
			var bytes = BytesHelper.StringToBytes(value, lenght);
			var byteDescription = AddBytes(bytes, description);
			byteDescription.RealValue = value;
		}

		public ByteDescription AddReference(ByteDescription byteDescription, string description = null)
		{
			var newByteDescription = AddBytes(new List<byte>() { 0, 0, 0 }, description);
			newByteDescription.AddressReference = byteDescription;
			return newByteDescription;
		}

		public void AddReference(BytesDatabase bytesDatabase, string description = null)
		{
			var byteDescriptions = AddBytes(new List<byte>() { 0, 0, 0 }, description);
			if (bytesDatabase != null)
				byteDescriptions.AddressReference = bytesDatabase.ByteDescriptions.FirstOrDefault();
		}

		public ByteDescription AddBytes(List<byte> bytes, string description = null, bool isReadOnly = false, bool ignoreUnequal = false)
		{
			var byteDescriptions = new List<ByteDescription>();
			foreach (var b in bytes)
			{
				var byteDescription = new ByteDescription()
				{
					Value = b,
                    IsReadOnly = isReadOnly,
					IgnoreUnequal = ignoreUnequal
				};
				byteDescriptions.Add(byteDescription);
			}
			var firstByteDescriptions = byteDescriptions[0];
			firstByteDescriptions.Description = description;
			ByteDescriptions.AddRange(byteDescriptions);
			return firstByteDescriptions;
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

		public void Order(int startOffset = 0)
		{
			for (int i = 0; i < ByteDescriptions.Count; i++)
			{
				ByteDescriptions[i].Offset = startOffset + i;
				ByteDescriptions[i].RelativeOffset = i;
			}
		}

		public void ResolveTableReferences()
		{
			foreach (var byteDescription in ByteDescriptions)
			{
				if (byteDescription.TableBaseReference != null)
				{
					byteDescription.AddressReference = byteDescription.TableBaseReference.BytesDatabase.ByteDescriptions.FirstOrDefault();
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
					var value = byteDescription.AddressReference.Offset;
					var bytes = BitConverter.GetBytes(value);
					if (BytesHelper.IsRevese)
					{
						ByteDescriptions[i + 0].Value = bytes[0];
						ByteDescriptions[i + 1].Value = bytes[1];
						ByteDescriptions[i + 2].Value = bytes[2];
					}
					else
					{
						ByteDescriptions[i + 0].Value = bytes[2];
						ByteDescriptions[i + 1].Value = bytes[1];
						ByteDescriptions[i + 2].Value = bytes[0];
						byteDescription.RealValue = value.ToString();
					}
				}
			}
		}

		public List<byte> GetBytes()
		{
			var bytes = new List<byte>();
			foreach (var byteDescription in ByteDescriptions)
			{
				bytes.Add((byte)byteDescription.Value);
			}
			return bytes;
		}
	}
}