using System.Linq;
using System.Reflection;
using Controls.Converters;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using Infrastructure.Common.Windows.TreeList;

namespace JournalModule.ViewModels
{
	public class FilterNameViewModel : TreeNodeViewModel<FilterNameViewModel>
	{
		public FilterNameViewModel(JournalSubsystemType journalSubsystemType)
		{
			JournalSubsystemType = journalSubsystemType;
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
						ImageSource = "/Controls;component/Images/Blank.png";
					else
						ImageSource = "/Controls;component/StateClassIcons/" + StateClass.ToString() + ".png";
				}
			}
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
		public JournalEventDescriptionType JournalEventDescriptionType { get; private set; }
		bool _isChecked;

		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				SetIsChecked(value);
				PropagateDown(value);
			}
		}

		public void SetIsChecked(bool value)
		{
			_isChecked = value;
			OnPropertyChanged(() => IsChecked);
		}

        void PropagateDown(bool value)
        {
            foreach (var child in Children)
            {
                child.SetIsChecked(value);
                child.PropagateDown(value);
            }
        }
	}
}