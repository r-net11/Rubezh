using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data
{
    [Serializable]
    public class EventBindingManager
    {
        public List<EventBindingItem> EventBindingItems { get; set; }
    }

    [Serializable]
    public class EventBindingItem
    {
        public int SourceId { get; set; }
        public string SourceEventName { get; set; }
        public int DestinationId { get; set; }
        public string DestinationFunctionName { get; set; }
    }
}
