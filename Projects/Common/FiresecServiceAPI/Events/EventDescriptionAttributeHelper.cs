using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using FiresecAPI.GK;

namespace FiresecAPI.Events
{
	public static class EventDescriptionAttributeHelper
	{
		public static string ToName(GlobalEventNameEnum globalEventNameEnum)
		{
			string name = null;
			FieldInfo fieldInfo = globalEventNameEnum.GetType().GetField(globalEventNameEnum.ToString());
			if (fieldInfo != null)
			{
				EventDescriptionAttribute[] descriptionAttributes = (EventDescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(EventDescriptionAttribute), false);
				if (descriptionAttributes.Length > 0)
				{
					EventDescriptionAttribute eventDescriptionAttribute = descriptionAttributes[0];
					name = eventDescriptionAttribute.Name;
				}
			}
			return name;
		}

		public static XStateClass ToStateClass(GlobalEventNameEnum globalEventNameEnum)
		{
			XStateClass stateClass = XStateClass.No;
			FieldInfo fieldInfo = globalEventNameEnum.GetType().GetField(globalEventNameEnum.ToString());
			if (fieldInfo != null)
			{
				EventDescriptionAttribute[] descriptionAttributes = (EventDescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(EventDescriptionAttribute), false);
				if (descriptionAttributes.Length > 0)
				{
					EventDescriptionAttribute eventDescriptionAttribute = descriptionAttributes[0];
					stateClass = eventDescriptionAttribute.StateClass;
				}
			}
			return stateClass;
		}
	}
}