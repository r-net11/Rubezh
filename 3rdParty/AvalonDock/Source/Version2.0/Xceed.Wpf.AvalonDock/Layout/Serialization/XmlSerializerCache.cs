using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Xceed.Wpf.AvalonDock.Layout.Serialization
{
	internal static class XmlSerializerCache
	{
		private static Dictionary<Type, XmlSerializer> _serializers = new Dictionary<Type, XmlSerializer>();
		public static XmlSerializer Get<T>()
		{
			var type = typeof(T);
			if (!_serializers.ContainsKey(type))
				_serializers.Add(type, new XmlSerializer(type));
			return _serializers[type];
		}
	}
}
