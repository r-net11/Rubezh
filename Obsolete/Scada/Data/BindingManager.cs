using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data
{
    [Serializable]
    public class BindingManager
    {
        public List<BindingItem> BindingItems { get; set; }
    }

    [Serializable]
    public class BindingItem
    {
        public int SourceId { get; set; }
        public int DestinationId { get; set; }
        public string SourceName { get; set; }
        public string DestinationName { get; set; }
    }
}
