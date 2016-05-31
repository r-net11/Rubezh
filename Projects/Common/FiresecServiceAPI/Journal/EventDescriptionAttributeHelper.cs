using StrazhAPI.GK;

namespace StrazhAPI.Journal
{
	public static class EventDescriptionAttributeHelper
	{
		public static string ToName(JournalEventNameType journalEventNameType)
		{
			string name = null;
			var fieldInfo = journalEventNameType.GetType().GetField(journalEventNameType.ToString());
			if (fieldInfo != null)
			{
				var descriptionAttributes = (EventNameAttribute[])fieldInfo.GetCustomAttributes(typeof(EventNameAttribute), false);
				if (descriptionAttributes.Length > 0)
				{
					var eventDescriptionAttribute = descriptionAttributes[0];
					name = eventDescriptionAttribute.Name;
				}
			}
			return name;
		}

		public static string ToName(JournalEventDescriptionType journalEventDescriptionType)
		{
			string name = null;
			var fieldInfo = journalEventDescriptionType.GetType().GetField(journalEventDescriptionType.ToString());
			if (fieldInfo != null)
			{
				var descriptionAttributes = (EventDescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(EventDescriptionAttribute), false);
				if (descriptionAttributes.Length > 0)
				{
					var eventDescriptionAttribute = descriptionAttributes[0];
					name = eventDescriptionAttribute.Name;
				}
			}
			return name;
		}

		public static JournalSubsystemType ToSubsystem(JournalEventNameType journalEventNameType)
		{
			var subsystemType = JournalSubsystemType.SKD;
			var fieldInfo = journalEventNameType.GetType().GetField(journalEventNameType.ToString());
			if (fieldInfo != null)
			{
				var descriptionAttributes = (EventNameAttribute[])fieldInfo.GetCustomAttributes(typeof(EventNameAttribute), false);
				if (descriptionAttributes.Length > 0)
				{
					var eventDescriptionAttribute = descriptionAttributes[0];
					subsystemType = eventDescriptionAttribute.JournalSubsystemType;
				}
			}
			return subsystemType;
		}

		public static XStateClass ToStateClass(JournalEventNameType journalEventNameType)
		{
			var stateClass = XStateClass.No;
			var fieldInfo = journalEventNameType.GetType().GetField(journalEventNameType.ToString());
			if (fieldInfo != null)
			{
				var descriptionAttributes = (EventNameAttribute[])fieldInfo.GetCustomAttributes(typeof(EventNameAttribute), false);
				if (descriptionAttributes.Length > 0)
				{
					var eventDescriptionAttribute = descriptionAttributes[0];
					stateClass = eventDescriptionAttribute.StateClass;
				}
			}
			return stateClass;
		}
	}
}