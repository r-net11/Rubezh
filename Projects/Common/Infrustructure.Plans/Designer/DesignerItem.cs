using System.Windows.Input;
using System.Windows.Media;
using Infrustructure.Plans.Elements;

namespace Infrustructure.Plans.Designer
{
	public abstract class DesignerItem : CommonDesignerItem
	{
		private CommonDesignerCanvas _designerCanvas;
		public CommonDesignerCanvas DesignerCanvas
		{
			get
			{
				if (_designerCanvas == null)
					_designerCanvas = VisualTreeHelper.GetParent(this) as CommonDesignerCanvas;
				return _designerCanvas;
			}
		}
		public ICommand ShowPropertiesCommand { get; protected set; }
		public ICommand DeleteCommand { get; protected set; }

		public ResizeChrome ResizeChrome { get; set; }
		public string Group { get; set; }

		public DesignerItem(ElementBase element)
			: base(element)
		{
			Group = string.Empty;
		}

		public override void ResetElement(ElementBase element)
		{
			base.ResetElement(element);
			if (DesignerCanvas != null)
				Redraw();
		}
		public void UpdateAdorner()
		{
			if (ResizeChrome != null)
				ResizeChrome.Initialize();
		}

		public virtual void UpdateZoom()
		{
			if (ResizeChrome != null)
				ResizeChrome.UpdateZoom();
		}
		public virtual void UpdateZoomPoint()
		{
		}
		public override void Redraw()
		{
			base.Redraw();
			UpdateAdorner();
		}

		protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
		{
			base.OnPreviewMouseDown(e);

			if (DesignerCanvas != null)
			{
				if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
					IsSelected = !IsSelected;
				else if (!IsSelected)
				{
					DesignerCanvas.DeselectAll();
					IsSelected = true;
				}
			}
			e.Handled = false;
		}

		protected abstract void OnShowProperties();
		protected abstract void OnDelete();
	}
}