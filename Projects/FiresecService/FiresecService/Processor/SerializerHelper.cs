using System.IO;
using System.Text;
using System.Xml.Serialization;
using FiresecService.XSD;

namespace FiresecService
{
    public static class SerializerHelper
    {
        public static Firesec.ZoneLogic.expr GetZoneLogic(string zoneLogicString)
        {
            try
            {
                return Deserialize<Firesec.ZoneLogic.expr>(zoneLogicString);
            }
            catch
            {
                return null;
            }
        }

        public static string SetZoneLogic(Firesec.ZoneLogic.expr zoneLogic)
        {
            return Serialize<Firesec.ZoneLogic.expr>(zoneLogic);
        }

        public static Firesec.Indicator.LEDProperties GetIndicatorLogic(string indicatorLogicString)
        {
            try
            {
                return Deserialize<Firesec.Indicator.LEDProperties>(indicatorLogicString);
            }
            catch
            {
                return null;
            }
        }

        public static string SetIndicatorLogic(Firesec.Indicator.LEDProperties indicatorLogic)
        {
            return Serialize<Firesec.Indicator.LEDProperties>(indicatorLogic);
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