using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using FiresecAPI.Models;
using Infrastructure.Common;
using PlansModule.Resize;
using PlansModule.Views;

namespace PlansModule.ViewModels
{
    public class MainCanvasViewModel : RegionViewModel
    {
        UIElement _activeElement;
        public static UIElement OriginalElementToResize;
        public static UIElement OriginalElementToResizeTextBox;
        public static Canvas Canvas;
        public static Plan Plan;

        public MainCanvasViewModel(UIElement element)
        {
            _activeElement = element;
            SetActive();
        }

        void SetActive()
        {
            if (_activeElement is Polygon)
            {
                SetActivePolygon(_activeElement as Polygon);
                OriginalElementToResize = _activeElement as Polygon;
                (_activeElement as Polygon).Cursor = Cursors.Cross;
            }
            else if (_activeElement is Rectangle)
            {
                SetActiveRectangle(_activeElement as Rectangle);
                OriginalElementToResize = _activeElement as Rectangle;
                (_activeElement as Rectangle).Cursor = Cursors.Cross;
            }
            else if (_activeElement is TextBox)
            {
                SetActiveTextBox(_activeElement as TextBox);
                OriginalElementToResizeTextBox = _activeElement as TextBox;
                (_activeElement as TextBox).Cursor = Cursors.Cross;
            }
            else if (_activeElement is Thumb)
            {
                SetActiveThumb(_activeElement as Thumb);
            }
        }

        void SetActiveTextBox(TextBox textbox)
        {
            Thumb thumb1 = new Thumb()
            {
                Height = 4,
                Width = 4,
                Background = Brushes.Blue,
                BorderBrush = Brushes.Blue,
                Name = "thumb1"
            };
            Canvas.SetLeft(thumb1, Canvas.GetLeft(textbox) - 2);
            Canvas.SetTop(thumb1, Canvas.GetTop(textbox) - 2);

            Thumb thumb2 = new Thumb()
            {
                Height = 4,
                Width = 4,
                Background = Brushes.Blue,
                BorderBrush = Brushes.Blue,
                Name = "thumb2"
            };
            Canvas.SetLeft(thumb2, Canvas.GetLeft(textbox) + textbox.ActualWidth - 2);
            Canvas.SetTop(thumb2, Canvas.GetTop(textbox) - 2);
            PlanCanvasView.Current.MainCanvas.Children.Add(thumb2);

            Thumb thumb3 = new Thumb()
            {
                Height = 4,
                Width = 4,
                Background = Brushes.Blue,
                BorderBrush = Brushes.Blue,
                Name = "thumb3"
            };
            Canvas.SetLeft(thumb3, Canvas.GetLeft(textbox) + textbox.ActualWidth - 2);
            Canvas.SetTop(thumb3, Canvas.GetTop(textbox) + textbox.ActualHeight - 2);
            PlanCanvasView.Current.MainCanvas.Children.Add(thumb3);

            Thumb thumb4 = new Thumb()
            {
                Height = 4,
                Width = 4,
                Background = Brushes.Blue,
                BorderBrush = Brushes.Blue,
                Name = "thumb4"
            };
            Canvas.SetLeft(thumb4, Canvas.GetLeft(textbox) - 2);
            Canvas.SetTop(thumb4, Canvas.GetTop(textbox) + textbox.ActualHeight - 2);
            PlanCanvasView.Current.MainCanvas.Children.Add(thumb4);
        }

        RectangleAdornerResize _overlayElementRectangle;
        void DragStarted(object sender, DragStartedEventArgs e)
        {
        }

        void RemoveAllWrapperForTexBox(UIElementCollection UIElementCollection)
        {
            foreach (UIElement element in Canvas.Children)
            {
                if (element is Rectangle)
                {
                    if ((element as Rectangle).Name == "textbox")
                    {
                        Canvas.Children.Remove(element);
                        RemoveAllWrapperForTexBox(Canvas.Children);
                        break;
                    }
                }
            }
        }

        void DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (OriginalElementToResize is Polygon)
            {
                (OriginalElementToResize as Polygon).Points = PolygonResizer.GetPolygonResize();
                PolygonResizer.RemovePolygon();
                PlanCanvasView.UpdateResizePlan(OriginalElementToResize, Plan);
                OriginalElementToResize = null;
                _activeElement = null;
            };

            RemoveAllWrapperForTexBox(Canvas.Children);
            MainCanvasViewModel.OriginalElementToResizeTextBox = null;
        }

        void DragDelta(object sender, DragDeltaEventArgs e)
        {
            Thumb thumb = e.Source as Thumb;

            double dX = Canvas.GetLeft(thumb) + e.HorizontalChange;
            double dY = Canvas.GetTop(thumb) + e.VerticalChange;

            Canvas.SetLeft(thumb, dX);
            Canvas.SetTop(thumb, dY);
            double test = Canvas.GetLeft(OriginalElementToResize);
            PolygonResizer.SetElementResize(OriginalElementToResize, thumb, OriginalElementToResizeTextBox);
            if (OriginalElementToResize is Rectangle)
            {
                string name = thumb.Name;
                switch (name)
                {
                    case "thumb1":
                        {
                            if (OriginalElementToResizeTextBox == null)
                            {
                                Rectangle rectangle = OriginalElementToResize as Rectangle;
                                double oldTop = Canvas.GetTop(rectangle);
                                double oldLeft = Canvas.GetLeft(rectangle);

                                if (dX < oldLeft)
                                {
                                    if (rectangle.Width + Math.Abs(dX - oldLeft) > rectangle.MinWidth)
                                    {
                                        Canvas.SetLeft(rectangle, dX);
                                        rectangle.Width = rectangle.Width + Math.Abs(dX - oldLeft);
                                    }
                                }
                                else
                                {
                                    if (rectangle.Width - Math.Abs(dX - oldLeft) > rectangle.MinWidth)
                                    {
                                        Canvas.SetLeft(rectangle, dX);
                                        rectangle.Width = rectangle.Width - Math.Abs(dX - oldLeft);
                                    }
                                }

                                if (dY < oldTop)
                                {
                                    if ((rectangle.Height + Math.Abs(dY - oldTop)) > rectangle.MinHeight)
                                    {
                                        rectangle.Height = rectangle.Height + Math.Abs(dY - oldTop);
                                        Canvas.SetTop(rectangle, dY);
                                    }
                                }
                                else
                                {
                                    if ((rectangle.Height - Math.Abs(dY - oldTop)) > rectangle.MinHeight)
                                    {
                                        rectangle.Height = rectangle.Height - Math.Abs(dY - oldTop);
                                        Canvas.SetTop(rectangle, dY);
                                    }
                                }
                            }
                            else
                            {
                                var textBox = OriginalElementToResizeTextBox as TextBox;
                                double oldTop = Canvas.GetTop(textBox);
                                double oldLeft = Canvas.GetLeft(textBox);
                                if (dX < oldLeft)
                                {
                                    if (textBox.FontSize + Math.Abs(e.HorizontalChange) > 0)
                                    {
                                        textBox.FontSize = textBox.FontSize + Math.Abs(e.HorizontalChange);
                                        Canvas.SetLeft(textBox, dX);
                                    }
                                }
                                else
                                {
                                    if (textBox.FontSize - Math.Abs(e.HorizontalChange) > 0)
                                    {
                                        textBox.FontSize = textBox.FontSize - Math.Abs(e.HorizontalChange);
                                        Canvas.SetLeft(textBox, dX);
                                    }
                                }
                                if (dY < oldTop)
                                {
                                    if (textBox.FontSize + Math.Abs(e.VerticalChange) > 0)
                                    {
                                        textBox.FontSize = textBox.FontSize + Math.Abs(e.VerticalChange);
                                        Canvas.SetTop(textBox, dY);
                                    }
                                }
                                else
                                {
                                    if (textBox.FontSize - Math.Abs(e.VerticalChange) > 0)
                                    {
                                        textBox.FontSize = textBox.FontSize - Math.Abs(e.VerticalChange);
                                        Canvas.SetTop(textBox, dY);
                                    }
                                }
                            }
                            break;
                        };
                    case "thumb2":
                        {
                            if (OriginalElementToResizeTextBox == null)
                            {
                                Rectangle rectangle = OriginalElementToResize as Rectangle;
                                double oldTop = Canvas.GetTop(rectangle);
                                double oldLeft = Canvas.GetLeft(rectangle);

                                if (dX < oldLeft + rectangle.Width)
                                {
                                    if (rectangle.Width - Math.Abs(oldLeft + rectangle.Width - dX) > rectangle.MinWidth)
                                    {
                                        rectangle.Width = rectangle.Width - Math.Abs(oldLeft + rectangle.Width - dX);
                                    }
                                }
                                else
                                {
                                    if (rectangle.Width + Math.Abs(oldLeft + rectangle.Width - dX) > rectangle.MinWidth)
                                    {
                                        rectangle.Width = rectangle.Width + Math.Abs(oldLeft + rectangle.Width - dX);
                                    }
                                }

                                if (dY < oldTop)
                                {
                                    if ((rectangle.Height + Math.Abs(dY - oldTop)) > rectangle.MinHeight)
                                    {
                                        rectangle.Height = rectangle.Height + Math.Abs(dY - oldTop);
                                        Canvas.SetTop(rectangle, dY);
                                    }
                                }
                                else
                                {
                                    if ((rectangle.Height - Math.Abs(dY - oldTop)) > rectangle.MinHeight)
                                    {
                                        rectangle.Height = rectangle.Height - Math.Abs(dY - oldTop);
                                        Canvas.SetTop(rectangle, dY);
                                    }
                                }
                            }
                            else
                            {/*
                                Thumb.Background = Brushes.Blue;
                                Thumb.BorderBrush = Brushes.Blue;
                                dX = dX - e.HorizontalChange;
                                dY = dY - e.VerticalChange;
                                Canvas.SetLeft(Thumb, dX);
                                Canvas.SetTop(Thumb, dY);
*/
                            }
                            break;
                        };
                    case "thumb3":
                        {
                            if (OriginalElementToResizeTextBox == null)
                            {
                                Rectangle rectangle = OriginalElementToResize as Rectangle;
                                double oldTop = Canvas.GetTop(rectangle);
                                double oldLeft = Canvas.GetLeft(rectangle);

                                if (dX < oldLeft + rectangle.Width)
                                {
                                    if (rectangle.Width - Math.Abs(oldLeft + rectangle.Width - dX) > rectangle.MinWidth)
                                    {
                                        rectangle.Width = rectangle.Width - Math.Abs(oldLeft + rectangle.Width - dX);
                                    }
                                }
                                else
                                {
                                    if (rectangle.Width + Math.Abs(oldLeft + rectangle.Width - dX) > rectangle.MinWidth)
                                    {
                                        rectangle.Width = rectangle.Width + Math.Abs(oldLeft + rectangle.Width - dX);
                                    }
                                }

                                if (dY < oldTop + rectangle.Height)
                                {
                                    if ((rectangle.Height - Math.Abs(oldTop + rectangle.Height - dY)) > rectangle.MinHeight)
                                    {
                                        rectangle.Height = rectangle.Height - Math.Abs(oldTop + rectangle.Height - dY);
                                    }
                                }
                                else
                                {
                                    if ((rectangle.Height + Math.Abs(oldTop + rectangle.Height - dY)) > rectangle.MinHeight)
                                    {
                                        rectangle.Height = rectangle.Height + Math.Abs(oldTop + rectangle.Height - dY);
                                    }
                                }
                            }
                            else
                            {
                                var textBox = OriginalElementToResizeTextBox as TextBox;
                                double oldTop = Canvas.GetTop(textBox);
                                double oldLeft = Canvas.GetLeft(textBox);
                                if (dX < oldLeft + textBox.Width)
                                {
                                    if (textBox.FontSize - Math.Abs(e.HorizontalChange) > 0)
                                    {
                                        textBox.FontSize = textBox.FontSize - e.HorizontalChange;
                                    }
                                }
                                else
                                {
                                    if (textBox.FontSize + e.HorizontalChange > 0)
                                    {
                                        textBox.FontSize = textBox.FontSize + e.HorizontalChange;
                                    }
                                }
                                if (dY < oldTop + textBox.Height)
                                {
                                    if (textBox.FontSize - e.VerticalChange > 0)
                                    {
                                        textBox.FontSize = textBox.FontSize - e.VerticalChange;
                                    }
                                }
                                else
                                {
                                    if (textBox.FontSize + e.VerticalChange > 0)
                                    {
                                        textBox.FontSize = textBox.FontSize + e.VerticalChange;
                                    }
                                }
                            }
                            break;
                        };
                    case "thumb4":
                        {
                            if (OriginalElementToResizeTextBox == null)
                            {
                                Rectangle rectangle = OriginalElementToResize as Rectangle;
                                double oldTop = Canvas.GetTop(rectangle);
                                double oldLeft = Canvas.GetLeft(rectangle);

                                if (dX < oldLeft)
                                {
                                    if (rectangle.Width + Math.Abs(dX - oldLeft) > rectangle.MinWidth)
                                    {
                                        Canvas.SetLeft(rectangle, dX);
                                        rectangle.Width = rectangle.Width + Math.Abs(dX - oldLeft);
                                    }
                                }
                                else
                                {
                                    if (rectangle.Width - Math.Abs(dX - oldLeft) > rectangle.MinWidth)
                                    {
                                        Canvas.SetLeft(rectangle, dX);
                                        rectangle.Width = rectangle.Width - Math.Abs(dX - oldLeft);
                                    }
                                }

                                if (dY < oldTop + rectangle.Height)
                                {
                                    if ((rectangle.Height - Math.Abs(oldTop + rectangle.Height - dY)) > rectangle.MinHeight)
                                    {
                                        rectangle.Height = rectangle.Height - Math.Abs(oldTop + rectangle.Height - dY);
                                    }
                                }
                                else
                                {
                                    if ((rectangle.Height + Math.Abs(oldTop + rectangle.Height - dY)) > rectangle.MinHeight)
                                    {
                                        rectangle.Height = rectangle.Height + Math.Abs(oldTop + rectangle.Height - dY);
                                    }
                                }
                            }
                            else
                            {/*
                                Thumb.Background = Brushes.Blue;
                                Thumb.BorderBrush = Brushes.Blue;
                                dX = dX - e.HorizontalChange;
                                dY = dY - e.VerticalChange;
                                Canvas.SetLeft(Thumb, dX);
                                Canvas.SetTop(Thumb, dY);
                                */
                            }
                            break;
                        };
                }
            }
            if (OriginalElementToResizeTextBox == null)
            {
                PlanCanvasView.UpdateResizePlan(OriginalElementToResize, Plan);
            }
            else
            {
                PlanCanvasView.UpdateResizePlan(OriginalElementToResizeTextBox, Plan);
                //OriginalElementToResizeTextBox = null;
            }
        }

        void SetActiveRectangle(Rectangle _rectangle)
        {
            var rectangle = _rectangle;
            string name = rectangle.Name;
            Thickness Thickness = new System.Windows.Thickness();
            Thickness.Bottom = 1;
            Thickness.Top = 1;
            Thickness.Left = 1;
            Thickness.Right = 1;
            Thumb thumb1 = new Thumb();
            thumb1.DragStarted += new DragStartedEventHandler(DragStarted);
            thumb1.DragDelta += new DragDeltaEventHandler(DragDelta);
            thumb1.DragCompleted += new DragCompletedEventHandler(DragCompleted);

            thumb1.Height = 4;
            thumb1.Width = 4;
            thumb1.Background = Brushes.Blue;
            thumb1.BorderBrush = Brushes.Blue;
            thumb1.Name = "thumb1";
            Canvas.SetLeft(thumb1, Canvas.GetLeft(_rectangle) - 2);
            Canvas.SetTop(thumb1, Canvas.GetTop(_rectangle) - 2);
            //PlanCanvasView.Current.MainCanvas.Children.Add(thumb1);
            PlanCanvasView.Current.MainCanvas.Children.Add(thumb1);
            Thumb thumb2 = new Thumb();
            thumb2.DragStarted += new DragStartedEventHandler(DragStarted);
            thumb2.DragDelta += new DragDeltaEventHandler(DragDelta);
            thumb2.DragCompleted += new DragCompletedEventHandler(DragCompleted);
            thumb2.Height = 4;
            thumb2.Width = 4;
            thumb2.Background = Brushes.Blue;
            thumb2.BorderBrush = Brushes.Blue;
            thumb2.Name = "thumb2";

            Canvas.SetLeft(thumb2, Canvas.GetLeft(_rectangle) + _rectangle.Width - 2);
            Canvas.SetTop(thumb2, Canvas.GetTop(_rectangle) - 2);

            PlanCanvasView.Current.MainCanvas.Children.Add(thumb2);

            Thumb thumb3 = new Thumb();
            thumb3.DragStarted += new DragStartedEventHandler(DragStarted);
            thumb3.DragDelta += new DragDeltaEventHandler(DragDelta);
            thumb3.DragCompleted += new DragCompletedEventHandler(DragCompleted);
            thumb3.Height = 4;
            thumb3.Width = 4;
            thumb3.Background = Brushes.Blue;
            thumb3.BorderBrush = Brushes.Blue;
            thumb3.Name = "thumb3";
            Canvas.SetLeft(thumb3, Canvas.GetLeft(_rectangle) + _rectangle.Width - 2);
            Canvas.SetTop(thumb3, Canvas.GetTop(_rectangle) + _rectangle.Height - 2);

            PlanCanvasView.Current.MainCanvas.Children.Add(thumb3);
            Thumb thumb4 = new Thumb();
            thumb4.DragStarted += new DragStartedEventHandler(DragStarted);
            thumb4.DragDelta += new DragDeltaEventHandler(DragDelta);
            thumb4.DragCompleted += new DragCompletedEventHandler(DragCompleted);
            thumb4.Height = 4;
            thumb4.Width = 4;
            thumb4.Background = Brushes.Blue;
            thumb4.BorderBrush = Brushes.Blue;
            thumb4.Name = "thumb4";
            Canvas.SetLeft(thumb4, Canvas.GetLeft(_rectangle) - 2);
            Canvas.SetTop(thumb4, Canvas.GetTop(_rectangle) + rectangle.Height - 2);

            PlanCanvasView.Current.MainCanvas.Children.Add(thumb4);
        }

        void SetActivePolygon(Polygon _polygon)
        {
            var polygon = _polygon;
            int index = 0;
            double dLeft = Canvas.GetLeft(_polygon);
            double dTop = Canvas.GetTop(_polygon);
            foreach (var point in _polygon.Points)
            {
                Thumb thumb = new Thumb();
                thumb.Height = 4;
                thumb.Width = 4;
                thumb.Background = Brushes.Blue;
                thumb.BorderBrush = Brushes.Blue;
                thumb.DragStarted += new DragStartedEventHandler(DragStarted);
                thumb.DragDelta += new DragDeltaEventHandler(DragDelta);
                thumb.DragCompleted += new DragCompletedEventHandler(DragCompleted);

                Canvas.SetLeft(thumb, point.X - 2 + dLeft);
                Canvas.SetTop(thumb, point.Y - 2 + dTop);
                string s = "Thumb" + (index).ToString();
                try
                {
                    thumb.Name = s;
                }
                catch (Exception ex)
                {
                    DialogBox.DialogBox.Show(ex.Message);
                }

                PlanCanvasView.Current.MainCanvas.Children.Add(thumb);
                index++;
            }
        }

        void SetActiveThumb(Thumb thumb)
        {
            thumb.Background = Brushes.Red;
            thumb.BorderBrush = Brushes.Red;
            Thickness Thickness = new System.Windows.Thickness()
            {
                Bottom = 2,
                Top = 2,
                Left = 2,
                Right = 2
            };
            thumb.BorderThickness = Thickness;
            thumb.Cursor = Cursors.ScrollAll;
        }
    }
}