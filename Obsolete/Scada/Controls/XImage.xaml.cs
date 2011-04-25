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
    public partial class XImage : ControlBase.UserControlBase
    {
        public XImage()
            : base()
        {
            InitializeComponent();
        }

        Stretch imageStretch;
        public Stretch ImageStretch
        {
            get { return imageStretch; }
            set
            {
                imageStretch = value;
                image.Stretch = imageStretch;
            }
        }

        string imageSource;
        public String ImageSource
        {
            get { return imageSource; }
            set
            {
                imageSource = value;
                if (imageSource != null)
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri(imageSource);
                    bitmapImage.EndInit();
                    image.Source = bitmapImage;
                }
            }
        }

        protected XImage(SerializationInfo info, StreamingContext ctxt)
            : base(info, ctxt)
        {
            InitializeComponent();

            ImageStretch = (Stretch)info.GetValue("ImageStretch", typeof(Stretch));
            ImageSource = (string)info.GetValue("ImageSource", typeof(string));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            base.GetObjectData(info, ctxt);
            info.AddValue("ImageStretch", ImageStretch);
            info.AddValue("ImageSource", ImageSource);
        }
    }
}
