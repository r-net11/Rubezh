using FiresecAPI.Models;

namespace AlarmModule
{
    public class Alarm
    {
        public AlarmType AlarmType { get; set; }
        public string Name { get; set; }
        public string Time { get; set; }
        public string DeviceId { get; set; }
        public string ClassId { get; set; }
    }
}