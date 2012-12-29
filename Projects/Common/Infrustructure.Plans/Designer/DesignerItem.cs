using System.Linq;
using System.Windows;
using System.Windows.Input;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;

namespace Infrustructure.Plans.Designer
{
	public abstract class DesignerItem : CommonDesignerItem
	{
		public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(DesignerItem), new FrameworkPropertyMetadata(false, IsSelectedChanged, IsSelectedCoerce));
		public virtual bool IsSelected
		{
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}

		public static readonly DependencyProperty IsSelectableProperty = DependencyProperty.Register("IsSelectable", typeof(bool), typeof(DesignerItem), new FrameworkPropertyMetadata(false, IsSelectableChanged, IsSelectableCoerce));
		public virtual bool IsSelectable
		{
			get { return (bool)GetValue(IsSelectableProperty); }
			set { SetValue(IsSelectableProperty, value); }
		}

		private static void IsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var designerItem = d as DesignerItem;
			if (e.Property == IsSelectedProperty)
			{
				if ((bool)e.NewValue && designerItem != null && designerItem.DesignerCanvas != null && designerItem.DesignerCanvas.SelectedItems.Count() == 1)
					EventService.EventAggregator.GetEvent<ElementSelectedEvent>().Publish(((CommonDesignerItem)d).Element);
				designerItem.IsSelectedChanged();
			}
		}
		private static void IsSelectableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (e.Property == IsSelectableProperty)
			{
				if (!(bool)e.NewValue)
					d.SetValue(IsSelectedProperty, false);
				((DesignerItem)d).IsSelectableChanged();
			}
		}
		private static object IsSelectedCoerce(DependencyObject d, object e)
		{
			DesignerItem designerItem = d as DesignerItem;
			return designerItem != null && !designerItem.IsSelectable ? false : e;
		}
		private static object IsSelectableCoerce(DependencyObject d, object e)
		{
			return e;
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
					Visibility = value ? Visibility.Visible : Visibility.Collapsed;
					if (!value)
						IsSelected = false;
				}
			}
		}

		private bool _isSelectableLayout;
		public virtual bool IsSelectableLayout
		{
			get { return _isSelectableLayout; }
			set
			{
				if (_isSelectableLayout != value)
				{
					_isSelectableLayout = value;
					IsSelectable = value;
					if (!value)
						IsSelected = false;
				}
			}
		}

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

		protected virtual void IsSelectableChanged()
		{
		}
		protected virtual void IsSelectedChanged()
		{
		}
	}
}