using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlarmModule
{
    public class Alarm
    {
        public AlarmType AlarmType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Time { get; set; }

        public string Path { get; set; }
        public string ZoneNo { get; set; }
    }
}
