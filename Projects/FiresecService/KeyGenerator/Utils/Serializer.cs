using System.IO;

namespace KeyGenerator.Utils
{
	public static class Serializer
	{
		public static T Deserialize<T>(string input) where T : class
		{
			var ser = new System.Xml.Serialization.XmlSerializer(typeof(T));

			using (var sr = new StringReader(input))
				return (T)ser.Deserialize(sr);
		}
	}
}
