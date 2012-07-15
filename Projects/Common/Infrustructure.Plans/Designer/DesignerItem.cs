using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Infrustructure.Plans.Elements;
using System.Windows;
using System.Windows.Media;
using Infrustructure.Plans.Painters;
using System.Windows.Input;
using Infrustructure.Plans.Events;
using System.ComponentModel;

namespace Infrustructure.Plans.Designer
{
	public abstract class DesignerItem : ContentControl, INotifyPropertyChanged
	{
		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(DesignerItem), new FrameworkPropertyMetadata(false));
		public virtual bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			set
			{
				SetValue(IsSelectedProperty, value);
				if (value)
					EventService.EventAggregator.GetEvent<ElementSelectedEvent>().Publish(Element);
			}
		}

		public static readonly DependencyProperty IsSelectableProperty = DependencyProperty.Register("IsSelectable", typeof(bool), typeof(DesignerItem), new FrameworkPropertyMetadata(false));
		public virtual bool IsSelectable
		{
			get { return (bool)GetValue(IsSelectableProperty); }
			set { SetValue(IsSelectableProperty, value); }
		}

		private bool _isVisibleLayout;
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

		private bool _isSelectableLayout;
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
		public ICommand ShowPropertiesCommand { get; protected set; }
		public ICommand DeleteCommand { get; protected set; }

		public ElementBase Element { get; protected set; }
		public IPainter Painter { get; private set; }
		public ResizeChrome ResizeChrome { get; set; }

		private string _title;
		public string Title 
		{
			get { return _title; }
			set
			{
				_title = value;
				OnPropertyChanged("Title");
			}
		}
		public string Group { get; protected set; }

		public DesignerItem(ElementBase element)
		{
			ResetElement(element);
			Group = string.Empty;
		}

		public void ResetElement(ElementBase element)
		{
			Element = element;
			MinWidth = 2 * element.BorderThickness;
			MinHeight = 2 * element.BorderThickness;
			DataContext = Element;
			Painter = PainterFactory.Create(Element);
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

		public virtual void SetLocation()
		{
			var rect = Element.GetRectangle();
			try
			{
				Canvas.SetLeft(this, rect.Left);
				Canvas.SetTop(this, rect.Top);
				ItemWidth = rect.Width;
				ItemHeight = rect.Height;
			}
			catch { }
		}
		public void Redraw()
		{
			Content = Painter == null ? null : Painter.Draw(Element);
			SetLocation();
			UpdateAdorner();
		}

		public virtual double ItemWidth
		{
			get { return Width - Element.BorderThickness; }
			set { Width = value + Element.BorderThickness; }
		}
		public virtual double ItemHeight
		{
			get { return Height - Element.BorderThickness; }
			set { Height = value + Element.BorderThickness; }
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

		public virtual void UpdateElementProperties()
		{
		}

		protected abstract void OnShowProperties();
		protected abstract void OnDelete();

		protected void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}