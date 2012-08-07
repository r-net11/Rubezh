using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Common.GK;
using FiresecClient;

namespace GKModule
{
	public static class BinaryFileConverter
	{
		public static void Convert()
		{
			DatabaseManager.Convert();

			var stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("<congig>");

			var gkDatabase = DatabaseManager.GkDatabases.First();
			SaveToFile(gkDatabase, @"D:\GKConfig\GK.GKBIN");
			stringBuilder.AppendLine("<gk name=\"GK.GKBIN\" description=\"описание ГК\"/>");

			foreach (var kauDatabase in DatabaseManager.KauDatabases)
			{
				var kauDevice = kauDatabase.Devices[0];
				short lineNo = XManager.GetKauLine(kauDevice);
				string lineTypeName = lineNo == 0 ? "baseline" : "reserveline";
				string fileName = "Kau" + kauDevice.Address + ".GKBIN";
				SaveToFile(kauDatabase, @"D:\GKConfig\" + fileName);
				stringBuilder.AppendLine("<kau name=\"" + fileName + "\" line=\"" + lineTypeName + "\" address=\"" + kauDevice.Address + "\" description=\"описание КАУ\"/>");
			}

			stringBuilder.AppendLine("</congig>");

			using (var streamWriter = new StreamWriter(@"D:\GKConfig\GK.gkprj"))
			{
				streamWriter.Write(stringBuilder.ToString());
			}
		}

		static void SaveToFile(CommonDatabase commonDatabase, string fileName)
		{
			var fileBytes = new List<byte>();
			fileBytes.Add(0x25);
			fileBytes.Add(0x08);
			fileBytes.Add(0x19);
			fileBytes.Add(0x65);
			fileBytes.AddRange(BytesHelper.ShortToBytes((short)(commonDatabase.BinaryObjects.Count + 1)));
			fileBytes.AddRange(BytesHelper.ShortToBytes((short)0));
			fileBytes.AddRange(BytesHelper.ShortToBytes((short)0));
			fileBytes.AddRange(BytesHelper.ShortToBytes((short)0));
			fileBytes.AddRange(BytesHelper.ShortToBytes((short)0));

			foreach (var binaryObject in commonDatabase.BinaryObjects)
			{
				fileBytes.AddRange(CreateDescriptor(binaryObject));
			}
			fileBytes.AddRange(CreateEndDescriptor((short)commonDatabase.BinaryObjects.Count));

			using (var fileStream = new FileStream(fileName, FileMode.Create))
			{
				fileStream.Write(fileBytes.ToArray(), 0, fileBytes.Count);
			}
		}

		public static void CreateListOfFilesForGk()
		{
			DatabaseManager.Convert();
			var gkDatabase = DatabaseManager.GkDatabases.First();

			foreach (var binaryObject in gkDatabase.BinaryObjects)
			{
				var fileName = "dsc" + String.Format("{0:00000}", binaryObject.GetNo()) + ".gk";
				var bytes = binaryObject.AllBytes;
				using (var fileStream = new FileStream(@"D:\GKConfig\" + fileName, FileMode.Create))
				{
					fileStream.Write(bytes.ToArray(), 0, bytes.Count);
				}
			}
		}

		public static List<byte> CreateDescriptor(BinaryObjectBase binaryObject)
		{
			var resultBytes = new List<byte>();
			var bytes = binaryObject.AllBytes;

			resultBytes.AddRange(BytesHelper.ShortToBytes((short)(binaryObject.GetNo())));
			resultBytes.Add(1);
			resultBytes.AddRange(BytesHelper.ShortToBytes((short)bytes.Count));
			resultBytes.AddRange(BytesHelper.StringDescriptionToBytes(binaryObject.BinaryBase.GetBinaryDescription(), 33));
			resultBytes.AddRange(bytes);
			var resultButesCount = resultBytes.Count;
			for (int i = 0; i < 256 - resultButesCount; i++)
			{
				resultBytes.Add(0);
			}
			return resultBytes;
		}

		public static List<byte> CreateEndDescriptor(short descriptorNo)
		{
			var resultBytes = new List<byte>();
			resultBytes.AddRange(BytesHelper.ShortToBytes(descriptorNo));
			resultBytes.Add(1);
			resultBytes.AddRange(BytesHelper.ShortToBytes((short)2));
			resultBytes.AddRange(BytesHelper.StringDescriptionToBytes("Завершающий дескриптор", 33));
			resultBytes.Add(255);
			resultBytes.Add(255);
			for (int i = 0; i < 256 - 2; i++)
			{
				resultBytes.Add(0);
			}
			return resultBytes;
		}
	}
}