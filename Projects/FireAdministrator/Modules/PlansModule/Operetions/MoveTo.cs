using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using PlansModule.ViewModels;
using System.Windows.Shapes;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

namespace PlansModule.Operations
{
    public static class MoveTo
    {
        private static int idElement;
        static string typeElement;

        static bool ActiveElement;
        static string UpdateElement;
        static bool _isDown;
        static bool _isResize;
        static bool _isDragging;
        static Point _startPoint;
        static UIElement _originalElement;
        static TextBox _originalElementTextBox;
        static PolygonAdorner _overlayElementPolygon;
        static RectangleAdorner _overlayElementRectangle;
        static TextBoxAdorner _overlayElementTexBox;
        static double _originalTop;
        static double _originalLeft;
        public static double dTop = 30;
        public static Canvas MainCanvas;

        public static void DragStarted(Canvas canvas)
        {
            MainCanvas = canvas;
            if (_originalElement != null)
            {
                _isDragging = true;

                _originalLeft = Canvas.GetLeft(_originalElement);
                _originalTop = Canvas.GetTop(_originalElement);
                if (_originalElement is Polygon)
                {
                    GetTypeElement();
                    _overlayElementPolygon = new PolygonAdorner(_originalElement);
                    if (!_isResize) //перемещение
                    {
                        _overlayElementPolygon.SetOperationMove(true);
                    }
                    else
                    {
                        _overlayElementPolygon.SetOperationMove(false);
                    }
                    AdornerLayer layer = AdornerLayer.GetAdornerLayer(_originalElement);
                    layer.Add(_overlayElementPolygon);
                };
                if (_originalElement is Rectangle)
                {
                    GetTypeElement();
                    _overlayElementRectangle = new RectangleAdorner(_originalElement);
                    if (!_isResize) //перемещение
                    {
                        _overlayElementRectangle.SetOperationMove(true);
                    }
                    else
                    {
                        _overlayElementRectangle.SetOperationMove(false);
                    }
                    AdornerLayer layer = AdornerLayer.GetAdornerLayer(_originalElement);
                    layer.Add(_overlayElementRectangle);
                };
            }
        }

        static void GetTypeElement()
        {
            if (_originalElement is Polygon)
            {
                Polygon polygon = (_originalElement as Polygon);
                typeElement = polygon.Name.Substring(0, 4);
            }
            if (_originalElement is Rectangle)
            {
                Rectangle rectangle = (_originalElement as Rectangle);
                typeElement = rectangle.Name.Substring(0, 4);
            }
        }

        public static void DragMoved(Canvas MainCanvas, double ZoomValue)
        {
            if (_overlayElementPolygon != null)
            {
                _overlayElementPolygon.Cursor = Cursors.Cross;
                //Point CurrentPosition = Mouse.GetPosition(MainCanvas);
                Point CurrentPosition = Mouse.GetPosition(MainCanvas);
                _overlayElementPolygon.LeftOffset = (CurrentPosition.X - _startPoint.X) * ZoomValue;
                _overlayElementPolygon.TopOffset = (CurrentPosition.Y - _startPoint.Y) * ZoomValue;
            };
            if (_overlayElementRectangle != null)
            {
                _overlayElementRectangle.Cursor = Cursors.Cross;
                //Point CurrentPosition = Mouse.GetPosition(MainCanvas);
                Point CurrentPosition = Mouse.GetPosition(MainCanvas);

                _overlayElementRectangle.LeftOffset = (CurrentPosition.X - _startPoint.X) * ZoomValue;
                _overlayElementRectangle.TopOffset = (CurrentPosition.Y - _startPoint.Y) * ZoomValue;
            }
            if (_overlayElementTexBox != null)
            {
                _overlayElementTexBox.Cursor = Cursors.Cross;
                //Point CurrentPosition = Mouse.GetPosition(MainCanvas);
                Point CurrentPosition = Mouse.GetPosition(MainCanvas);
                _overlayElementTexBox.LeftOffset = (CurrentPosition.X - _startPoint.X) * ZoomValue;
                _overlayElementTexBox.TopOffset = (CurrentPosition.Y - _startPoint.Y) * ZoomValue;
            }


        }

        static void ClearAllSelected(Canvas MainCanvas)
        {
            //MainCanvas.Cursor = Cursors.Arrow;
            MainCanvas.Cursor = Cursors.Arrow;
            //foreach (var element in MainCanvas.Children)
            foreach (var element in MainCanvas.Children)
            {
                if (element is Thumb)
                {
                    //MainCanvas.Children.Remove((UIElement)element);
                    MainCanvas.Children.Remove((UIElement)element);
                    ClearAllSelected(MainCanvas);
                    break;
                }
            }
        }

        public static void ToMove(UIElement element)
        {              
            if (element != MainCanvas)
            {
                bool IsCanavs = false;
                if (element is Rectangle)
                {
                    var rect = element as Rectangle;
                    if (rect.Name == "canv0")
                    {
                        IsCanavs = true;
                        _originalElementTextBox = null;
                        ActiveElement = false;
                        ClearAllSelected(MainCanvas);

                    }
                    else IsCanavs = false;
                }
                if (!IsCanavs)
                {
                    if (element is TextBox)
                    {
                        TextBox textbox = element as TextBox;
                        _originalElementTextBox = textbox;
                        Rectangle rectangle = new Rectangle();
                        rectangle.Name = "textbox";
                        rectangle.Height = textbox.ActualHeight;
                        rectangle.Width = textbox.ActualWidth;
                        SolidColorBrush brush = new SolidColorBrush();
                        rectangle.Fill = brush;
                        Canvas.SetLeft(rectangle, Canvas.GetLeft(textbox));
                        Canvas.SetTop(rectangle, Canvas.GetTop(textbox));
                        //MainCanvas.Children.Add(rectangle);
                        MainCanvas.Children.Add(rectangle);
                    }
                    else
                    {
                        if (element is Rectangle)
                        {
                            MainCanvasViewModel canvasViewModel = new MainCanvasViewModel((UIElement)element);
                            ActiveElement = true;
                        }
                        else
                        {
                            MainCanvasViewModel canvasViewModel = new MainCanvasViewModel((UIElement)element);
                            ActiveElement = true;
                            _originalElementTextBox = null;
                        }
                    }
                }
                else
                {
                    _originalElementTextBox = null;
                    ActiveElement = false;
                    ClearAllSelected(MainCanvas);
                }

            }
            else
            {
                _originalElementTextBox = null;
                ActiveElement = false;
                ClearAllSelected(MainCanvas);
            }
        }

        
    }
}
