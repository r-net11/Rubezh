using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace Firesec.Helper
{
    public static class MetadataHelper
    {
        static MetadataHelper()
        {
            string MetadataString = FiresecApi.Resource1.FiresecMetadata;
            byte[] bytes = Encoding.Default.GetBytes(MetadataString);
            MemoryStream memoryStream = new MemoryStream(bytes);

            XmlSerializer serializer = new XmlSerializer(typeof(Firesec.Metadata.config));
            Metadata = (Firesec.Metadata.config)serializer.Deserialize(memoryStream);
            memoryStream.Close();

            //XmlSerializer serializer = new XmlSerializer(typeof(Firesec.Metadata.config));
            //FileStream fileStream = new FileStream("..\\..\\metadata.xml", FileMode.Open);
            //Metadata = (Firesec.Metadata.config)serializer.Deserialize(fileStream);
        }

        public static Firesec.Metadata.config Metadata { get; set; }
    }
}
