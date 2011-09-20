using System.IO;
using System.Text;
using System.Xml.Serialization;
using Firesec.IndicatorsLogic;
using Firesec.ZonesLogic;
using Firesec.Groups;

namespace FiresecService
{
    public static class SerializerHelper
    {
        public static expr GetZoneLogic(string zoneLogicString)
        {
            try
            {
                return Deserialize<expr>(zoneLogicString);
            }
            catch
            {
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
            catch
            {
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
            catch
            {
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

            byte[] bytes = Encoding.Default.GetBytes(input);
            T output = default(T);

            using (var memoryStream = new MemoryStream(bytes))
            {
                var serializer = new XmlSerializer(typeof(T));
                output = (T) serializer.Deserialize(memoryStream);
            }

            return output;
        }

        public static string Serialize<T>(T input)
        {
            var serializer = new XmlSerializer(typeof(T));
            byte[] bytes = null;

            using (var memoryStream = new MemoryStream())
            {
                serializer.Serialize(memoryStream, input);
                bytes = memoryStream.ToArray();
            }

            string output = Encoding.UTF8.GetString(bytes);
            output = output.Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");

            return output;
        }
    }
}