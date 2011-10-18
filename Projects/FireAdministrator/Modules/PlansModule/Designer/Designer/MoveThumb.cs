using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace PlansModule.Designer
{
    public class MoveThumb : Thumb
    {
        private DesignerItem designerItem;
        private DesignerCanvas designerCanvas;

        public MoveThumb()
        {
            DragStarted += new DragStartedEventHandler(this.MoveThumb_DragStarted);
            DragDelta += new DragDeltaEventHandler(this.MoveThumb_DragDelta);
        }

        private void MoveThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.designerItem = DataContext as DesignerItem;

            if (this.designerItem != null)
            {
                this.designerCanvas = VisualTreeHelper.GetParent(this.designerItem) as DesignerCanvas;
            }
        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (this.designerItem != null && this.designerCanvas != null && this.designerItem.IsSelected)
            {
                double minLeft = double.MaxValue;
                double minTop = double.MaxValue;
                double maxRight = 0;
                double maxBottom = 0;

                foreach (DesignerItem item in this.designerCanvas.SelectedItems)
                {
                    minLeft = Math.Min(Canvas.GetLeft(item), minLeft);
                    minTop = Math.Min(Canvas.GetTop(item), minTop);
                    maxRight = Math.Max(Canvas.GetLeft(item) + item.Width, maxRight);
                    maxBottom = Math.Max(Canvas.GetTop(item) + item.Height, maxBottom);
                }

                double deltaHorizontal = Math.Max(-minLeft, e.HorizontalChange);
                double deltaVertical = Math.Max(-minTop, e.VerticalChange);

                double deltaRight = designerCanvas.Width - maxRight;
                double deltaBottom = designerCanvas.Height - maxBottom;

                if (deltaRight < 0)
                {
                    deltaHorizontal = Math.Min(0, e.HorizontalChange);
                }
                if (deltaBottom < 0)
                {
                    deltaVertical = Math.Min(0, e.VerticalChange);
                }

                foreach (DesignerItem item in this.designerCanvas.SelectedItems)
                {
                    Canvas.SetLeft(item, Canvas.GetLeft(item) + deltaHorizontal);
                    Canvas.SetTop(item, Canvas.GetTop(item) + deltaVertical);
                }

                this.designerCanvas.InvalidateMeasure();
                e.Handled = true;
            }
        }
    }
}
