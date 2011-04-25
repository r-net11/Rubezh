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
using ControlBase;
using System.Windows.Media.Animation;

namespace Controls
{
    [Serializable]
    public partial class XAlarm : ControlBase.UserControlBase
    {
        public XAlarm()
        {
            InitializeComponent();
        }

        [FunctionAttribute]
        public void BeginAlarm()
        {
            Storyboard storyboard = new Storyboard();
            ColorAnimation colorAnimation = new ColorAnimation();
            colorAnimation.From = Colors.Red;
            colorAnimation.To = Colors.Blue;
            colorAnimation.Duration = TimeSpan.FromSeconds(1);
            Storyboard.SetTargetName(colorAnimation, "ellipse");
            Storyboard.SetTargetProperty(colorAnimation, new PropertyPath("Fill.Color"));
            storyboard.Children.Add(colorAnimation);
            storyboard.AutoReverse = true;
            storyboard.RepeatBehavior = RepeatBehavior.Forever;
            ellipse.BeginStoryboard(storyboard);
        }

        protected XAlarm(SerializationInfo info, StreamingContext ctxt)
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
