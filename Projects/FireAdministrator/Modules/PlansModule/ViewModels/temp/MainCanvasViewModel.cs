using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using PlansModule.Views;
using System.Windows.Controls.Primitives;
using System;
using System.Windows.Input;

namespace PlansModule.ViewModels
{
    public class MainCanvasViewModel : RegionViewModel
    {
        UIElement ActiveElement = null;

        public MainCanvasViewModel(UIElement _element)
        {
            ActiveElement = _element;
            SetActive();

        }

        void SetActive()
        {
            if (ActiveElement is Polygon)
            {
                SetActivePolygon(ActiveElement as Polygon);
                (ActiveElement as Polygon).Cursor = Cursors.Cross;
            };
            if (ActiveElement is Rectangle)
            {
                SetActiveRectangle(ActiveElement as Rectangle);
                (ActiveElement as Rectangle).Cursor = Cursors.Cross;
            }
            if (ActiveElement is TextBox)
            {
                SetActiveTextBox(ActiveElement as TextBox);
                (ActiveElement as TextBox).Cursor = Cursors.Cross;
            }

            if (ActiveElement is Thumb)
            {
                SetActiveThumb(ActiveElement as Thumb);
            }
        }

        void SetActiveTextBox(TextBox _textbox)
        {
            var textbox = _textbox;
            
            double d1 = Canvas.GetLeft(textbox);
            double d2 = Canvas.GetTop(textbox);
            double d3 = textbox.ActualHeight;
            double d4 = textbox.ActualWidth;
            
            Thickness Thickness = new System.Windows.Thickness();
            Thickness.Bottom = 1;
            Thickness.Top = 1;
            Thickness.Left = 1;
            Thickness.Right = 1;
            Thumb thumb1 = new Thumb();
            thumb1.Height = 4;
            thumb1.Width = 4;
            thumb1.Background = Brushes.Blue;
            thumb1.BorderBrush = Brushes.Blue;
            thumb1.Name = "thumb1";
            Canvas.SetLeft(thumb1, Canvas.GetLeft(textbox) - 2);
            Canvas.SetTop(thumb1, Canvas.GetTop(textbox) - 2);
            PlanCanvasView.Current.MainCanvas.Children.Add(thumb1);

            Thumb thumb2 = new Thumb();
            thumb2.Height = 4;
            thumb2.Width = 4;
            thumb2.Background = Brushes.Blue;
            thumb2.BorderBrush = Brushes.Blue;
            thumb2.Name = "thumb2";

            Canvas.SetLeft(thumb2, Canvas.GetLeft(textbox) + textbox.ActualWidth - 2);
            Canvas.SetTop(thumb2, Canvas.GetTop(textbox) - 2);
            PlanCanvasView.Current.MainCanvas.Children.Add(thumb2);

            Thumb thumb3 = new Thumb();
            thumb3.Height = 4;
            thumb3.Width = 4;
            thumb3.Background = Brushes.Blue;
            thumb3.BorderBrush = Brushes.Blue;
            thumb3.Name = "thumb3";
            Canvas.SetLeft(thumb3, Canvas.GetLeft(textbox) + textbox.ActualWidth - 2);
            Canvas.SetTop(thumb3, Canvas.GetTop(textbox) + textbox.ActualHeight - 2);
            PlanCanvasView.Current.MainCanvas.Children.Add(thumb3);
            Thumb thumb4 = new Thumb();
            thumb4.Height = 4;
            thumb4.Width = 4;
            thumb4.Background = Brushes.Blue;
            thumb4.BorderBrush = Brushes.Blue;
            thumb4.Name = "thumb4";
            Canvas.SetLeft(thumb4, Canvas.GetLeft(textbox) - 2);
            Canvas.SetTop(thumb4, Canvas.GetTop(textbox) + textbox.ActualHeight - 2);
            PlanCanvasView.Current.MainCanvas.Children.Add(thumb4);
            //textbox.IsHitTestVisible = false;
        }
        void SetActiveRectangle(Rectangle _rectangle)
        {
            var rectangle = _rectangle;
            Thickness Thickness = new System.Windows.Thickness();
            Thickness.Bottom = 1;
            Thickness.Top = 1;
            Thickness.Left = 1;
            Thickness.Right = 1;
            Thumb thumb1 = new Thumb();
            thumb1.Height = 4;
            thumb1.Width = 4;
            thumb1.Background = Brushes.Blue;
            thumb1.BorderBrush = Brushes.Blue;
            thumb1.Name = "thumb1";
            Canvas.SetLeft(thumb1, Canvas.GetLeft(_rectangle) - 2);
            Canvas.SetTop(thumb1, Canvas.GetTop(_rectangle) - 2);
            PlanCanvasView.Current.MainCanvas.Children.Add(thumb1);

            Thumb thumb2 = new Thumb();
            thumb2.Height = 4;
            thumb2.Width = 4;
            thumb2.Background = Brushes.Blue;
            thumb2.BorderBrush = Brushes.Blue;
            thumb2.Name = "thumb2";

            Canvas.SetLeft(thumb2, Canvas.GetLeft(_rectangle)+_rectangle.Width - 2);
            Canvas.SetTop(thumb2, Canvas.GetTop(_rectangle) - 2);
            PlanCanvasView.Current.MainCanvas.Children.Add(thumb2);

            Thumb thumb3 = new Thumb();
            thumb3.Height = 4;
            thumb3.Width = 4;
            thumb3.Background = Brushes.Blue;
            thumb3.BorderBrush = Brushes.Blue;
            thumb3.Name = "thumb3";
            Canvas.SetLeft(thumb3, Canvas.GetLeft(_rectangle)+_rectangle.Width - 2);
            Canvas.SetTop(thumb3, Canvas.GetTop(_rectangle)+_rectangle.Height - 2);
            PlanCanvasView.Current.MainCanvas.Children.Add(thumb3);
            Thumb thumb4 = new Thumb();
            thumb4.Height = 4;
            thumb4.Width = 4;
            thumb4.Background = Brushes.Blue;
            thumb4.BorderBrush = Brushes.Blue;
            thumb4.Name = "thumb4";
            Canvas.SetLeft(thumb4, Canvas.GetLeft(_rectangle) - 2);
            Canvas.SetTop(thumb4, Canvas.GetTop(_rectangle)+rectangle.Height - 2);
            PlanCanvasView.Current.MainCanvas.Children.Add(thumb4);

        }
        void SetActivePolygon(Polygon _polygon)
        {
            var polygon = _polygon;
            int index = 1;
            double dLeft = Canvas.GetLeft(_polygon);
            double dTop = Canvas.GetTop(_polygon);
            foreach (var point in _polygon.Points)
            {
                Thumb thumb = new Thumb();
                thumb.Height = 4;
                thumb.Width = 4;
                thumb.Background = Brushes.Blue;
                thumb.BorderBrush = Brushes.Blue;
                Thickness Thickness = new System.Windows.Thickness();
                Thickness.Bottom=1;
                Thickness.Top=1;
                Thickness.Left=1;
                Thickness.Right=1;
                thumb.BorderThickness = Thickness;
                Canvas.SetLeft(thumb, point.X - 2 + dLeft);
                Canvas.SetTop(thumb, point.Y - 2 + dTop);
                string s = "thumb" + (index + 1).ToString();
                try
                {
                    thumb.Name = s;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                PlanCanvasView.Current.MainCanvas.Children.Add(thumb);
                index++;
            }
        }

        void SetActiveThumb(Thumb _thumb)
        {
            _thumb.Background = Brushes.Red;
            _thumb.BorderBrush = Brushes.Red;
            Thickness Thickness = new System.Windows.Thickness();
            Thickness.Bottom = 2;
            Thickness.Top = 2;
            Thickness.Left = 2;
            Thickness.Right = 2;
            _thumb.BorderThickness = Thickness;
            _thumb.Cursor = Cursors.ScrollAll;
            
        }
    }
}