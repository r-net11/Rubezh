using System.Collections.Generic;

namespace XFiresecAPI
{
    public class XDriversConfiguration
    {
        public XDriversConfiguration()
        {
            XDrivers = new List<XDriver>();
        }

        public List<XDriver> XDrivers { get; set; }
    }
}