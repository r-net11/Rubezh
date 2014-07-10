using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using FiresecAPI.GK;

namespace FiresecAPI.Journal
{
	public static class EventDescriptionAttributeHelper
	{
		public static string ToName(JournalEventNameType journalEventNameType)
		{
			string name = null;
			FieldInfo fieldInfo = journalEventNameType.GetType().GetField(journalEventNameType.ToString());
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

		public static XStateClass ToStateClass(JournalEventNameType journalEventNameType)
		{
			XStateClass stateClass = XStateClass.No;
			FieldInfo fieldInfo = journalEventNameType.GetType().GetField(journalEventNameType.ToString());
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