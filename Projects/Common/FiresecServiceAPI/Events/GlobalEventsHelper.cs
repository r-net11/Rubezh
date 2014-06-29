using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace FiresecAPI.Events
{
	public static class GlobalEventsHelper
	{
		public static EventDescriptionAttribute ToDescription(this Enum value)
		{
			FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
			if (fieldInfo != null)
			{
				EventDescriptionAttribute[] descriptionAttributes = (EventDescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(EventDescriptionAttribute), false);
				if (descriptionAttributes.Length > 0)
					return descriptionAttributes[0];
			}
			return null;
		}
	}
}