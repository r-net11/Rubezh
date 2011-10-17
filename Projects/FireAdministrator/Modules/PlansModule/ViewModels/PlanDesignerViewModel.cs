using System;
using System.Linq;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DiagramDesigner;
using FiresecAPI.Models;
using Infrastructure.Common;
using DeviceControls;
using FiresecClient;

namespace PlansModule.ViewModels
{
    public class PlanDesignerViewModel : BaseViewModel
    {
        public DesignerCanvas DesignerCanvas;

        public void Initialize(Plan plan)
        {
            DesignerCanvas.Width = plan.Width;
            DesignerCanvas.Height = plan.Height;

            foreach (var elementRectangle in plan.Rectangls)
            {
                var content = new Rectangle()
                {
                    Fill = new SolidColorBrush(Colors.Red),
                    Stroke = new SolidColorBrush(Colors.Blue),
                    IsHitTestVisible = false
                };

                if (elementRectangle.BackgroundPixels != null)
                {
                    BitmapImage image = null;
                    using (var imageStream = new MemoryStream(elementRectangle.BackgroundPixels))
                    {
                        image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = imageStream;
                        image.EndInit();
                    }
                    content.Fill = new ImageBrush(image);
                }

                var designerItem = new DesignerItem()
                {
                    ItemType = "Rectangle",
                    IsPolygon = false,
                    MinWidth = 20,
                    MinHeight = 20,
                    Width = elementRectangle.Width,
                    Height = elementRectangle.Height,
                    Content = content
                };
                DesignerCanvas.SetLeft(designerItem, elementRectangle.Left);
                DesignerCanvas.SetTop(designerItem, elementRectangle.Top);
                DesignerCanvas.Children.Add(designerItem);
            }

            foreach (var elementTextBlock in plan.TextBoxes)
            {
                var content = new TextBlock()
                {
                    Text = elementTextBlock.Text,
                    IsHitTestVisible = false
                };
                //elementTextBlock.BorderColor;

                var designerItem = new DesignerItem()
                {
                    ItemType = "TextBox",
                    IsPolygon = false,
                    MinWidth = 20,
                    MinHeight = 20,
                    Content = content
                };
                DesignerCanvas.SetLeft(designerItem, elementTextBlock.Left);
                DesignerCanvas.SetTop(designerItem, elementTextBlock.Top);
                DesignerCanvas.Children.Add(designerItem);
            }

            foreach (var elementZonePolygon in plan.ElementZones)
            {
                double minLeft = double.MaxValue;
                double minTop = double.MaxValue;
                double maxLeft = 0;
                double maxTop = 0;

                foreach (var point in elementZonePolygon.PolygonPoints)
                {
                    minLeft = Math.Min(point.X, minLeft);
                    minTop = Math.Min(point.Y, minTop);
                    maxLeft = Math.Max(point.X, maxLeft);
                    maxTop = Math.Max(point.Y, maxTop);
                }

                var pointCollection = new PointCollection();
                foreach (var point in elementZonePolygon.PolygonPoints)
                {
                    pointCollection.Add(new Point(point.X - minLeft, point.Y - minTop));
                }

                var content = new Polygon()
                {
                    Points = new PointCollection(pointCollection),
                    Fill = new SolidColorBrush(Colors.Orange),
                    Stroke = new SolidColorBrush(Colors.Green),
                    IsHitTestVisible = false
                };

                var designerItem = new DesignerItem()
                {
                    ItemType = "Polygon",
                    IsPolygon = true,
                    MinWidth = 20,
                    MinHeight = 20,
                    Width = maxLeft - minLeft,
                    Height = maxTop - minTop,
                    Content = content,
                    Opacity = 0.5
                };
                DesignerCanvas.SetLeft(designerItem, minLeft);
                DesignerCanvas.SetTop(designerItem, minTop);
                DesignerCanvas.Children.Add(designerItem);
            }

            foreach (var elementDevice in plan.ElementDevices)
            {
                var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == elementDevice.Id);
                var content = new DeviceControl()
                {
                    DriverId = device.Driver.UID,
                    StateType = StateType.Norm,
                    IsHitTestVisible = false
                };

                var designerItem = new DesignerItem()
                {
                    ItemType = "Rectangle",
                    IsPolygon = false,
                    MinWidth = 5,
                    MinHeight = 5,
                    Width = elementDevice.Width,
                    Height = elementDevice.Height,
                    Content = content
                };
                DesignerCanvas.SetLeft(designerItem, elementDevice.Left);
                DesignerCanvas.SetTop(designerItem, elementDevice.Top);
                DesignerCanvas.Children.Add(designerItem);
            }

            DesignerCanvas.DeselectAll();
        }
    }
}
