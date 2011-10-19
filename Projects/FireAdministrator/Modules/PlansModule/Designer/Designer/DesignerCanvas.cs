using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using FiresecAPI.Models;
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

            var plan = FiresecManager.PlansConfiguration.Plans[0];
            var elementBase = designerItemData.PlansElement as ElementBase;
            Point position = e.GetPosition(this);
            elementBase.Left = Math.Max(0, position.X - elementBase.Width / 2);
            elementBase.Top = Math.Max(0, position.Y - elementBase.Height / 2);

            if (designerItemData.PlansElement is ElementRectangle)
            {
                plan.ElementRectangles.Add(elementBase as ElementRectangle);
            }
            if (designerItemData.PlansElement is ElementEllipse)
            {
                plan.ElementEllipses.Add(elementBase as ElementEllipse);
            }
            if (designerItemData.PlansElement is ElementPolygon)
            {
                plan.ElementPolygons.Add(elementBase as ElementPolygon);
            }
            if (designerItemData.PlansElement is ElementTextBlock)
            {
                plan.ElementTextBlocks.Add(elementBase as ElementTextBlock);
            }

            var frameworkElement = elementBase.Draw();
            var designerItem = new DesignerItem()
            {
                MinWidth = 10,
                MinHeight = 10,
                Width = elementBase.Width,
                Height = elementBase.Height,
                Content = frameworkElement,
                ElementBase = elementBase,
                IsPolygon = elementBase is ElementPolygon
            };
            frameworkElement.IsHitTestVisible = false;
            DesignerCanvas.SetLeft(designerItem, elementBase.Left);
            DesignerCanvas.SetTop(designerItem, elementBase.Top);
            this.Children.Add(designerItem);

            this.DeselectAll();
            designerItem.IsSelected = true;

            e.Handled = true;
        }
    }
}
