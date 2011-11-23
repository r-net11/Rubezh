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
            DeviceUID = Guid.Empty;
        }

        public Device Device { get; set; }

        [DataMember]
        public Guid DeviceUID { get; set; }

        public override FrameworkElement Draw()
        {
            return null;
        }

        public override ElementBase Clone()
        {
            ElementBase elementBase = new ElementDevice()
            {
                DeviceUID = DeviceUID
            };
            Copy(elementBase);
            return elementBase;
        }
    }
}
