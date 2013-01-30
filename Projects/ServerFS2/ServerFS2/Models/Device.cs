using System.Collections.Generic;

namespace ServerFS2
{
    public class Device
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Version { get; set; }
        public string SerialNo { get; set; }
        public int UsbChannel { get; set; }
        public List<JournalItem> JournalItems { get; set; }
        public List<JournalItem> SecJournalItems { get; set; }
        public Device()
        {
            JournalItems = new List<JournalItem>();
            SecJournalItems = new List<JournalItem>();
        }
    }
}
