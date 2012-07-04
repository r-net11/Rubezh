using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Infrastructure;

namespace PlansModule.Designer
{
	public class MoveThumb : Thumb
	{
		private bool _wasMoved;
		private DesignerItem DesignerItem
		{
			get { return DataContext as DesignerItem; }
		}
		private DesignerCanvas DesignerCanvas
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
			_wasMoved = false;
			DesignerCanvas.BeginChange();
		}
		private void MoveThumb_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			if (_wasMoved)
				DesignerCanvas.EndChange();
		}
		private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
		{
			if (DesignerItem.IsSelected)
			{
				_wasMoved = true;

				double minLeft = double.MaxValue;
				double minTop = double.MaxValue;
				double maxRight = 0;
				double maxBottom = 0;

				foreach (DesignerItem designerItem in DesignerCanvas.SelectedItems)
				{
					minLeft = Math.Min(Canvas.GetLeft(designerItem), minLeft);
					minTop = Math.Min(Canvas.GetTop(designerItem), minTop);
					maxRight = Math.Max(Canvas.GetLeft(designerItem) + designerItem.ItemWidth, maxRight);
					maxBottom = Math.Max(Canvas.GetTop(designerItem) + designerItem.ItemHeight, maxBottom);
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

				foreach (DesignerItem designerItem in DesignerCanvas.SelectedItems)
				{
					double left = Canvas.GetLeft(designerItem) + deltaHorizontal;
					double top = Canvas.GetTop(designerItem) + deltaVertical;
					Canvas.SetLeft(designerItem, left);
					Canvas.SetTop(designerItem, top);
					//designerItem.ElementBase.Left = left;
					//designerItem.ElementBase.Top = top;
				}

				DesignerCanvas.InvalidateMeasure();
				e.Handled = true;

				ServiceFactory.SaveService.PlansChanged = true;
			}
		}
	}
}