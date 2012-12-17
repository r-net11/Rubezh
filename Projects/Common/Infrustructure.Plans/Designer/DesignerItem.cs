using System.Windows.Input;
using Infrustructure.Plans.Elements;
using System.Windows.Media;
using System.Windows;

namespace Infrustructure.Plans.Designer
{
	public abstract class DesignerItem : CommonDesignerItem
	{
		public CommonDesignerCanvas DesignerCanvas { get; internal set; }
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
		public virtual void UpdateAdornerLayout()
		{
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