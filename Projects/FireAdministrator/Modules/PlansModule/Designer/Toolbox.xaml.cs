using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PlansModule.Designer
{
    public partial class Toolbox : UserControl
    {
        public Toolbox()
        {
            InitializeComponent();
        }

        private Point? dragStartPoint = null;

        private void Rectangle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            this.dragStartPoint = new Point?(e.GetPosition(this));
        }

        private void Rectangle_MouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                this.dragStartPoint = null;
            }

            if (this.dragStartPoint.HasValue)
            {
                var designerItemData = CreateDesignerItemData((sender as TextBlock).Name);

                DataObject dataObject = new DataObject("DESIGNER_ITEM", designerItemData);
                DragDrop.DoDragDrop(this, dataObject, DragDropEffects.Copy);
            }

            e.Handled = true;
        }

        DesignerItemData CreateDesignerItemData(string itemType)
        {
            var designerItemData = new DesignerItemData();
            designerItemData.ItemType = itemType;

            switch (itemType)
            {
                case "Rectangle":
                    System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle();
                    rectangle.Fill = new SolidColorBrush(Colors.LightGreen);
                    designerItemData.FrameworkElement = rectangle;
                    break;

                case "Ellipse":
                    System.Windows.Shapes.Ellipse ellipse = new Ellipse();
                    ellipse.Fill = new SolidColorBrush(Colors.LightGreen);
                    designerItemData.FrameworkElement = ellipse;
                    break;

                case "Polygon":
                    Polygon polygon = new Polygon();
                    polygon.Fill = new SolidColorBrush(Colors.Orange);
                    polygon.Stretch = Stretch.Fill;
                    polygon.Points = new PointCollection();
                    polygon.Points.Add(new Point(0, 0));
                    polygon.Points.Add(new Point(50, 0));
                    polygon.Points.Add(new Point(50, 50));
                    polygon.Points.Add(new Point(0, 50));
                    designerItemData.FrameworkElement = polygon;
                    designerItemData.IsPolygon = true;
                    break;

                case "TextBlock":
                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = "Text";
                    designerItemData.FrameworkElement = textBlock;
                    break;
            }

            designerItemData.Width = 50;
            designerItemData.Height = 50;
            designerItemData.MinWidth = 20;
            designerItemData.MinHeight = 20;
            designerItemData.FrameworkElement.IsHitTestVisible = false;

            return designerItemData;
        }
    }
}
