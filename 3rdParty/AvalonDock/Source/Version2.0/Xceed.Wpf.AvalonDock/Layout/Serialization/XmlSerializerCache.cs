using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Xceed.Wpf.AvalonDock.Layout.Serialization
{
	internal static class XmlSerializerCache
	{
		private static Dictionary<Type, XmlSerializer> _serializers = new Dictionary<Type, XmlSerializer>();
		public static XmlSerializer Get<T>()
		{
			var type = typeof(T);
			return Get(type);
		}
		public static XmlSerializer Get(Type type)
		{
			if (!_serializers.ContainsKey(type))
				_serializers.Add(type, new XmlSerializer(type));
			return _serializers[type];
		}
	}
}
