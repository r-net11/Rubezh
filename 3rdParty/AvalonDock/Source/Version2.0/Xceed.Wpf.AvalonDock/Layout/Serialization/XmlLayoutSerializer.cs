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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace Xceed.Wpf.AvalonDock.Layout.Serialization
{
	public class XmlLayoutSerializer : LayoutSerializer
	{
		private XmlSerializer _serializer;

		public XmlLayoutSerializer(DockingManager manager)
			: base(manager)
		{
			_serializer = new XmlSerializer(typeof(LayoutRoot));
		}

		public void Serialize(System.Xml.XmlWriter writer)
		{
			_serializer.Serialize(writer, Manager.Layout);
		}
		public void Serialize(System.IO.TextWriter writer)
		{
			_serializer.Serialize(writer, Manager.Layout);
		}
		public void Serialize(System.IO.Stream stream)
		{
			_serializer.Serialize(stream, Manager.Layout);
		}

		public void Serialize(string filepath)
		{
			using (var stream = new StreamWriter(filepath))
				Serialize(stream);
		}

		public void Deserialize(System.IO.Stream stream)
		{
			try
			{
				StartDeserialization();
				var layout = _serializer.Deserialize(stream) as LayoutRoot;
				FixupLayout(layout);
				Manager.Layout = layout;
			}
			finally
			{
				EndDeserialization();
			}
		}

		public void Deserialize(System.IO.TextReader reader)
		{
			try
			{
				StartDeserialization();
				var layout = _serializer.Deserialize(reader) as LayoutRoot;
				FixupLayout(layout);
				Manager.Layout = layout;
			}
			finally
			{
				EndDeserialization();
			}
		}

		public void Deserialize(System.Xml.XmlReader reader)
		{
			try
			{
				StartDeserialization();
				var layout = _serializer.Deserialize(reader) as LayoutRoot;
				FixupLayout(layout);
				Manager.Layout = layout;
			}
			finally
			{
				EndDeserialization();
			}
		}

		public void Deserialize(string filepath)
		{
			using (var stream = new StreamReader(filepath))
				Deserialize(stream);
		}
	}
}
