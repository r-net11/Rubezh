using System.Reflection;
using StrazhAPI.GK;

namespace StrazhAPI.Journal
{
	public static class EventDescriptionAttributeHelper
	{
		public static string ToName(JournalEventNameType journalEventNameType)
		{
			string name = null;
			FieldInfo fieldInfo = journalEventNameType.GetType().GetField(journalEventNameType.ToString());
			if (fieldInfo != null)
			{
				EventNameAttribute[] descriptionAttributes = (EventNameAttribute[])fieldInfo.GetCustomAttributes(typeof(EventNameAttribute), false);
				if (descriptionAttributes.Length > 0)
				{
					EventNameAttribute eventDescriptionAttribute = descriptionAttributes[0];
					name = eventDescriptionAttribute.Name;
				}
			}
			return name;
		}

		public static string ToName(JournalEventDescriptionType journalEventDescriptionType)
		{
			string name = null;
			FieldInfo fieldInfo = journalEventDescriptionType.GetType().GetField(journalEventDescriptionType.ToString());
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

		public static JournalSubsystemType ToSubsystem(JournalEventNameType journalEventNameType)
		{
			JournalSubsystemType subsystemType = JournalSubsystemType.SKD;
			FieldInfo fieldInfo = journalEventNameType.GetType().GetField(journalEventNameType.ToString());
			if (fieldInfo != null)
			{
				EventNameAttribute[] descriptionAttributes = (EventNameAttribute[])fieldInfo.GetCustomAttributes(typeof(EventNameAttribute), false);
				if (descriptionAttributes.Length > 0)
				{
					EventNameAttribute eventDescriptionAttribute = descriptionAttributes[0];
					subsystemType = eventDescriptionAttribute.JournalSubsystemType;
				}
			}
			return subsystemType;
		}

		public static XStateClass ToStateClass(JournalEventNameType journalEventNameType)
		{
			XStateClass stateClass = XStateClass.No;
			FieldInfo fieldInfo = journalEventNameType.GetType().GetField(journalEventNameType.ToString());
			if (fieldInfo != null)
			{
				EventNameAttribute[] descriptionAttributes = (EventNameAttribute[])fieldInfo.GetCustomAttributes(typeof(EventNameAttribute), false);
				if (descriptionAttributes.Length > 0)
				{
					EventNameAttribute eventDescriptionAttribute = descriptionAttributes[0];
					stateClass = eventDescriptionAttribute.StateClass;
				}
			}
			return stateClass;
		}
	}
}