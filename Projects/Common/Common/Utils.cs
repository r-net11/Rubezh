using System.IO;
using System.Runtime.Serialization;

namespace Common
{
	public static class Utils
	{
		public static T Clone<T>(T obj)
			where T : class
		{
			var serializer = new DataContractSerializer(typeof(T), null, int.MaxValue, false, true, null);
			using (var ms = new MemoryStream())
			{
				serializer.WriteObject(ms, obj);
				ms.Position = 0;
				return (T)serializer.ReadObject(ms);
			}
		}
	}
}
