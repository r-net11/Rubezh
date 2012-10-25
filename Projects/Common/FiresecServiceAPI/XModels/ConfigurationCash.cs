using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XFiresecAPI
{
    public static class XConfigurationCash
    {
        public static XDriversConfiguration XDriversConfiguration { get; set; }
        static XConfigurationCash()
        {
            XDriversConfiguration = new XDriversConfiguration();
        }
    }
}
