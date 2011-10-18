using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace PlansModule.Designer
{
    public class ResizeThumb : Thumb
    {
        private DesignerItem designerItem;
        private DesignerCanvas designerCanvas;

        public ResizeThumb()
        {
            DragStarted += new DragStartedEventHandler(this.ResizeThumb_DragStarted);
            DragDelta += new DragDeltaEventHandler(this.ResizeThumb_DragDelta);
        }

        private void ResizeThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.designerItem = DataContext as DesignerItem;

            if (this.designerItem != null)
            {
                this.designerCanvas = VisualTreeHelper.GetParent(this.designerItem) as DesignerCanvas;
            }
        }

        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (this.designerItem != null && this.designerCanvas != null && this.designerItem.IsSelected)
            {
                double minLeft = double.MaxValue;
                double minTop = double.MaxValue;
                double minDeltaHorizontal = double.MaxValue;
                double minDeltaVertical = double.MaxValue;
                double dragDeltaVertical, dragDeltaHorizontal;

                foreach (DesignerItem designerItem in this.designerCanvas.SelectedItems)
                {
                    minLeft = Math.Min(Canvas.GetLeft(designerItem), minLeft);
                    minTop = Math.Min(Canvas.GetTop(designerItem), minTop);

                    minDeltaVertical = Math.Min(minDeltaVertical, designerItem.ActualHeight - designerItem.MinHeight);
                    minDeltaHorizontal = Math.Min(minDeltaHorizontal, designerItem.ActualWidth - designerItem.MinWidth);
                }

                foreach (DesignerItem designerItem in this.designerCanvas.SelectedItems)
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
            }
        }
    }
}
