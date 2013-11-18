using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
    [DataContract]
    public class HexFileCollectionInfo
    {
        public HexFileCollectionInfo()
        {
            FileInfos = new List<HEXFileInfo>();
            DriverType = XDriverType.GK;
            Name = "Обновление прошивки";
            MinorVersion = 1;
            MajorVersion = 1;
        }

        [DataMember]
        public XDriverType DriverType { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int MinorVersion { get; set; }

        [DataMember]
        public int MajorVersion { get; set; }

        [DataMember]
        public List<HEXFileInfo> FileInfos { get; set; }
    }
}