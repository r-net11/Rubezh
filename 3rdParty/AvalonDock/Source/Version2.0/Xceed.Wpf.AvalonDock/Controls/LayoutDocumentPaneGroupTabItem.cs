

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.AvalonDock.Layout;

namespace Xceed.Wpf.AvalonDock.Controls
{
	public class LayoutDocumentPaneGroupTabItem : Control
	{
		static LayoutDocumentPaneGroupTabItem()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutDocumentPaneGroupTabItem), new FrameworkPropertyMetadata(typeof(LayoutDocumentPaneGroupTabItem)));
		}

		public LayoutDocumentPaneGroupTabItem()
		{
		}

		#region Model

		/// <summary>
		/// Model Dependency Property
		/// </summary>
		public static readonly DependencyProperty ModelProperty =
			DependencyProperty.Register("Model", typeof(LayoutDocumentPaneGroup), typeof(LayoutDocumentPaneGroupTabItem),
				new FrameworkPropertyMetadata((LayoutDocumentPaneGroup)null,
					new PropertyChangedCallback(OnModelChanged)));

		/// <summary>
		/// Gets or sets the Model property.  This dependency property 
		/// indicates the layout content model attached to the tab item.
		/// </summary>
		public LayoutDocumentPaneGroup Model
		{
			get { return (LayoutDocumentPaneGroup)GetValue(ModelProperty); }
			set { SetValue(ModelProperty, value); }
		}

		/// <summary>
		/// Handles changes to the Model property.
		/// </summary>
		private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((LayoutDocumentPaneGroupTabItem)d).OnModelChanged(e);
		}


		/// <summary>
		/// Provides derived classes an opportunity to handle changes to the Model property.
		/// </summary>
		protected virtual void OnModelChanged(DependencyPropertyChangedEventArgs e)
		{

		}

		#endregion


	}
}
