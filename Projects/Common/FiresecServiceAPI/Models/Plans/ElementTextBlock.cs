using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FiresecAPI.Models
{
    [DataContract]
    public class ElementTextBlock : ElementBase
    {
        public ElementTextBlock()
        {
            Text = "Text";
        }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public double FontSize { get; set; }

        [DataMember]
        public string ForegroundColor { get; set; }

        [DataMember]
        public Color BackgroundColor { get; set; }

        [DataMember]
        public Color BorderColor { get; set; }

        [DataMember]
        public double BorderThickness { get; set; }

        public override FrameworkElement Draw()
        {
            var textBlock = new Label()
            {
                Background = new SolidColorBrush(BackgroundColor),
                //Foreground = new SolidColorBrush(ForegroundColor),
                BorderBrush = new SolidColorBrush(BorderColor),
                BorderThickness = new Thickness(BorderThickness),
                Content = Text
            };
            return textBlock;
        }
    }
}
