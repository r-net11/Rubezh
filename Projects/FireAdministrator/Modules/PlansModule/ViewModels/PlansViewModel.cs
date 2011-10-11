using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DiagramDesigner;
using FiresecClient;
using Infrastructure.Common;
using System.Windows;

namespace PlansModule.ViewModels
{
    public class PlansViewModel : RegionViewModel
    {
        public PlansViewModel()
        {
            TestCommand = new RelayCommand(OnTest);

            DesignerCanvas = new DesignerCanvas();
            DesignerCanvas.AllowDrop = true;
            DesignerCanvas.Background = new SolidColorBrush(Colors.DarkGray);

            Initialize();
        }

        void Initialize()
        {
            var plan = FiresecManager.PlansConfiguration.Plans[0];

            DesignerCanvas.Width = plan.Width;
            DesignerCanvas.Height = plan.Height;

            foreach (var elementRectangle in plan.Rectangls)
            {
                var content = new Rectangle();
                content.Fill = new SolidColorBrush(Colors.Red);
                content.Stroke = new SolidColorBrush(Colors.Blue);
                content.IsHitTestVisible = false;

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

                var designerItem = new DesignerItem();
                designerItem.ItemType = "Rectangle";
                designerItem.IsPolygon = false;
                designerItem.MinWidth = 20;
                designerItem.MinHeight = 20;
                designerItem.Width = elementRectangle.Width;
                designerItem.Height = elementRectangle.Height;
                designerItem.Content = content;
                DesignerCanvas.SetLeft(designerItem, elementRectangle.Left);
                DesignerCanvas.SetTop(designerItem, elementRectangle.Top);
                DesignerCanvas.Children.Add(designerItem);
            }

            foreach (var elementTextBlock in plan.TextBoxes)
            {
                var content = new TextBlock();
                content.Text = elementTextBlock.Text;
                //elementTextBlock.BorderColor;
                content.IsHitTestVisible = false;

                var designerItem = new DesignerItem();
                designerItem.ItemType = "TextBox";
                designerItem.IsPolygon = false;
                designerItem.MinWidth = 20;
                designerItem.MinHeight = 20;
                designerItem.Content = content;
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

                var content = new Polygon();
                content.Points = new PointCollection(pointCollection);
                content.Fill = new SolidColorBrush(Colors.Orange);
                content.Stroke = new SolidColorBrush(Colors.Green);
                content.IsHitTestVisible = false;

                var designerItem = new DesignerItem();
                designerItem.ItemType = "Polygon";
                designerItem.IsPolygon = true;
                designerItem.MinWidth = 20;
                designerItem.MinHeight = 20;
                designerItem.Width = maxLeft - minLeft;
                designerItem.Height = maxTop - minTop;
                designerItem.Content = content;
                DesignerCanvas.SetLeft(designerItem, minLeft);
                DesignerCanvas.SetTop(designerItem, minTop);
                DesignerCanvas.Children.Add(designerItem);
            }

            foreach (var elementDevice in plan.ElementDevices)
            {
                var content = new Rectangle();
                content.Fill = new SolidColorBrush(Colors.Gold);
                content.Stroke = new SolidColorBrush(Colors.Lime);
                content.IsHitTestVisible = false;

                var designerItem = new DesignerItem();
                designerItem.ItemType = "Rectangle";
                designerItem.IsPolygon = false;
                designerItem.MinWidth = 5;
                designerItem.MinHeight = 5;
                designerItem.Width = elementDevice.Width;
                designerItem.Height = elementDevice.Height;
                designerItem.Content = content;
                DesignerCanvas.SetLeft(designerItem, elementDevice.Left);
                DesignerCanvas.SetTop(designerItem, elementDevice.Top);
                DesignerCanvas.Children.Add(designerItem);
            }

            DesignerCanvas.DeselectAll();
        }

        DesignerCanvas _designerCanvas;
        public DesignerCanvas DesignerCanvas
        {
            get { return _designerCanvas; }
            set
            {
                _designerCanvas = value;
                OnPropertyChanged("DesignerCanvas");
            }
        }

        public RelayCommand TestCommand { get; private set; }
        void OnTest()
        {
            DesignerCanvas.Background = new SolidColorBrush(Colors.Blue);
        }
    }
}
