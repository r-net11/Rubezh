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
            };
            if (ActiveElement is Thumb)
            {
                SetActiveThumb(ActiveElement as Thumb);
            }
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
                //Thumbs.Add(thumb);
                PlanCanvasView.Current.MainCanvas.Children.Add(thumb);
                index++;
            }
        }

        void SetActiveThumb(Thumb _thumb)
        {
            _thumb.Background = Brushes.Green;
        }
    }
}