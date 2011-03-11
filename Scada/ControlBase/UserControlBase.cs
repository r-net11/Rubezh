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

namespace ControlBase
{
    [Serializable]
    public class UserControlBase : UserControl, ISerializable
    {
        static UserControlBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(UserControlBase), new FrameworkPropertyMetadata(typeof(UserControlBase)));
        }

        public UserControlBase()
        {
            BorderColor = Colors.White;
            BorderHeight = 0;
        }

        Border border;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            border = (Border)this.Template.FindName("PART_BORDER", this);

            BorderColor = BorderColor;
            BorderHeight = BorderHeight;
        }

        Color borderColor;
        public Color BorderColor
        {
            get { return borderColor; }
            set
            {
                borderColor = value;
                if (border != null)
                border.BorderBrush = new SolidColorBrush(borderColor);
            }
        }

        double borderHeight;
        public double BorderHeight
        {
            get { return borderHeight; }
            set
            {
                borderHeight = value;
                if (border != null)
                border.BorderThickness = new Thickness(borderHeight);
            }
        }

        double top;
        public double Top
        {
            get { return top; }
            set
            {
                top = value;
            }
        }

        double left;
        public double Left
        {
            get { return left; }
            set
            {
                left = value;
            }
        }

        int id;
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        protected UserControlBase(SerializationInfo info, StreamingContext ctxt)
            : this()
        {
            Width = (double)info.GetValue("Width", typeof(double));
            Height = (double)info.GetValue("Height", typeof(double));
            Top = (double)info.GetValue("Top", typeof(double));
            Left = (double)info.GetValue("Left", typeof(double));
            BorderColor = (Color)ColorConverter.ConvertFromString((string)info.GetValue("BorderColor", typeof(string)));
            BorderHeight = (double)info.GetValue("BorderHeight", typeof(double));
            Id = (int)info.GetValue("Id", typeof(int));
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Width", Width);
            info.AddValue("Height", Height);
            info.AddValue("Top", Top);
            info.AddValue("Left", Left);
            info.AddValue("BorderColor", BorderColor.ToString());
            info.AddValue("BorderHeight", BorderHeight);
            info.AddValue("Id", Id);
        }

        public BitmapImage GetImage()
        {
            return (BitmapImage)FindResource("IconImage");
        }
    }
}
