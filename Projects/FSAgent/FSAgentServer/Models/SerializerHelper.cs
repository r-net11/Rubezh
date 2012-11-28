using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Common;

namespace FSAgentServer
{
	public static class SerializerHelper
	{
		public static T Deserialize<T>(string input)
		{
			try
			{
				if ((string.IsNullOrEmpty(input)) || (input == "0") || !input.StartsWith("<?xml"))
				{
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