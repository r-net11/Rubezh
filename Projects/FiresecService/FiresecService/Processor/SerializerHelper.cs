using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Firesec.Groups;
using Firesec.IndicatorsLogic;
using Firesec.ZonesLogic;
using Common;

namespace FiresecService.Processor
{
	public static class SerializerHelper
	{
		public static expr GetZoneLogic(string zoneLogicString)
		{
			try
			{
				return Deserialize<expr>(zoneLogicString);
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове SerializerHelper.GetZoneLogic");
				return null;
			}
		}

		public static string SetZoneLogic(expr zoneLogic)
		{
			return Serialize<expr>(zoneLogic);
		}

		public static LEDProperties GetIndicatorLogic(string indicatorLogicString)
		{
			try
			{
				return Deserialize<LEDProperties>(indicatorLogicString);
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове SerializerHelper.GetIndicatorLogic");
				return null;
			}
		}

		public static string SetIndicatorLogic(LEDProperties indicatorLogic)
		{
			return Serialize<LEDProperties>(indicatorLogic);
		}

		public static RCGroupProperties GetGroupProperties(string groupPropertyString)
		{
			try
			{
				return Deserialize<RCGroupProperties>(groupPropertyString);
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове SerializerHelper.GetGroupProperties");
				return null;
			}
		}

		public static string SeGroupProperty(RCGroupProperties groupProperty)
		{
			return Serialize<RCGroupProperties>(groupProperty);
		}

		public static T Deserialize<T>(string input)
		{
			if (string.IsNullOrEmpty(input))
				return default(T);

			try
			{
				using (var memoryStream = new MemoryStream(Encoding.Default.GetBytes(input)))
				{
					var serializer = new XmlSerializer(typeof(T));
					return (T)serializer.Deserialize(memoryStream);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове SerializerHelper.Deserialize<T>");
				return default(T);
			}
		}

		public static string Serialize<T>(T input)
		{
			using (var memoryStream = new MemoryStream())
			{
				var serializer = new XmlSerializer(typeof(T));
				serializer.Serialize(memoryStream, input);

				string output = Encoding.UTF8.GetString(memoryStream.ToArray());
				output = output.Replace(" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");
				output = output.Replace("\r\n", "");

				return output;
			}
		}
	}
}