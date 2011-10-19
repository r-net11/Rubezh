using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FiresecAPI.Models;

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
            ElementBase plansElement = null;

            switch (itemType)
            {
                case "ElementRectangle":
                    plansElement = new ElementRectangle();
                    break;

                case "ElementEllipse":
                    plansElement = new ElementEllipse();
                    break;

                case "ElementPolygon":
                    plansElement = new ElementPolygon();
                    break;

                case "ElementTextBlock":
                    plansElement = new ElementTextBlock();
                    break;
            }

            var designerItemData = new DesignerItemData()
            {
                PlansElement = plansElement
            };

            return designerItemData;
        }
    }
}
