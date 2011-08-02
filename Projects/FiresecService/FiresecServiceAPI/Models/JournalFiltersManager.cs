using System;
using System.Collections.Generic;

namespace FiresecAPI.Models
{
    [Serializable]
    public class JournalFiltersManager
    {
        public List<JournalFilter> Filters { get; set; }
    }
}
