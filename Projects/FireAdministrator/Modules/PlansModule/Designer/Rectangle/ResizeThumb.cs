using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Collections.Generic;
using FiresecAPI.Models;
using Infrastructure;
using PlansModule.Events;

namespace PlansModule.Designer
{
    public class ResizeThumb : Thumb
    {
        DesignerItem DesignerItem
        {
            get { return DataContext as DesignerItem; }
        }

        DesignerCanvas DesignerCanvas
        {
            get { return DesignerItem.DesignerCanvas; }
        }

        public ResizeThumb()
        {
            DragStarted += new DragStartedEventHandler(this.ResizeThumb_DragStarted);
            DragDelta += new DragDeltaEventHandler(this.ResizeThumb_DragDelta);
            DragCompleted += new DragCompletedEventHandler(ResizeThumb_DragCompleted);
        }

        private void ResizeThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            DesignerCanvas.BeginChange();
        }

        void ResizeThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            DesignerCanvas.EndChange();
        }

        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (DesignerItem.IsSelected)
            {
                double minLeft = double.MaxValue;
                double minTop = double.MaxValue;
                double minDeltaHorizontal = double.MaxValue;
                double minDeltaVertical = double.MaxValue;
                double dragDeltaVertical, dragDeltaHorizontal;

                foreach (DesignerItem designerItem in DesignerCanvas.SelectedItems)
                {
                    minLeft = Math.Min(Canvas.GetLeft(designerItem), minLeft);
                    minTop = Math.Min(Canvas.GetTop(designerItem), minTop);

                    minDeltaVertical = Math.Min(minDeltaVertical, designerItem.ActualHeight - designerItem.MinHeight);
                    minDeltaHorizontal = Math.Min(minDeltaHorizontal, designerItem.ActualWidth - designerItem.MinWidth);
                }

                foreach (DesignerItem designerItem in DesignerCanvas.SelectedItems)
                {
                    if (designerItem.IsPolygon)
                        continue;

                    switch (VerticalAlignment)
                    {
                        case VerticalAlignment.Bottom:
                            dragDeltaVertical = Math.Min(-e.VerticalChange, minDeltaVertical);
                            designerItem.Height = designerItem.ActualHeight - dragDeltaVertical;
                            break;
                        case VerticalAlignment.Top:
                            dragDeltaVertical = Math.Min(Math.Max(-minTop, e.VerticalChange), minDeltaVertical);
                            Canvas.SetTop(designerItem, Canvas.GetTop(designerItem) + dragDeltaVertical);
                            designerItem.Height = designerItem.ActualHeight - dragDeltaVertical;
                            break;
                    }

                    switch (HorizontalAlignment)
                    {
                        case HorizontalAlignment.Left:
                            dragDeltaHorizontal = Math.Min(Math.Max(-minLeft, e.HorizontalChange), minDeltaHorizontal);
                            Canvas.SetLeft(designerItem, Canvas.GetLeft(designerItem) + dragDeltaHorizontal);
                            designerItem.Width = designerItem.ActualWidth - dragDeltaHorizontal;
                            break;
                        case HorizontalAlignment.Right:
                            dragDeltaHorizontal = Math.Min(-e.HorizontalChange, minDeltaHorizontal);
                            designerItem.Width = designerItem.ActualWidth - dragDeltaHorizontal;
                            break;
                    }
                }

                e.Handled = true;
                PlansModule.HasChanges = true;
            }
        }
    }
}
