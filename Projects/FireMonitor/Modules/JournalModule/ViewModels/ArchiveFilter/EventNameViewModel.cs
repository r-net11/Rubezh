using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using Infrastructure.Common.CheckBoxList;
using FiresecAPI.Events;
using System.Reflection;

namespace JournalModule.ViewModels
{
	public class EventNameViewModel : CheckBoxItemViewModel
	{
		public EventNameViewModel(GlobalEventNameEnum globalEventNameEnum)
		{
			EventName = globalEventNameEnum;

			FieldInfo fieldInfo = globalEventNameEnum.GetType().GetField(globalEventNameEnum.ToString());
			if (fieldInfo != null)
			{
				EventDescriptionAttribute[] descriptionAttributes = (EventDescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(EventDescriptionAttribute), false);
				if (descriptionAttributes.Length > 0)
				{
					EventDescriptionAttribute eventDescriptionAttribute = descriptionAttributes[0];
					Name = eventDescriptionAttribute.Name;
					SubsystemType = eventDescriptionAttribute.SubsystemType;
					StateClass = eventDescriptionAttribute.StateClass;
				}
			}
		}

		public GlobalEventNameEnum EventName { get; private set; }
		public string Name { get; private set; }
		public XStateClass StateClass { get; private set; }
		public GlobalSubsystemType SubsystemType { get; private set; }

		bool _isEnabled;
		public bool IsEnabled
		{
			get { return _isEnabled; }
			set
			{
				_isEnabled = value;
				OnPropertyChanged("IsEnabled");
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