
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using Infrustructure.Plans.Elements;
namespace Infrustructure.Plans.Designer
{
	public abstract class ResizeChrome : Control, INotifyPropertyChanged
	{
		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		protected DesignerItem DesignerItem { get; private set; }
		public CommonDesignerCanvas DesignerCanvas
		{
			get { return DesignerItem.DesignerCanvas; }
		}

		public ResizeChrome(DesignerItem designerItem)
		{
			DesignerItem = designerItem;
			AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(ResizeThumb_DragDelta));
			Loaded += (s, e) => UpdateZoom();
		}

		public abstract void Initialize();
		protected abstract void Resize(ResizeDirection direction, Vector vector);

		public virtual void UpdateZoom()
		{
			OnPropertyChanged("ResizeThumbSize");
			OnPropertyChanged("ThumbMargin");
			OnPropertyChanged("ResizeBorderSize");
			OnPropertyChanged("ResizeMargin");
			OnPropertyChanged("Thickness");
		}

		public virtual double ResizeThumbSize { get { return 7 / DesignerCanvas.Zoom; } }
		public virtual double ThumbMargin { get { return -2 / DesignerCanvas.Zoom; } }
		public virtual double ResizeBorderSize { get { return 3 / DesignerCanvas.Zoom; } }
		public virtual double ResizeMargin { get { return -4 / DesignerCanvas.Zoom; } }
		public virtual double Thickness { get { return 1 / DesignerCanvas.Zoom; } }

		protected void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
		{
			if (DesignerItem.IsSelected)
			{
				ResizeThumb thumb = (ResizeThumb)e.OriginalSource;
				foreach (DesignerItem designerItem in DesignerCanvas.SelectedItems)
					if (designerItem.ResizeChrome != null)
						designerItem.ResizeChrome.Resize(thumb.Direction, CalculateSize(thumb.Direction, e.HorizontalChange, e.VerticalChange));
				e.Handled = true;
			}
		}
		private Vector CalculateSize(ResizeDirection direction, double horizontalChange, double verticalChange)
		{
			double dragDeltaHorizontal = horizontalChange;
			double dragDeltaVertical = verticalChange;
			foreach (DesignerItem designerItem in DesignerCanvas.SelectedItems)
			{
				Rect rect = new Rect(Canvas.GetLeft(designerItem), Canvas.GetTop(designerItem), designerItem.ActualWidth, designerItem.ActualHeight);
				if ((direction & ResizeDirection.Top) == ResizeDirection.Top)
				{
					if (rect.Height - dragDeltaVertical < DesignerItem.MinHeight)
						dragDeltaVertical = rect.Height - DesignerItem.MinHeight;
					if (rect.Top + dragDeltaVertical < 0)
						dragDeltaVertical = -rect.Top;
				}
				else if ((direction & ResizeDirection.Bottom) == ResizeDirection.Bottom)
				{
					if (rect.Height + dragDeltaVertical < DesignerItem.MinHeight)
						dragDeltaVertical = DesignerItem.MinHeight - rect.Height;
					if (rect.Bottom + dragDeltaVertical > DesignerCanvas.Height)
						dragDeltaVertical = DesignerCanvas.Height - rect.Bottom;
				}
				if ((direction & ResizeDirection.Left) == ResizeDirection.Left)
				{
					if (rect.Width - dragDeltaHorizontal < DesignerItem.MinWidth)
						dragDeltaHorizontal = rect.Width - DesignerItem.MinWidth;
					if (rect.Left + dragDeltaHorizontal < 0)
						dragDeltaHorizontal = -rect.Left;
				}
				else if ((direction & ResizeDirection.Right) == ResizeDirection.Right)
				{
					if (rect.Width + dragDeltaHorizontal < DesignerItem.MinWidth)
						dragDeltaHorizontal = DesignerItem.MinWidth - rect.Width;
					if (rect.Right + dragDeltaHorizontal > DesignerCanvas.Width)
						dragDeltaHorizontal = DesignerCanvas.Width - rect.Right;
				}
			}
			return new Vector(dragDeltaHorizontal, dragDeltaVertical);
		}
	}
}
