using StrazhAPI.Models.Layouts;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrazhAPI.Models
{
    [DataContract(Name = "LayoutsConfiguration", Namespace = "")]
    public class LayoutsConfiguration : VersionedConfiguration
    {
        public LayoutsConfiguration()
        {
            Layouts = new List<Layout>();
        }

		[DataMember]
        public List<Layout> Layouts { get; set; }

        public override bool ValidateVersion()
        {
            foreach (var layout in Layouts)
            {
                if (layout.UID == Guid.Empty)
                    layout.UID = Guid.NewGuid();
            }

            return true;
        }

        public void Update()
        {
        }
    }
}