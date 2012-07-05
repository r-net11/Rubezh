using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Infrustructure.Plans.Elements;
using System.Windows;
using System.Windows.Media;
using Infrustructure.Plans.Painters;

namespace Infrustructure.Plans.Designer
{
	public abstract class DesignerItem : ContentControl
	{
		public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(DesignerItem), new FrameworkPropertyMetadata(false));
		public virtual bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			set
			{
				SetValue(IsSelectedProperty, value);
				//if (value)
				//    ServiceFactory.Events.GetEvent<ElementSelectedEvent>().Publish(ElementBase.UID);
			}
		}

		public static readonly DependencyProperty IsSelectableProperty = DependencyProperty.Register("IsSelectable", typeof(bool), typeof(DesignerItem), new FrameworkPropertyMetadata(false));
		public virtual bool IsSelectable
		{
			get { return (bool)GetValue(IsSelectableProperty); }
			set { SetValue(IsSelectableProperty, value); }
		}

		bool _isVisibleLayout;
		public virtual bool IsVisibleLayout
		{
			get { return _isVisibleLayout; }
			set
			{
				_isVisibleLayout = value;
				Visibility = value ? Visibility.Visible : Visibility.Collapsed;
				if (!value)
					IsSelected = false;
			}
		}

		bool _isSelectableLayout;
		public virtual bool IsSelectableLayout
		{
			get { return _isSelectableLayout; }
			set
			{
				_isSelectableLayout = value;
				IsSelectable = value;
				if (!value)
					IsSelected = false;
			}
		}

		public CommonDesignerCanvas DesignerCanvas
		{
			get { return VisualTreeHelper.GetParent(this) as CommonDesignerCanvas; }
		}

		public ElementBase Element { get; protected set; }
		public IPainter Painter { get; private set; }
		public IResizeChromeBase ResizeChrome { get; set; }

		public DesignerItem(ElementBase element)
		{
			ResetElement(element);
		}

		public void ResetElement(ElementBase element)
		{
			Element = element;
			DataContext = Element;
			Painter = PainterFactory.Create(Element);
			Redraw();
		}
		public void UpdateAdorner()
		{
			if (ResizeChrome != null)
				ResizeChrome.Initialize();
		}

		public void UpdateZoom()
		{
			if (ResizeChrome != null)
				ResizeChrome.UpdateZoom();
		}
		public void UpdateZoomDevice()
		{
			//if (IsDevice)
			//{
			//    this.Width = DesignerCanvas.PlanDesignerViewModel.DeviceZoom;
			//    this.Height = DesignerCanvas.PlanDesignerViewModel.DeviceZoom;
			//    Canvas.SetLeft(this, ElementBase.Left - this.Width / 2);
			//    Canvas.SetTop(this, ElementBase.Top - this.Height / 2);
			//}
		}

		public void SetLocation(Rect rect)
		{
			Canvas.SetLeft(this, rect.Left);
			Canvas.SetTop(this, rect.Top);
			ItemWidth = rect.Width;
			ItemHeight = rect.Height;
		}
		public void Redraw()
		{
			Content = Painter == null ? null : Painter.Draw(Element);
			SetLocation(Element.GetRectangle());
			UpdateAdorner();
		}

		public abstract double ItemWidth { get; set; }
		public abstract double ItemHeight { get; set; }


		public abstract void Remove();
		public abstract void UpdateElementProperties();
	}
}
