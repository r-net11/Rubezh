using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Infrastructure;
using PlansModule.Events;
using FiresecAPI.Models;
using System.Collections.Generic;

namespace PlansModule.Designer
{
    public class MoveThumb : Thumb
    {
        DesignerItem DesignerItem
        {
            get { return DataContext as DesignerItem; }
        }

        DesignerCanvas DesignerCanvas
        {
            get { return VisualTreeHelper.GetParent(DesignerItem) as DesignerCanvas; }
        }

        public MoveThumb()
        {
            DragStarted += new DragStartedEventHandler(this.MoveThumb_DragStarted);
            DragDelta += new DragDeltaEventHandler(this.MoveThumb_DragDelta);
            DragCompleted += new DragCompletedEventHandler(MoveThumb_DragCompleted);
        }

        private void MoveThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            DesignerCanvas.BeginChange();
        }

        void MoveThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            DesignerCanvas.EndChange();
        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (DesignerItem.IsSelected)
            {
                double minLeft = double.MaxValue;
                double minTop = double.MaxValue;
                double maxRight = 0;
                double maxBottom = 0;

                foreach (DesignerItem item in DesignerCanvas.SelectedItems)
                {
                    minLeft = Math.Min(Canvas.GetLeft(item), minLeft);
                    minTop = Math.Min(Canvas.GetTop(item), minTop);
                    maxRight = Math.Max(Canvas.GetLeft(item) + item.Width, maxRight);
                    maxBottom = Math.Max(Canvas.GetTop(item) + item.Height, maxBottom);
                }

                double deltaHorizontal = Math.Max(-minLeft, e.HorizontalChange);
                double deltaVertical = Math.Max(-minTop, e.VerticalChange);

                double deltaRight = DesignerCanvas.Width - maxRight;
                double deltaBottom = DesignerCanvas.Height - maxBottom;

                if (deltaRight < 0)
                {
                    deltaHorizontal = Math.Min(0, e.HorizontalChange);
                }
                if (deltaBottom < 0)
                {
                    deltaVertical = Math.Min(0, e.VerticalChange);
                }

                foreach (DesignerItem item in DesignerCanvas.SelectedItems)
                {
                    Canvas.SetLeft(item, Canvas.GetLeft(item) + deltaHorizontal);
                    Canvas.SetTop(item, Canvas.GetTop(item) + deltaVertical);
                }

                DesignerCanvas.InvalidateMeasure();
                e.Handled = true;

                PlansModule.HasChanges = true;
            }
        }
    }
}
