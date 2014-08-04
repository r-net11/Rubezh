using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using Infrastructure.Common.CheckBoxList;

namespace GKModule.ViewModels
{
	public class EventNameViewModel : CheckBoxItemViewModel
	{
		public EventNameViewModel(JournalEventNameType journalEventNameType, List<string> distinctDatabaseDescriptions)
		{
			EventName = journalEventNameType;

			FieldInfo fieldInfo = journalEventNameType.GetType().GetField(journalEventNameType.ToString());
			if (fieldInfo != null)
			{
				EventDescriptionAttribute[] descriptionAttributes = (EventDescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(EventDescriptionAttribute), false);
				if (descriptionAttributes.Length > 0)
				{
					EventDescriptionAttribute eventDescriptionAttribute = descriptionAttributes[0];
					Name = eventDescriptionAttribute.Name;
					JournalSubsystemType = eventDescriptionAttribute.JournalSubsystemType;
					StateClass = eventDescriptionAttribute.StateClass;
				}
			}

			IsEnabled = distinctDatabaseDescriptions.Any(x => x == Name);
		}

		public JournalEventNameType EventName { get; private set; }
		public string Name { get; private set; }
		public XStateClass StateClass { get; private set; }
		public JournalSubsystemType JournalSubsystemType { get; private set; }
		
		bool _isEnabled;
		public bool IsEnabled
		{
			get { return _isEnabled; }
			set
			{
				_isEnabled = value;
				OnPropertyChanged(() => IsEnabled);
			}
		}

		public static int Compare(EventNameViewModel x, EventNameViewModel y)
		{
			if (x.IsEnabled && !y.IsEnabled)
				return -1;
			if (!x.IsEnabled && y.IsEnabled)
				return 1;
			return x.Name.CompareTo(y.Name);
		}
	}
}