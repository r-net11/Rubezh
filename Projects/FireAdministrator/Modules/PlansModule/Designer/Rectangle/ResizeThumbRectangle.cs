using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Infrastructure;
using Infrustructure.Plans;
using Infrustructure.Plans.Designer;

namespace PlansModule.Designer
{
    public class ResizeThumbRectangle : Thumb
    {
        public DesignerItem DesignerItem
        {
            get { return DataContext as DesignerItem; }
        }

		CommonDesignerCanvas DesignerCanvas
        {
            get { return DesignerItem.DesignerCanvas; }
        }

        public void InitializeDragEvents()
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

                foreach (var designerItem in DesignerCanvas.SelectedItems)
                {
					//if ((designerItem.IsDevice) || (designerItem.IsPolygon))
					//    continue;

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
                ServiceFactory.SaveService.PlansChanged = true;
            }
        }
    }
}