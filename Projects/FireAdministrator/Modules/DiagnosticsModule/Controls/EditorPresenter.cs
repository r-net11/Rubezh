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
				bool val = IsEditing;
				SetValue(IsEditingProperty, value);
				if (val != IsEditing)
				{
					if (IsEditing && EditTemplate != null)
						Template = EditTemplate;
					else if (!IsEditing)
						Template = ViewTemplate;
				}
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

		private bool _canBeEdit = false;
		private bool _isMouseWithinScope = false;
		private TreeViewItem _treeViewItem;

		public EditorPresenter()
		{
			LostKeyboardFocus += new KeyboardFocusChangedEventHandler(OnEditorLostKeyboardFocus);
			IsKeyboardFocusWithinChanged += new DependencyPropertyChangedEventHandler(OnEditorIsKeyboardFocusWithinChanged);
			LostKeyboardFocus += new KeyboardFocusChangedEventHandler(OnEditorLostKeyboardFocus);
			LayoutUpdated += new EventHandler(OnEditorLayoutUpdated);
			KeyDown += new KeyEventHandler(OnEditorKeyDown);
			FocusVisualStyle = null;
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
					IsEditing = true;

				if (IsParentSelected)
					_isMouseWithinScope = true;
			}
		}
		protected override void OnPreviewMouseDoubleClick(MouseButtonEventArgs e)
		{
			e.Handled = true;
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			if (IsEditing)
				Focus();
			if (_treeViewItem == null)
				_treeViewItem = GetDependencyObjectFromVisualTree(this, typeof(TreeViewItem)) as TreeViewItem;
		}
		private void OnEditorLayoutUpdated(object sender, EventArgs e)
		{
			if (IsEditing && !IsKeyboardFocusWithin)
				Focus();
		}
		private void OnEditorKeyDown(object sender, KeyEventArgs e)
		{
			if (IsEditing && (e.Key == Key.Enter || e.Key == Key.F2))
			{
				IsEditing = false;
				_canBeEdit = false;
			}
		}
		private void OnEditorLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			if (!IsKeyboardFocusWithin)
				IsEditing = false;
		}
		private void OnEditorIsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.OldValue == true && (bool)e.NewValue == false)
				IsEditing = false;
		}

		private bool IsParentSelected
		{
			get
			{
				if (_treeViewItem == null)
					return true;
				else
					return _treeViewItem.IsSelected;
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
