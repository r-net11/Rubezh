using System;
using System.Runtime.Serialization;
using System.Windows;

namespace FiresecAPI.Models
{
    [DataContract]
    public abstract class ElementBase
    {
        public ElementBase()
        {
            UID = Guid.NewGuid();
            Width = 50;
            Height = 50;
        }

        [DataMember]
        public Guid UID { get; set; }

        [DataMember]
        public double Left { get; set; }

        [DataMember]
        public double Top { get; set; }

        [DataMember]
        public double Height { get; set; }

        [DataMember]
        public double Width { get; set; }

        public abstract FrameworkElement Draw();

        public abstract ElementBase Clone();

        protected virtual void Copy(ElementBase elementBase)
        {
            elementBase.UID = UID;
            elementBase.Left = Left;
            elementBase.Top = Top;
            elementBase.Height = Height;
            elementBase.Width = Width;
        }

        public const int BigIntConstant = 100000;
    }
}
