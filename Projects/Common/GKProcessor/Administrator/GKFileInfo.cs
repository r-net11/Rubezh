using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using FiresecAPI.GK;
using Infrastructure.Common;

namespace GKProcessor
{
	public class GKFileInfo
	{
		public static string Error { get; private set; }
		public byte MinorVersion { get; private set; }
		public byte MajorVersion { get; private set; }
		public List<byte> Hash1 { get; set; }
		public int DescriptorsCount { get; set; }
		public long FileSize { get; set; }
		public DateTime Date { get; set; }

		public List<byte> FileBytes { get; private set; }
		public List<byte> InfoBlock { get; private set; }

		void InitializeInfoBlock()
		{
			InfoBlock = new List<byte>(256) { MinorVersion, MajorVersion };
			InfoBlock.AddRange(Hash1);
			InfoBlock.AddRange(BitConverter.GetBytes(DescriptorsCount));
			InfoBlock.AddRange(BitConverter.GetBytes(FileSize));
			InfoBlock.AddRange(BitConverter.GetBytes(Date.Ticks));
			while (InfoBlock.Count < 256)
				InfoBlock.Add(0);
		}

		public static GKFileInfo BytesToGKFileInfo(List<byte> bytes)
		{
			Error = null;
			try
			{
				return new GKFileInfo
				{
					InfoBlock = bytes,
					MinorVersion = bytes[0],
					MajorVersion = bytes[1],
					Hash1 = bytes.GetRange(2, 32),
					DescriptorsCount = BitConverter.ToInt32(bytes.GetRange(34, 4).ToArray(), 0),
					FileSize = BitConverter.ToInt64(bytes.GetRange(38, 8).ToArray(), 0),
					Date = DateTime.FromBinary(BitConverter.ToInt64(bytes.GetRange(46, 8).ToArray(), 0)),
					FileBytes = new List<byte>()
				};
			}
			catch
			{
				Error = "Информационный блок поврежден";
				return null;
			}
		}

		public static bool CompareHashes(List<byte> Hash1, List<byte> Hash2)
		{
			return Hash1.SequenceEqual(Hash2);
		}
	}
}