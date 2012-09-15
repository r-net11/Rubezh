using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Common;
using Firesec.Groups;
using Firesec.Models.IndicatorsLogic;
using Firesec.Models.ZonesLogic;

namespace Firesec
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
				if ((string.IsNullOrEmpty(input)) && (input == "0"))
				{
					Logger.Error("SerializerHelper.Deserialize<" + typeof(T).ToString() + "> input IsNullOrEmpty");
					return default(T);
				}
				input = input.Replace("&#xD;&#xA;", "");
				using (var memoryStream = new MemoryStream(Encoding.Default.GetBytes(input)))
				{
					var serializer = new XmlSerializer(typeof(T));
					return (T)serializer.Deserialize(memoryStream);
				}
			}
			catch (InvalidOperationException e)
			{
				var firstCharsInInput = input.Substring(0, Math.Min(input.Length, 100));
				Logger.Error("Исключение InvalidOperationException при вызове SerializerHelper.Deserialize<" + typeof(T).ToString() + "> " + firstCharsInInput);
				return default(T);
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове SerializerHelper.Deserialize<" + typeof(T).ToString() + ">");
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