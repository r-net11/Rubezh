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
    public partial class XButton : ControlBase.UserControlBase
    {
        public XButton()
            :base()
        {
            InitializeComponent();

            ForefrontColor = Colors.White;
            BackgroundColor = Colors.DarkRed;
        }

        Color forefrontColor;
        public Color ForefrontColor
        {
            get { return forefrontColor; }
            set
            {
                forefrontColor = value;
                button.Foreground = new SolidColorBrush(forefrontColor);
            }
        }

        Color backgroundColor;
        public Color BackgroundColor
        {
            get { return backgroundColor; }
            set
            {
                backgroundColor = value;
                button.Background = new SolidColorBrush(backgroundColor);
            }
        }

        string caption;
        public string Caption
        {
            get { return caption; }
            set
            {
                caption = value;
                button.Content = caption;
            }
        }

        protected XButton(SerializationInfo info, StreamingContext ctxt)
            : base(info, ctxt)
        {
            InitializeComponent();

            ForefrontColor = (Color)ColorConverter.ConvertFromString((string)info.GetValue("ForefrontColor", typeof(string)));
            BackgroundColor = (Color)ColorConverter.ConvertFromString((string)info.GetValue("BackgroundColor", typeof(string)));
            Caption = (string)info.GetValue("Caption", typeof(string));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            base.GetObjectData(info, ctxt);
            info.AddValue("ForefrontColor", ForefrontColor.ToString());
            info.AddValue("BackgroundColor", BackgroundColor.ToString());
            info.AddValue("Caption", Caption);
        }
    }
}
