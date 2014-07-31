
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Controls
{
    public class LayoutDocumentPaneGroupTabControl : ContentControl
	{
		static LayoutDocumentPaneGroupTabControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutDocumentPaneGroupTabControl), new FrameworkPropertyMetadata(typeof(LayoutDocumentPaneGroupTabControl)));
		}

		public LayoutDocumentPaneGroupTabControl()
		{
		}

		#region Model

		/// <summary>
		/// Model Dependency Property
		/// </summary>
		public static readonly DependencyProperty ModelProperty =
			DependencyProperty.Register("Model", typeof(LayoutElement), typeof(LayoutDocumentPaneGroupTabControl),
				new FrameworkPropertyMetadata((LayoutElement)null,
					new PropertyChangedCallback(OnModelChanged)));

		/// <summary>
		/// Gets or sets the Model property.  This dependency property 
		/// indicates the layout content model attached to the tab item.
		/// </summary>
		public LayoutElement Model
		{
			get { return (LayoutElement)GetValue(ModelProperty); }
			set { SetValue(ModelProperty, value); }
		}

		/// <summary>
		/// Handles changes to the Model property.
		/// </summary>
		private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((LayoutDocumentPaneGroupTabControl)d).OnModelChanged(e);
		}


		/// <summary>
		/// Provides derived classes an opportunity to handle changes to the Model property.
		/// </summary>
		protected virtual void OnModelChanged(DependencyPropertyChangedEventArgs e)
		{
			if (Model != null)
				Content = Model.Root.Manager.CreateUIElementForModel(Model);
			else
				Content = null;
		}

		#endregion
	}
}