using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using FiresecAPI.Models.Plans;
using FiresecClient;

namespace PlansModule.Designer
{
    public class DesignerCanvas : Canvas
    {
        private Point? dragStartPoint = null;

        public IEnumerable<DesignerItem> SelectedItems
        {
            get
            {
                var selectedItems = from item in this.Children.OfType<DesignerItem>()
                                    where item.IsSelected == true
                                    select item;

                return selectedItems;
            }
        }

        public void DeselectAll()
        {
            foreach (DesignerItem item in this.SelectedItems)
            {
                item.IsSelected = false;
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Source == this)
            {
                this.dragStartPoint = new Point?(e.GetPosition(this));
                this.DeselectAll();
                e.Handled = true;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton != MouseButtonState.Pressed)
            {
                this.dragStartPoint = null;
            }

            if (this.dragStartPoint.HasValue)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if (adornerLayer != null)
                {
                    RubberbandAdorner adorner = new RubberbandAdorner(this, this.dragStartPoint);
                    if (adorner != null)
                    {
                        adornerLayer.Add(adorner);
                    }
                }

                e.Handled = true;
            }
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            DesignerItemData designerItemData = e.Data.GetData("DESIGNER_ITEM") as DesignerItemData;

            Point position = e.GetPosition(this);
            var designerItem = new DesignerItem()
            {
                ItemType = designerItemData.ItemType,
                IsPolygon = designerItemData.IsPolygon,
                MinWidth = designerItemData.MinWidth,
                MinHeight = designerItemData.MinHeight,
                Width = designerItemData.Width,
                Height = designerItemData.Height,
                Content = designerItemData.FrameworkElement,
                ConfigurationItem = null
            };
            DesignerCanvas.SetLeft(designerItem, Math.Max(0, position.X - designerItemData.Width / 2));
            DesignerCanvas.SetTop(designerItem, Math.Max(0, position.Y - designerItemData.Height / 2));
            this.Children.Add(designerItem);

            if (designerItemData.ItemType == "Ellipse")
            {
                ElementEllipse elementEllipse = new ElementEllipse();
                elementEllipse.Left = DesignerCanvas.GetLeft(designerItem);
                elementEllipse.Top = DesignerCanvas.GetTop(designerItem);
                elementEllipse.Width = designerItemData.Width;
                elementEllipse.Height = designerItemData.Height;

                FiresecManager.PlansConfiguration.Plans[0].ElementEllipses.Add(elementEllipse);

                designerItem.ConfigurationItem = elementEllipse;
            }

            this.DeselectAll();
            designerItem.IsSelected = true;

            e.Handled = true;
        }
    }
}
