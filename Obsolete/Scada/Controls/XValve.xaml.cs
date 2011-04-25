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
using ControlBase;
using System.Runtime.Serialization;
using System.Windows.Media.Animation;

namespace Controls
{
    [Serializable]
    public partial class XValve : ControlBase.UserControlBase
    {
        public XValve()
            :base()
        {
            InitializeComponent();
        }

        [FunctionAttribute]
        public void Open()
        {
            Storyboard storyboard = new Storyboard();
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = 0;
            doubleAnimation.To = Width;
            doubleAnimation.Duration = TimeSpan.FromSeconds(1);
            Storyboard.SetTargetName(doubleAnimation, rectangle1.Name);
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(Rectangle.WidthProperty));
            storyboard.Children.Add(doubleAnimation);
            storyboard.Completed += new EventHandler(storyboard_Completed);
            this.BeginStoryboard(storyboard);
        }

        void storyboard_Completed(object sender, EventArgs e)
        {
            OnOpened();
        }

        [FunctionAttribute]
        public void Close()
        {
        }

        [EventAttribute]
        public event Action Opened;
        void OnOpened()
        {
            if (Opened != null)
                Opened();
        }

        [EventAttribute]
        public event Action Closed;
        void OnClosed()
        {
            if (Closed != null)
                Closed();
        }

        protected XValve(SerializationInfo info, StreamingContext ctxt)
            : base(info, ctxt)
        {
            InitializeComponent();
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            base.GetObjectData(info, ctxt);
        }
    }
}
