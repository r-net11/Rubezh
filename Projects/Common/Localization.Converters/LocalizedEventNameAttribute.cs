using System;

namespace Localization.Converters
{
    public class LocalizedEventNameAttribute : Attribute
    {
        //public JournalSubsystemType JournalSubsystemType { get; set; }

        public string Name { get; set; }

        //public XStateClass StateClass { get; set; }

        public string NameInFilter { get; set; }
    }
}
