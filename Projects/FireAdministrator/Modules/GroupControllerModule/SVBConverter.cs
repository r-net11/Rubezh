using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GKModule.Database;
using System.IO;

namespace GKModule
{
	public static class SVBConverter
	{
		public static void Convert1()
		{
			DatabaseProcessor.Convert();
			var gkDatabase = DatabaseProcessor.DatabaseCollection.GkDatabases.First();

			var fileBytes = new List<byte>();
			fileBytes.Add(0x25);
			fileBytes.Add(0x08);
			fileBytes.Add(0x19);
			fileBytes.Add(0x65);
			fileBytes.AddRange(BytesHelper.ShortToBytes((short)(gkDatabase.BinaryObjects.Count + 1)));
			fileBytes.AddRange(BytesHelper.ShortToBytes((short)0));
			fileBytes.AddRange(BytesHelper.ShortToBytes((short)0));
			fileBytes.AddRange(BytesHelper.ShortToBytes((short)0));
			fileBytes.AddRange(BytesHelper.ShortToBytes((short)0));

			foreach (var binaryObject in gkDatabase.BinaryObjects)
			{
				var bytes = binaryObject.AllBytes;

				fileBytes.AddRange(BytesHelper.ShortToBytes((short)(binaryObject.GetNo())));
				fileBytes.Add(0);
				fileBytes.AddRange(BytesHelper.ShortToBytes((short)bytes.Count));
				//for (int j = 0; j < 33; j++)
				//{
				//    fileBytes.Add(32);
				//}
				fileBytes.AddRange(StringDescriptionToBytes(binaryObject.Device.Driver.ShortName + " - " + binaryObject.Device.Address));
				fileBytes.AddRange(bytes);
				for (int i = 0; i < 256 - gkDatabase.BinaryObjects.Count; i++)
				{
					fileBytes.Add(0);
				}
			}
			fileBytes.AddRange(BytesHelper.ShortToBytes((short)(gkDatabase.BinaryObjects.Count)));
			fileBytes.Add(0);
			fileBytes.AddRange(BytesHelper.ShortToBytes((short)2));
			//for (int j = 0; j < 33; j++)
			//{
			//    fileBytes.Add(32);
			//}
			fileBytes.AddRange(StringDescriptionToBytes("Завершающий дескриптор"));
			fileBytes.Add(255);
			fileBytes.Add(255);
			for (int i = 0; i < 256 - 2; i++)
			{
				fileBytes.Add(0);
			}

			Directory.CreateDirectory(@"D:\GKConfig");

			using (var fileStream = new FileStream(@"D:\GKConfig\GK.GKBIN", FileMode.Create))
			{
				fileStream.Write(fileBytes.ToArray(), 0, fileBytes.Count);
			}

			using (var streamWriter = new StreamWriter(@"D:\GKConfig\GK.gkprj"))
			{
				var stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("<congig>");
				stringBuilder.AppendLine("<gk name=\"GK.GKBIN\" description=\"описание ГК\"/>");
				stringBuilder.AppendLine("</congig>");

				streamWriter.Write(stringBuilder.ToString());
			}
		}

		public static void Convert2()
		{
			DatabaseProcessor.Convert();
			var gkDatabase = DatabaseProcessor.DatabaseCollection.GkDatabases.First();

			Directory.CreateDirectory(@"D:\GKConfig");

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

		static List<byte> StringDescriptionToBytes(string str)
		{
			var bytes = new List<byte>();
			foreach (var c in str)
			{
				bytes.Add((byte)c);
			}
			for(int i = 0; i < 32 - str.Length; i++)
			{
				bytes.Add(32);
			}
			return bytes;
		}
	}
}