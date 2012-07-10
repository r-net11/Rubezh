
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
namespace Infrustructure.Plans.Designer
{
	public abstract class ResizeChrome : Control, INotifyPropertyChanged
	{
		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		protected DesignerItem DesignerItem { get; set; }
		public CommonDesignerCanvas DesignerCanvas
		{
			get { return DesignerItem.DesignerCanvas; }
		}

		public ResizeChrome()
		{
			AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(ResizeThumb_DragDelta));
		}

		public abstract void Initialize();
		protected abstract void Resize(ResizeDirection direction, double horizontalChange, double verticalChange);

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
						designerItem.ResizeChrome.Resize(thumb.Direction, e.HorizontalChange, e.VerticalChange);
				e.Handled = true;
			}
		}
	}
}
