using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FiresecAPI.Models
{
    [DataContract]
    public class ElementRectangle : ElementBase, IZIndexedElement
    {
        public ElementRectangle()
        {
            BackgroundColor = Colors.DarkRed;
            BorderColor = Colors.Orange;
            BorderThickness = 1;
        }

        public int idElementCanvas { get; set; }

        [DataMember]
        public byte[] BackgroundPixels { get; set; }

        [DataMember]
        public Color BackgroundColor { get; set; }

        [DataMember]
        public Color BorderColor { get; set; }

        [DataMember]
        public double BorderThickness { get; set; }

        [DataMember]
        public int ZIndex { get; set; }

        public override FrameworkElement Draw()
        {
            var rectangle = new Rectangle()
            {
                Fill = new SolidColorBrush(BackgroundColor),
                Stroke = new SolidColorBrush(BorderColor),
                StrokeThickness = BorderThickness
            };

            if (BackgroundPixels != null)
            {
                rectangle.Fill = PlanElementsHelper.CreateBrush(BackgroundPixels); ;
            }

            return rectangle;
        }

        public override ElementBase Clone()
        {
            ElementRectangle elementBase = new ElementRectangle()
            {
                BackgroundColor = BackgroundColor,
                BorderColor = BorderColor,
                BorderThickness = BorderThickness
            };
            if (BackgroundPixels != null)
                elementBase.BackgroundPixels = (byte[])BackgroundPixels.Clone();
            Copy(elementBase);
            return elementBase;
        }
    }
}
