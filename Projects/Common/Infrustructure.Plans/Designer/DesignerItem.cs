using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Events;
using System.Windows.Media;

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
		public static readonly DependencyProperty IsSelectableProperty = DependencyProperty.Register("IsSelectable", typeof(bool), typeof(DesignerItem), new FrameworkPropertyMetadata(true, IsSelectableChanged, IsSelectableCoerce));
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
				var designerItem = d as DesignerItem;
				designerItem.IsSelectableChanged();
			}
		}
		private static object IsSelectedCoerce(DependencyObject d, object e)
		{
			DesignerItem designerItem = d as DesignerItem;
			return designerItem != null && (!designerItem.IsSelectable || !designerItem.IsEnabled) ? false : e;
		}
		private static object IsSelectableCoerce(DependencyObject d, object e)
		{
			return e;
		}

		public override bool IsVisibleLayout
		{
			get { return base.IsVisibleLayout; }
			set
			{
				if (IsVisibleLayout != value && !value)
					IsSelected = false;
				base.IsVisibleLayout = value;
			}
		}
		public ICommand ShowPropertiesCommand { get; protected set; }
		public ICommand DeleteCommand { get; protected set; }

		public ResizeChrome ResizeChrome { get; private set; }
		public string Group { get; set; }
		public override bool AllowDrag { get { return true; } }

		public DesignerItem(ElementBase element)
			: base(element)
		{
			Group = string.Empty;
			IsVisibleLayout = true;
			_moveAdorner = null;
		}

		protected override void ResetIsEnabled()
		{
			base.ResetIsEnabled();
			IsEnabled &= IsSelectable;
			if (!IsEnabled)
				IsSelected = false;
		}
		public override void Redraw()
		{
			base.Redraw();
			if (ResizeChrome != null)
				ResizeChrome.Redraw();
		}
		public override void RefreshPainter()
		{
			base.RefreshPainter();
			if (ResizeChrome != null)
				ResizeChrome.InvalidateVisual();
		}
		protected void SetResizeChrome(ResizeChrome resizeChrome)
		{
			//ResizeChrome = resizeChrome;
			//Children.Add(ResizeChrome);
			if (ResizeChrome != null)
				ResizeChrome.IsVisible = IsSelected;
		}

		protected override void MouseDown(Point point, MouseButtonEventArgs e)
		{
			base.MouseDown(point, e);
			if (IsEnabled && DesignerCanvas != null)
			{
				if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
					IsSelected = !IsSelected;
				else if (!IsSelected)
				{
					DesignerCanvas.DeselectAll();
					IsSelected = true;
				}
			}
		}
		protected override void MouseDoubleClick(Point point, MouseButtonEventArgs e)
		{
			base.MouseDoubleClick(point, e);
			if (IsEnabled && DesignerCanvas != null)
				ShowPropertiesCommand.Execute(null);
		}

		protected override void SetIsMouseOver(bool value)
		{
			base.SetIsMouseOver(value);
			if (_moveAdorner == null)
				DesignerCanvas.Cursor = value && IsEnabled ? Cursors.SizeAll : Cursors.Arrow;
		}
		protected override ContextMenu ContextMenuOpening()
		{
			if (IsEnabled)
			{
				if (!IsSelected)
				{
					DesignerCanvas.DeselectAll();
					IsSelected = true;
				}
				return base.ContextMenuOpening();
			}
			else
				return null;
		}

		protected abstract void OnShowProperties();
		protected abstract void OnDelete();

		protected void IsSelectableChanged()
		{
			ResetIsEnabled();
			SetIsMouseOver(false);
		}
		protected void IsSelectedChanged()
		{
			if (ResizeChrome != null)
				ResizeChrome.IsVisible = IsSelected;
		}

		private MoveAdorner _moveAdorner;
		public override void DragStarted(Point point)
		{
			if (IsSelected)
			{
				IsBusy = true;
				DesignerCanvas.BeginChange();
				if (_moveAdorner != null)
					_moveAdorner.Hide();
				_moveAdorner = new MoveAdorner(DesignerCanvas);
				_moveAdorner.Show(point);
			}
		}
		public override void DragCompleted(Point point)
		{
			IsBusy = false;
			if (_moveAdorner != null)
			{
				_moveAdorner.Hide();
				if (_moveAdorner.IsMoved)
					DesignerCanvas.EndChange();
				_moveAdorner = null;
			}
		}
	}
}