using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Infrastructure;
using Infrustructure.Plans;
using Infrustructure.Plans.Designer;
using System.Windows;
using System.Windows.Input;

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
			Initialized += new EventHandler(MoveThumb_Initialized);
			DragStarted += new DragStartedEventHandler(this.MoveThumb_DragStarted);
			DragDelta += new DragDeltaEventHandler(this.MoveThumb_DragDelta);
			DragCompleted += new DragCompletedEventHandler(MoveThumb_DragCompleted);
		}

		void MoveThumb_Initialized(object sender, EventArgs e)
		{
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
					deltaHorizontal = Math.Min(0, e.HorizontalChange);
				if (deltaBottom < 0)
					deltaVertical = Math.Min(0, e.VerticalChange);
				Vector shift = new Vector(deltaHorizontal, deltaVertical);

				foreach (DesignerItem designerItem in DesignerCanvas.SelectedItems)
				{
					designerItem.Element.Position += shift;
					designerItem.SetLocation();
				}

				DesignerCanvas.InvalidateMeasure();
				e.Handled = true;

				ServiceFactory.SaveService.PlansChanged = true;
			}
		}

		protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
		{
			base.OnPreviewMouseDown(e);
		}
	}
}