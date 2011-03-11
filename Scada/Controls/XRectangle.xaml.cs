using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.Serialization;

namespace Controls
{
    [Serializable]
    public partial class XRectangle : ControlBase.UserControlBase
    {
        public XRectangle()
            : base()
        {
            InitializeComponent();

            ForefrontColor = Colors.Black;
            BackgroundColor = Colors.Green;
        }

        Color forefrontColor;
        public Color ForefrontColor
        {
            get { return forefrontColor; }
            set
            {
                forefrontColor = value;
                //textBox.Foreground = new SolidColorBrush(forefrontColor);
            }
        }

        Color backgroundColor;
        public Color BackgroundColor
        {
            get { return backgroundColor; }
            set
            {
                backgroundColor = value;
                rectangle.Fill = new SolidColorBrush(backgroundColor);
                //textBox.Background = new SolidColorBrush(backgroundColor);
            }
        }

        protected XRectangle(SerializationInfo info, StreamingContext ctxt)
            : base(info, ctxt)
        {
            InitializeComponent();

            ForefrontColor = (Color)ColorConverter.ConvertFromString((string)info.GetValue("ForefrontColor", typeof(string)));
            BackgroundColor = (Color)ColorConverter.ConvertFromString((string)info.GetValue("BackgroundColor", typeof(string)));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            base.GetObjectData(info, ctxt);
            info.AddValue("ForefrontColor", ForefrontColor.ToString());
            info.AddValue("BackgroundColor", BackgroundColor.ToString());
        }
    }
}
