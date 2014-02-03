/************************************************************************

   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the New BSD
   License (BSD) as published at http://avalondock.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up AvalonDock in Extended WPF Toolkit Plus at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like facebook.com/datagrids

  **********************************************************************/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Controls
{
	public class LayoutDocumentControl : Control
	{
		static LayoutDocumentControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutDocumentControl), new FrameworkPropertyMetadata(typeof(LayoutDocumentControl)));
			FocusableProperty.OverrideMetadata(typeof(LayoutDocumentControl), new FrameworkPropertyMetadata(false));
		}

		public LayoutDocumentControl()
		{
			//SetBinding(FlowDirectionProperty, new Binding("Model.Root.Manager.FlowDirection") { Source = this });

		}


		#region Model

		/// <summary>
		/// Model Dependency Property
		/// </summary>
		public static readonly DependencyProperty ModelProperty =
			DependencyProperty.Register("Model", typeof(LayoutContent), typeof(LayoutDocumentControl),
				new FrameworkPropertyMetadata((LayoutContent)null,
					new PropertyChangedCallback(OnModelChanged)));

		/// <summary>
		/// Gets or sets the Model property.  This dependency property 
		/// indicates the model attached to this view.
		/// </summary>
		public LayoutContent Model
		{
			get { return (LayoutContent)GetValue(ModelProperty); }
			set { SetValue(ModelProperty, value); }
		}

		/// <summary>
		/// Handles changes to the Model property.
		/// </summary>
		private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((LayoutDocumentControl)d).OnModelChanged(e);
		}

		/// <summary>
		/// Provides derived classes an opportunity to handle changes to the Model property.
		/// </summary>
		protected virtual void OnModelChanged(DependencyPropertyChangedEventArgs e)
		{
			if (Model != null)
				SetLayoutItem(Model.Root.Manager.GetLayoutItemFromModel(Model));
			else
				SetLayoutItem(null);
		}
		#endregion

		#region LayoutItem

		/// <summary>
		/// LayoutItem Read-Only Dependency Property
		/// </summary>
		private static readonly DependencyPropertyKey LayoutItemPropertyKey
			= DependencyProperty.RegisterReadOnly("LayoutItem", typeof(LayoutItem), typeof(LayoutDocumentControl),
				new FrameworkPropertyMetadata((LayoutItem)null));

		public static readonly DependencyProperty LayoutItemProperty
			= LayoutItemPropertyKey.DependencyProperty;

		/// <summary>
		/// Gets the LayoutItem property.  This dependency property 
		/// indicates the LayoutItem attached to this tag item.
		/// </summary>
		public LayoutItem LayoutItem
		{
			get { return (LayoutItem)GetValue(LayoutItemProperty); }
		}

		/// <summary>
		/// Provides a secure method for setting the LayoutItem property.  
		/// This dependency property indicates the LayoutItem attached to this tag item.
		/// </summary>
		/// <param name="value">The new value for the property.</param>
		protected void SetLayoutItem(LayoutItem value)
		{
			SetValue(LayoutItemPropertyKey, value);
		}

		#endregion

		protected override void OnPreviewGotKeyboardFocus(System.Windows.Input.KeyboardFocusChangedEventArgs e)
		{
			if (Model != null)
				Model.IsActive = true;
			base.OnPreviewGotKeyboardFocus(e);
		}

		///////////////////////////////////////////////////

		bool _isMouseDown = false;
		Point _mouseDownPoint;

		protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown(e);
			Model.IsActive = true;
			if (e.ClickCount == 1)
			{
				_mouseDownPoint = e.GetPosition(this);
				_isMouseDown = true;
			}
		}
		protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
		{
			if (IsMouseCaptured)
				ReleaseMouseCapture();
			_isMouseDown = false;
			base.OnMouseLeftButtonUp(e);
		}

		protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (_isMouseDown)
			{
				Point ptMouseMove = e.GetPosition(this);
				if (Math.Abs(ptMouseMove.X - _mouseDownPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
					Math.Abs(ptMouseMove.Y - _mouseDownPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
				{
					CaptureMouse();
					_isMouseDown = false;
				}
			}
			if (IsMouseCaptured)
			{
				var mousePosInScreenCoord = this.PointToScreenDPI(e.GetPosition(this));
				ReleaseMouseCapture();
				var manager = Model.Root.Manager;
				manager.StartDraggingFloatingWindowForContent(Model);
			}
		}

		protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
		{
			base.OnMouseLeave(e);
			_isMouseDown = false;
		}
		protected override void OnMouseEnter(MouseEventArgs e)
		{
			base.OnMouseEnter(e);
			_isMouseDown = false;
		}
		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Middle)
			{
				if (LayoutItem.CloseCommand.CanExecute(null))
					LayoutItem.CloseCommand.Execute(null);
			}
			else if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2 && LayoutItem.ConfigureCommand != null && LayoutItem.ConfigureCommand.CanExecute(null))
				LayoutItem.ConfigureCommand.Execute(null);
			base.OnMouseDown(e);
		}
	}
}
