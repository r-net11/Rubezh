using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml;
using ControlBase;

namespace DiagramDesigner
{
    public class DesignerCanvas : Canvas
    {
        private Point? dragStartPoint = null;

        public List<DesignerItem> SelectedItems
        {
            get
            {
                var selectedItems = from item in this.Children.OfType<DesignerItem>()
                                    where item.IsSelected == true
                                    select item;

                return selectedItems.ToList();
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
            Type type = e.Data.GetData("DESIGNER_ITEM") as Type;
            if (type != null)
            {
                DesignerItem designerItem = null;

                UserControlBase userControlBase = Activator.CreateInstance(type) as UserControlBase;
                userControlBase.IsHitTestVisible = false;

                if (userControlBase != null)
                {
                    userControlBase.Id = Data.IdManager.Next;

                    designerItem = new DesignerItem();
                    designerItem.Content = userControlBase;

                    Point position = e.GetPosition(this);
                    if (userControlBase.MinHeight != 0 && userControlBase.MinWidth != 0)
                    {
                        designerItem.Width = userControlBase.MinWidth * 2; ;
                        designerItem.Height = userControlBase.MinHeight * 2;
                    }
                    else
                    {
                        designerItem.Width = 65;
                        designerItem.Height = 65;
                    }
                    DesignerCanvas.SetLeft(designerItem, Math.Max(0, position.X - designerItem.Width / 2));
                    DesignerCanvas.SetTop(designerItem, Math.Max(0, position.Y - designerItem.Height / 2));
                    this.Children.Add(designerItem);

                    this.DeselectAll();
                    designerItem.IsSelected = true;
                }

                e.Handled = true;
            }
        }

        public List<UserControlBase> GetAllControls()
        {
            List<UserControlBase> AllControls = new List<UserControlBase>();
            foreach (UIElement element in this.Children)
            {
                if (element is DesignerItem)
                {
                    DesignerItem designerItem = element as DesignerItem;
                    if (designerItem.Content is UserControlBase)
                    {
                        UserControlBase userControlBase = designerItem.Content as UserControlBase;
                        AllControls.Add(userControlBase);
                    }
                }
            }
            return AllControls;
        }
    }
}
