using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace DiagnosticsModule.Controls
{
	public class EditorPresenter : Control
	{
		static EditorPresenter()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(EditorPresenter), new FrameworkPropertyMetadata(typeof(EditorPresenter)));
		}

		public static DependencyProperty IsEditingProperty = DependencyProperty.Register("IsEditing", typeof(bool), typeof(EditorPresenter), new FrameworkPropertyMetadata(false));
		public bool IsEditing
		{
			get { return (bool)GetValue(IsEditingProperty); }
			private set
			{
				SetValue(IsEditingProperty, value);
				UpdateEditingMode();
			}
		}

		public static DependencyProperty EditTemplateProperty = DependencyProperty.Register("EditTemplate", typeof(ControlTemplate), typeof(EditorPresenter), new FrameworkPropertyMetadata());
		public ControlTemplate EditTemplate
		{
			get { return (ControlTemplate)this.GetValue(EditTemplateProperty); }
			set { SetValue(EditTemplateProperty, value); }
		}

		public static DependencyProperty ViewTemplateProperty = DependencyProperty.Register("ViewTemplate", typeof(ControlTemplate), typeof(EditorPresenter), new FrameworkPropertyMetadata(ViewTemplateChanged));
		public ControlTemplate ViewTemplate
		{
			get { return (ControlTemplate)this.GetValue(ViewTemplateProperty); }
			set { SetValue(ViewTemplateProperty, value); }
		}

		private EditorAdorner _adorner;
		private bool _canBeEdit = false;
		private bool _isMouseWithinScope = false;
		private ItemsControl _itemsControl;
		private TreeViewItem _treeViewItem;

		public EditorPresenter()
		{
			
		}

		private static void ViewTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			EditorPresenter presenter = (EditorPresenter)d;
			presenter.Template = (ControlTemplate)e.NewValue;
			presenter.IsEditing = false;
		}

		protected override void OnMouseEnter(MouseEventArgs e)
		{
			base.OnMouseEnter(e);
			if (!IsEditing && IsParentSelected)
			{
				_canBeEdit = true;
			}
		}
		protected override void OnMouseLeave(MouseEventArgs e)
		{
			base.OnMouseLeave(e);
			_isMouseWithinScope = false;
			_canBeEdit = false;
		}
		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			base.OnMouseUp(e);

			if (e.ChangedButton == MouseButton.Right || e.ChangedButton == MouseButton.Middle)
				return;

			if (!IsEditing)
			{
				if (!e.Handled && (_canBeEdit || _isMouseWithinScope))
				{
					IsEditing = true;
				}

				//Handle a specific case: After a ListViewItem was selected by clicking it,
				// Clicking the EditBox again should switch into Editable mode.
				if (IsParentSelected)
					_isMouseWithinScope = true;
			}
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			//hook resize event to handle the the column resize. 
			HookTemplateParentResizeEvent();
			//hook the resize event to  handle ListView resize cases.
			HookItemsControlEvents();
			_treeViewItem = GetDependencyObjectFromVisualTree(this, typeof(TreeViewItem)) as TreeViewItem;
		}

		private void UpdateEditingMode()
		{
			//if (value != IsEditing)
			//    Template = value ? EditTemplate : ViewTemplate;
			if (IsEditing && _adorner == null)
			{
				var child = (VisualChildrenCount == 1 ? GetVisualChild(0) : null) as UIElement;
				_adorner = new EditorAdorner(this, child);
				AdornerLayer layer = AdornerLayer.GetAdornerLayer(child);
				layer.Add(_adorner);
				_adorner.Root.IsKeyboardFocusWithinChanged += new DependencyPropertyChangedEventHandler(Root_IsKeyboardFocusWithinChanged);
				_adorner.Root.KeyDown += new KeyEventHandler(OnTextBoxKeyDown);
				_adorner.Root.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(OnTextBoxLostKeyboardFocus);
			}
			if (_adorner != null)
				_adorner.UpdateVisibilty(IsEditing);
		}

		private void OnTextBoxKeyDown(object sender, KeyEventArgs e)
		{
			if (IsEditing && (e.Key == Key.Enter || e.Key == Key.F2))
			{
				IsEditing = false;
				_canBeEdit = false;
			}
		}
		private void OnTextBoxLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			if (!((Control)sender).IsKeyboardFocusWithin)
				IsEditing = false;
		}
		private void Root_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.OldValue == true && (bool)e.NewValue == false)
				IsEditing = false;
		}

		private bool IsParentSelected
		{
			get
			{
				if (_treeViewItem == null)
					return false;
				else
					return _treeViewItem.IsSelected;
			}
		}

		private void OnCouldSwitchToNormalMode(object sender, RoutedEventArgs e)
		{
			IsEditing = false;
		}
		private void OnScrollViewerChanged(object sender, RoutedEventArgs args)
		{
			if (IsEditing && Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed)
			{
				IsEditing = false;
			}
		}
		private void HookItemsControlEvents()
		{
			_itemsControl = GetDependencyObjectFromVisualTree(this, typeof(ItemsControl)) as ItemsControl;
			if (_itemsControl != null)
			{
				//The reason of hooking Resize/ScrollChange/MouseWheel event is :
				//when one of these event happens, the EditBox should be switched into editable mode.
				_itemsControl.SizeChanged += new SizeChangedEventHandler(OnCouldSwitchToNormalMode);
				_itemsControl.AddHandler(ScrollViewer.ScrollChangedEvent, new RoutedEventHandler(OnScrollViewerChanged));
				_itemsControl.AddHandler(ScrollViewer.MouseWheelEvent, new RoutedEventHandler(OnCouldSwitchToNormalMode), true);
			}
		}
		private void HookTemplateParentResizeEvent()
		{
			FrameworkElement parent = TemplatedParent as FrameworkElement;
			if (parent != null)
			{
				parent.SizeChanged += new SizeChangedEventHandler(OnCouldSwitchToNormalMode);
			}
		}
		private DependencyObject GetDependencyObjectFromVisualTree(DependencyObject startObject, Type type)
		{
			//Iterate the visual tree to get the parent(ItemsControl) of this control
			DependencyObject parent = startObject;
			while (parent != null)
			{
				if (type.IsInstanceOfType(parent))
					break;
				else
					parent = VisualTreeHelper.GetParent(parent);
			}
			return parent;
		}
	}

}
