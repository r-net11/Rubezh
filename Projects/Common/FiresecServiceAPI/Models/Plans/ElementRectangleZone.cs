using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FiresecAPI.Models
{
    [DataContract]
    public class ElementRectangleZone : ElementBase
    {
        public ElementRectangleZone()
        {
        }

        [DataMember]
        public ulong? ZoneNo { get; set; }

        [DataMember]
        public Color BackgroundColor { get; set; }

        public override FrameworkElement Draw()
        {
            var rectangle = new Rectangle()
            {
                Fill = new SolidColorBrush(BackgroundColor),
            };

            return rectangle;
        }

        public override ElementBase Clone()
        {
            ElementBase elementBase = new ElementRectangleZone()
            {
                BackgroundColor = BackgroundColor,
                ZoneNo = ZoneNo
            };
            Copy(elementBase);
            return elementBase;
        }
    }
}
