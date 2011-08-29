using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Controls.Primitives;
using FiresecClient;
using System.Windows.Media;
using System.Collections.Generic;
using FiresecAPI.Models;

using System.Windows.Shapes;
using System.Windows.Input;
using PlansModule.ViewModels;

namespace PlansModule.Views
{
    public partial class PlanCanvasView : UserControl
    {
        public static PlanCanvasView Current { get; set; }
        private bool IsActiveElement;
        private bool _isDown;
        private bool _isResize;
        private bool _isDragging;
        private Point _startPoint;
        private UIElement _originalElement;
        private PolygonAdorner _overlayElementPolygon;
        private double _originalTop;
        private double _originalLeft;


        public PlanCanvasView()
        {
            Current = this;
            InitializeComponent();
        }

        public void ChangeSelectedPlan(Plan plan)
        {
            MainCanvas.Children.Clear();
            foreach (var zona in plan.ElementZones)
            {
                Polygon myPolygon = new Polygon();
                myPolygon.ToolTip = "Зона №" + zona.ZoneNo;
                myPolygon.Stroke = System.Windows.Media.Brushes.Black;
                myPolygon.Fill = System.Windows.Media.Brushes.LightSeaGreen;
                myPolygon.StrokeThickness = 2;
                PointCollection myPointCollection = new PointCollection();
                var point = new Point();
                foreach (var _point in zona.PolygonPoints)
                {
                    point.X = _point.X;
                    point.Y = _point.Y;
                    myPointCollection.Add(point);
                }
                myPolygon.Points = myPointCollection;
                MainCanvas.Children.Add(myPolygon);
            }

        }

        private void ClearAllSelected()
        {
            foreach (var element in MainCanvas.Children)
            {
                if (element is Thumb)
                {
                    MainCanvas.Children.Remove((UIElement)element);
                    ClearAllSelected();
                    break;
                }
            }
        }

        private void DragStarted()
        {
            if (_originalElement != null)
            {
                _isDragging = true;

                _originalLeft = Canvas.GetLeft(_originalElement);
                _originalTop = Canvas.GetTop(_originalElement);
                _overlayElementPolygon = new PolygonAdorner(_originalElement);
                if (!_isResize) //перемещение
                {
                    _overlayElementPolygon.SetOperationMove(true);
                }
                AdornerLayer layer = AdornerLayer.GetAdornerLayer(_originalElement);
                layer.Add(_overlayElementPolygon);
            }
        }
        private void DragMoved()
        {
            if (_overlayElementPolygon != null)
            {
                Point CurrentPosition = Mouse.GetPosition(MainCanvas);
                _overlayElementPolygon.LeftOffset = CurrentPosition.X - _startPoint.X;
                _overlayElementPolygon.TopOffset = CurrentPosition.Y - _startPoint.Y;
            }

        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            var element = e.Source;
            if (_isDown && IsActiveElement)
            {
                /*
                if ((_isDragging == false) && ((Math.Abs(e.GetPosition(MainCanvas).X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance) ||
                    (Math.Abs(e.GetPosition(MainCanvas).Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)))
                 * */
                if (_isDragging == false)
                {
                    DragStarted();
                }
                if (_isDragging)
                {
                    DragMoved();
                }
            }
            else
            {
                if (e.Source != this.MainCanvas)
                {
                    MainCanvasViewModel caonvasViewModel = new MainCanvasViewModel((UIElement)e.Source);
                    IsActiveElement = true;
                }
                else
                {
                    IsActiveElement = false;
                    ClearAllSelected();
                }
            }
        }

        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (IsActiveElement)
            {
                _isDown = true;
                _isResize = false;
                _startPoint = e.GetPosition(this);
                _originalElement = e.Source as UIElement;
                e.Handled = true;
            }
        }
        public void DragFinished(bool cancelled)
        {
            if (_isDragging && _overlayElementPolygon != null)
            {
                AdornerLayer.GetAdornerLayer(_overlayElementPolygon.AdornedElement).Remove(_overlayElementPolygon);

                if (cancelled == false)
                {
                    Polygon polygon = _originalElement as Polygon;
                    PointCollection pointCollection = new PointCollection();
                    foreach (var _point in polygon.Points)
                    {
                        //polygon.Points.IndexOf(point)
                        Point point = _point;
                        point.X = point.X + _overlayElementPolygon.LeftOffset;
                        point.Y = point.Y + _overlayElementPolygon.TopOffset;
                        pointCollection.Add(point);

                    }
                    polygon.Points=pointCollection;
                    //Canvas.SetTop(_originalElement, _originalTop + _overlayElementPolygon.TopOffset);
                    //Canvas.SetLeft(_originalElement, _originalLeft + _overlayElementPolygon.LeftOffset);
                    //listShapes.DragFinished(_code, canvas, _originalTop + _overlayElementPolygon.TopOffset, _originalLeft + _overlayElementPolygon.LeftOffset);
                }
            }
            _isDragging = false;
            _isDown = false;

        }
        private void MainCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDown)
            {
                DragFinished(false);
                e.Handled = true;
                ClearAllSelected();
            }
        }
    }
}