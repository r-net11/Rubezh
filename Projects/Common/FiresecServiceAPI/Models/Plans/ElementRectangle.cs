using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System;

namespace FiresecAPI.Models
{
    [DataContract]
    public class ElementRectangle : ElementBase
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
                try
                {
                    BitmapImage bitmapImage = null;
                    using (var imageStream = new MemoryStream(BackgroundPixels))
                    {
                        bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.StreamSource = imageStream;
                        bitmapImage.EndInit();
                    }
                    rectangle.Fill = new ImageBrush(bitmapImage);
                }
                catch (Exception) { ;}
            }

            return rectangle;
        }
    }
}
