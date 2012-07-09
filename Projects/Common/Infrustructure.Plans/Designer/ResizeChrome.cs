
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Shapes;
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

		public abstract void Initialize();

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
	}
}
