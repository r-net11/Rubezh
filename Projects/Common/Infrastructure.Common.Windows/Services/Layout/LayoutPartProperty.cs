using System;
using RubezhAPI.Automation;

namespace Infrastructure.Common.Windows.Services.Layout
{
	public class LayoutPartProperty
	{
		public LayoutPartProperty(LayoutPartPropertyAccess access, Type type, LayoutPartPropertyName name)
		{
			Access = access;
			Type = type;
			Name = name;
		}

		public LayoutPartPropertyAccess Access { get; private set; }
		public Type Type { get; private set; }
		public LayoutPartPropertyName Name { get; private set; }

		public LayoutPartPropertyType PropertyType
		{
			get
			{
				if (Type == typeof(Boolean))
					return LayoutPartPropertyType.Boolean;
				else if (Type == typeof(Int16) || Type == typeof(Int32) || Type == typeof(Int64))
					return LayoutPartPropertyType.Integer;
				else if (Type == typeof(double))
					return LayoutPartPropertyType.Double;
				else if (Type == typeof(DateTime))
					return LayoutPartPropertyType.DateTime;
				else if (Type == typeof(string))
					return LayoutPartPropertyType.String;
				else
					return LayoutPartPropertyType.Object;
			}
		}
	}
}
