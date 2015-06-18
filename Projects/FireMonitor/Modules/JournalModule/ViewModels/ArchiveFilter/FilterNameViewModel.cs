using System.Linq;
using System.Reflection;
using Controls.Converters;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using Infrastructure.Common.TreeList;

namespace JournalModule.ViewModels
{
	public class FilterNameViewModel : TreeNodeViewModel<FilterNameViewModel>
	{
		public FilterNameViewModel(JournalSubsystemType journalSubsystemType)
		{
			JournalSubsystemType = journalSubsystemType;
			//IsSubsystem = true;
			Name = journalSubsystemType.ToDescription();
			var converter = new JournalSubsystemTypeToIconConverter();
			ImageSource = (string)converter.Convert(journalSubsystemType, typeof(JournalSubsystemType), null, null);
		}

		public FilterNameViewModel(JournalEventNameType journalEventNameType)
		{
			JournalEventNameType = journalEventNameType;

			FieldInfo fieldInfo = journalEventNameType.GetType().GetField(journalEventNameType.ToString());
			if (fieldInfo != null)
			{
				EventNameAttribute[] descriptionAttributes = (EventNameAttribute[])fieldInfo.GetCustomAttributes(typeof(EventNameAttribute), false);
				if (descriptionAttributes.Length > 0)
				{
					EventNameAttribute eventNameAttribute = descriptionAttributes[0];
					Name = eventNameAttribute.Name;
					JournalSubsystemType = eventNameAttribute.JournalSubsystemType;
					StateClass = eventNameAttribute.StateClass;
					if (StateClass == XStateClass.Norm)
						ImageSource = null;
					else
						ImageSource = "/Controls;component/StateClassIcons/" + StateClass.ToString() + ".png";
				}
			}
			//IsSubsystem = false;
		}

	    public FilterNameViewModel (JournalEventDescriptionType journalEventDescriptionType, string name)
	    {
            JournalEventDescriptionType = journalEventDescriptionType;
            Name = name;
            ImageSource = "/Controls;component/Images/Blank.png";
	    }

	    public JournalEventNameType JournalEventNameType { get; private set; }
		public string Name { get; private set; }
		public string ImageSource { get; private set; }
		public XStateClass StateClass { get; private set; }
		public JournalSubsystemType JournalSubsystemType { get; private set; }
		//public bool IsSubsystem { get; private set; }
        public JournalEventDescriptionType JournalEventDescriptionType { get; private set; }

		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			
			set
			{
				SetIsChecked(value);
				PropogateDown(value);
				PropogateUp(value);
			}
		}

		public void SetIsChecked(bool value)
		{
			_isChecked = value;
			OnPropertyChanged(() => IsChecked);
		}
        void PropogateUp(bool value)
        {
            if (Parent != null)
            {
                var isAllChecked = Parent.Children.All(x => x.IsChecked == true);
                Parent.SetIsChecked(isAllChecked);
                Parent.PropogateUp(value);
            }
        }

        void PropogateDown(bool value)
        {
            foreach (var child in Children)
            {
                child.SetIsChecked(value);
                child.PropogateDown(value);
            }
        }
	}
}