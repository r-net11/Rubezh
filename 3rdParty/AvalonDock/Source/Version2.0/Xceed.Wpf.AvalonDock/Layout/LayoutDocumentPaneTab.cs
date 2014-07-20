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
using System.Windows.Controls;
using System.Windows.Markup;

namespace Xceed.Wpf.AvalonDock.Layout
{
	[ContentProperty("Children")]
	[Serializable]
	public class LayoutDocumentPaneTab : LayoutPositionableGroup<ILayoutDocumentPane>, ILayoutDocumentPane
	{
		public LayoutDocumentPaneTab()
		{
		}

		public LayoutDocumentPaneTab(LayoutDocumentPane documentPane)
		{
			Children.Add(documentPane);
		}

		protected override bool GetVisibility()
		{
			return true;
		}

		public override void WriteXml(System.Xml.XmlWriter writer)
		{
			base.WriteXml(writer);
		}

		public override void ReadXml(System.Xml.XmlReader reader)
		{
			base.ReadXml(reader);
		}

#if DEBUG
		public override void ConsoleDump(int tab)
		{
			System.Diagnostics.Debug.Write(new string(' ', tab * 4));
			System.Diagnostics.Debug.WriteLine(string.Format("LayoutDocumentPaneTab({0})", ChildrenCount));

			foreach (LayoutElement child in Children)
				child.ConsoleDump(tab + 1);
		}
#endif
	}
}
