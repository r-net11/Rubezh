using System.IO;

namespace GenerateKeyApplication.Common
{
	public static class Utilites
	{
		public static byte[] GetBytesFrom(Stream inputStream)
		{
			if (inputStream == null) return null;

			using(inputStream)
			using (var memStream = new MemoryStream())
			{
				inputStream.CopyTo(memStream);
				return memStream.ToArray();
			}
		}
	}
}
