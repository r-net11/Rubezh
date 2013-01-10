using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Painters;

namespace Infrustructure.Plans.Designer
{
	public abstract class CommonDesignerItem : DrawingVisual
	{
		public const int DefaultPointSize = 30;

		public event EventHandler ItemPropertyChanged;
		public event EventHandler TitleChanged;

		public CommonDesignerCanvas DesignerCanvas { get; internal set; }
		public ElementBase Element { get; protected set; }
		public IPainter Painter { get; protected set; }
		public bool IsMouseOver { get; private set; }
		public bool IsBusy { get; protected set; }
		public bool IsEnabled { get; protected set; }
		protected Rect OriginalRect { get; private set; }
		protected TranslateTransform TranslateTransform { get; private set; }
		protected ScaleTransform ScaleTransform { get; private set; }

		public event Action<CommonDesignerItem> UpdateProperties;

		private string _title;
		public string Title
		{
			get { return _title; }
			set
			{
				if (Title != value)
				{
					_title = value;
					if (TitleChanged != null)
						TitleChanged(this, EventArgs.Empty);
				}
			}
		}

		private bool _isVisibleLayout;
		public virtual bool IsVisibleLayout
		{
			get { return _isVisibleLayout; }
			set
			{
				if (_isVisibleLayout != value)
				{
					_isVisibleLayout = value;
					Opacity = IsVisibleLayout ? 1 : 0;
					ResetIsEnabled();
				}
			}
		}

		public CommonDesignerItem(ElementBase element)
		{
			ScaleTransform = new ScaleTransform();
			TranslateTransform = new TranslateTransform();
			Transform = new TransformGroup()
			{
				Children = new TransformCollection()
				{
					ScaleTransform,
					TranslateTransform
				}
			};
			IsBusy = false;
			ResetIsEnabled();
			ResetElement(element);
		}

		public virtual void UpdateZoom()
		{
		}
		public virtual void UpdateZoomPoint()
		{
			Translate();
		}

		public virtual void ResetElement(ElementBase element)
		{
			Element = element;
			Painter = PainterFactory.Create(Element);
		}
		public virtual void Redraw()
		{
			//MinWidth = Element.BorderThickness;
			//MinHeight = Element.BorderThickness;
			//if (Element is ElementBaseShape)
			//{
			//    MinWidth += 3;
			//    MinHeight += 3;
			//}
			using (DrawingContext drawingContext = RenderOpen())
			{
				OriginalRect = GetRectangle();
				Translate(true);
				Render(drawingContext);
			}
		}
		protected virtual void Render(DrawingContext drawingContext)
		{
			if (Painter != null)
				Painter.Draw(drawingContext, Element, GetRectangle());
		}
		public virtual void Translate(bool force = false)
		{
			var rect = GetRectangle();
			if (rect.Size != OriginalRect.Size || force)
			{
				ScaleTransform.CenterX = OriginalRect.Left;
				ScaleTransform.CenterY = OriginalRect.Top;
				ScaleTransform.ScaleX = rect.Width / OriginalRect.Width;
				ScaleTransform.ScaleY = rect.Height / OriginalRect.Height;
			}
			if (rect.TopLeft != OriginalRect.TopLeft || force)
			{
				TranslateTransform.X = rect.Left - OriginalRect.Left;
				TranslateTransform.Y = rect.Top - OriginalRect.Top;
			}
		}
		protected virtual Rect GetRectangle()
		{
			return Element.GetRectangle();
		}
		protected virtual void ResetIsEnabled()
		{
			IsEnabled = IsVisibleLayout;
		}

		public virtual void UpdateElementProperties()
		{
			OnUpdateProperties();
		}
		protected void OnUpdateProperties()
		{
			if (UpdateProperties != null)
				UpdateProperties(this);
		}

		public virtual ContextMenu GetContextMenu()
		{
			return null;
		}

		protected void OnDesignerItemPropertyChanged()
		{
			if (ItemPropertyChanged != null)
				ItemPropertyChanged(this, EventArgs.Empty);
		}

		internal void OnMouseDown(MouseButtonEventArgs e)
		{
			if (IsVisibleLayout)
				MouseDown(e);
		}
		internal void OnMouseUp(MouseButtonEventArgs e)
		{
			if (IsVisibleLayout)
				MouseUp(e);
		}
		internal void OnMouseMove(MouseEventArgs e)
		{
			if (IsVisibleLayout)
				MouseMove(e);
		}
		internal void OnMouseDoubleClick(MouseButtonEventArgs e)
		{
			if (IsVisibleLayout)
				MouseDoubleClick(e);
		}
		protected virtual void MouseDown(MouseButtonEventArgs e)
		{
			DesignerCanvas.SetTitle(null);
		}
		protected virtual void MouseUp(MouseButtonEventArgs e)
		{
		}
		protected virtual void MouseMove(MouseEventArgs e)
		{
		}
		protected virtual void MouseDoubleClick(MouseButtonEventArgs e)
		{
		}

		internal virtual void SetIsMouseOver(bool value)
		{
			IsMouseOver = value;
			DesignerCanvas.SetTitle(value ? Title : null);
		}
		internal virtual ContextMenu ContextMenuOpening()
		{
			DesignerCanvas.SetTitle(null);
			return GetContextMenu();
		}
	}
}