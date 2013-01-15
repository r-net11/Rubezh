using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace TestUSB
{
	public static class MetadataHelper
	{
		public static Rubezh2010.driverConfig Metadata { get; private set; }

		public static void Initialize()
		{
			using (var fileStream = new FileStream("rubezh2010.xml", FileMode.Open))
			{
				var serializer = new XmlSerializer(typeof(Rubezh2010.driverConfig));
				Metadata = (Rubezh2010.driverConfig)serializer.Deserialize(fileStream);
			}
		}

		public static string GetEventByCode(int eventCode)
		{
			string stringCode = "$" + eventCode.ToString("X2");
			var nativeEvent = Metadata.events.FirstOrDefault(x => x.rawEventCode == stringCode);
			if (nativeEvent != null)
				return nativeEvent.eventMessage;
			return "Неизвестный код события " + eventCode.ToString("x2");
		}

		public static string GetExactEventForFlag(string eventName, int flag)
		{
			var firstIndex = eventName.IndexOf("[");
			var lastIndex = eventName.IndexOf("]");
			if (firstIndex != -1 && lastIndex != -1)
			{
				var firstPart = eventName.Substring(0, firstIndex);
				var secondPart = eventName.Substring(firstIndex + 1, lastIndex - firstIndex - 1);
				var secondParts = secondPart.Split('/');
				if (flag < secondParts.Count())
				{
					var choise = secondParts[flag];
					return firstPart + choise;
				}
			}
			return eventName;
		}
	}
}