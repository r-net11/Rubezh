using System;
using System.IO;
using System.Reflection;

namespace Common
{
	public static class AssemblyHelper
	{
		private const int c_PeHeaderOffset = 60;
		private const int c_LinkerTimestampOffset = 8;

		public static DateTime GetAssemblyTimestamp(Assembly assembly)
		{
			string filePath = assembly.Location;
			byte[] b = new byte[2048];
			Stream stream = null;
			try
			{
				stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
				stream.Read(b, 0, 2048);
			}
			finally
			{
				if (stream != null)
					stream.Close();
			}

			int i = System.BitConverter.ToInt32(b, c_PeHeaderOffset);
			int secondsSince1970 = System.BitConverter.ToInt32(b, i + c_LinkerTimestampOffset);
			DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0); //TODO: ???
			dt = dt.AddSeconds(secondsSince1970);
			dt = dt.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(dt).Hours);
			return dt;
		}
	}
}