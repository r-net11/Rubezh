using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace FiresecMetadata
{
    public static class MetadataHelper
    {
        static MetadataHelper()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Firesec.Metadata.config));
            FileStream fileStream = new FileStream("..\\..\\metadata.xml", FileMode.Open);
            Metadata = (Firesec.Metadata.config)serializer.Deserialize(fileStream);
        }

        public static Firesec.Metadata.config Metadata { get; set; }
    }
}
