using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FiresecAPI.Models
{
    [DataContract]
    public class ElementEllipse : ElementBase, IZIndexedElement
    {
        public ElementEllipse()
        {
            BackgroundColor = Colors.DarkRed;
            BorderColor = Colors.Orange;
            BorderThickness = 1;
        }

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
            var ellipse = new Ellipse()
            {
                Fill = new SolidColorBrush(BackgroundColor),
                Stroke = new SolidColorBrush(BorderColor),
                StrokeThickness = BorderThickness
            };

            if (BackgroundPixels != null)
            {
                ellipse.Fill = PlanElementsHelper.CreateBrush(BackgroundPixels);
            }

            return ellipse;
        }

        public override ElementBase Clone()
        {
            ElementEllipse elementBase = new ElementEllipse()
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
