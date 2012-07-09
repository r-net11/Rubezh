using System;
using System.Windows.Controls.Primitives;

namespace Infrustructure.Plans.Designer
{
	public class ResizeThumb : Thumb
	{
		public DesignerItem DesignerItem
		{
			get { return DataContext as DesignerItem; }
		}
		private CommonDesignerCanvas DesignerCanvas
		{
			get { return DesignerItem.DesignerCanvas; }
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
			DragStarted += new DragStartedEventHandler(this.ResizeThumb_DragStarted);
			DragCompleted += new DragCompletedEventHandler(ResizeThumb_DragCompleted);
		}

		private void ResizeThumb_DragStarted(object sender, DragStartedEventArgs e)
		{
			DesignerCanvas.BeginChange();
		}
		private void ResizeThumb_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			DesignerCanvas.EndChange();
		}
	}
}
