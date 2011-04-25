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

namespace Controls
{
    [Serializable]
    public partial class XDetector : ControlBase.UserControlBase
    {
        public XDetector()
            : base()
        {
            InitializeComponent();
            LineColor = Colors.Green;
        }

        [FunctionAttribute]
        public void Function_1()
        {
            MessageBox.Show("Function_1 was called");
        }

        [FunctionAttribute]
        public void Function_2()
        {
            MessageBox.Show("Function_2 was called");
        }

        [EventAttribute]
        public event Action XEvent;
        void OnXEvent()
        {
            if (XEvent != null)
            {
                XEvent();
            }
        }

        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(string), typeof(XDetector),
            new FrameworkPropertyMetadata(OnStatePropertyChanged));

        [CanBindAttribute]
        public string State
        {
            get { return (string)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        private static void OnStatePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            XDetector current = source as XDetector;
            string state = (string)e.NewValue;

            switch (state)
            {
                case "Fire":
                    current.LineColor = Colors.Red;
                    current.OnXEvent();
                    break;
                case "Normal":
                    current.LineColor = Colors.Green;
                    break;
                case "Failure":
                    current.LineColor = Colors.Gray;
                    break;
            }
        }

        Color lineColor;
        public Color LineColor
        {
            get { return lineColor; }
            set
            {
                lineColor = value;
                this.InvalidateVisual();
            }
        }

        protected XDetector(SerializationInfo info, StreamingContext ctxt)
            : base(info, ctxt)
        {
            InitializeComponent();
            LineColor = (Color)ColorConverter.ConvertFromString((string)info.GetValue("LineColor", typeof(string)));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            base.GetObjectData(info, ctxt);
            info.AddValue("LineColor", LineColor.ToString());
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            double w = this.RenderSize.Width;
            double h = this.RenderSize.Height;
            drawingContext.DrawLine(new Pen(new SolidColorBrush(LineColor), 5), new Point(w / 2, h), new Point(w / 2, h / 2));
            DrawArc(drawingContext, null, new Pen(new SolidColorBrush(LineColor), 5),
                new Point(w, 0),
                new Point(0, 0),
                new Size(w / 2, h / 2));
        }

        void DrawArc(DrawingContext drawingContext, Brush brush, Pen pen, Point start, Point end, Size radius)
        {
            PathGeometry geometry = new PathGeometry();
            PathFigure figure = new PathFigure();
            geometry.Figures.Add(figure);
            figure.StartPoint = start;

            figure.Segments.Add(new ArcSegment(end, radius, 0, false, SweepDirection.Clockwise, true));

            drawingContext.DrawGeometry(brush, pen, geometry);
        }
    }
}
