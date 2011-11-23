using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FiresecAPI.Models
{
    [DataContract]
    public class ElementRectangleZone : ElementBase, IElementZone
    {
        public Zone Zone { get; set; }

        [DataMember]
        public ulong? ZoneNo { get; set; }

        public override FrameworkElement Draw()
        {
            var rectangle = new Rectangle()
            {
                Fill = new SolidColorBrush(ElementZoneHelper.GetZoneColor(Zone)),
            };

            return rectangle;
        }

        public override ElementBase Clone()
        {
            ElementBase elementBase = new ElementRectangleZone()
            {
                ZoneNo = ZoneNo
            };
            Copy(elementBase);
            return elementBase;
        }
    }
}
