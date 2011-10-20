using System;
using System.Runtime.Serialization;
using System.Windows;

namespace FiresecAPI.Models
{
    [DataContract]
    public class ElementDevice : ElementBase
    {
        public ElementDevice()
        {
            Width = 20;
            Height = 20;
        }

        [DataMember]
        public Guid Id { get; set; }

        public override FrameworkElement Draw()
        {
            return null;
        }
    }
}
