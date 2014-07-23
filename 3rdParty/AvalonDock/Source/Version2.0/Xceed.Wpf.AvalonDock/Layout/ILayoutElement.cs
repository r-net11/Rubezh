/************************************************************************

   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the New BSD
   License (BSD) as published at http://avalondock.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up AvalonDock in Extended WPF Toolkit Plus at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like facebook.com/datagrids

  **********************************************************************/

using System.ComponentModel;
using System.Windows.Media;
using System;

namespace Xceed.Wpf.AvalonDock.Layout
{
	public interface ILayoutElement : INotifyPropertyChanged, INotifyPropertyChanging
	{
		ILayoutContainer Parent { get; }
		ILayoutRoot Root { get; }
		bool IsSelected { get; set; }
		string Title { get; set; }
		bool IsActive { get; set; }
		object ToolTip { get; set; }
		ImageSource IconSource { get; set; }
		DateTime? LastActivationTimeStamp { get; set; }
		int Margin { get; set; }
	}
}
