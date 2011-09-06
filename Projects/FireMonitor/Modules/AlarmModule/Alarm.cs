using FiresecAPI.Models;
using System;

namespace AlarmModule
{
    public class Alarm
    {
        public AlarmType AlarmType { get; set; }
        public string Name { get; set; }
        public string Time { get; set; }
        public Guid DeviceUID { get; set; }
        public string ClassId { get; set; }
    }
}