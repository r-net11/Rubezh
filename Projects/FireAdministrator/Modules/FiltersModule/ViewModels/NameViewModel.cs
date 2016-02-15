using System.Linq;
using System.Reflection;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using Infrastructure.Common.TreeList;
using Controls.Converters;
using System;

namespace FiltersModule.ViewModels
{
	public class NameViewModel : TreeNodeViewModel<NameViewModel>
	{
		public JournalSubsystemType JournalSubsystemType { get; private set; }
		public JournalEventNameType JournalEventNameType { get; private set; }

		public string Name { get; private set; }
		public string ImageSource { get; private set; }
		public XStateClass StateClass { get; private set; }
		public bool IsSubsystem { get; private set; }

		public NameViewModel(JournalSubsystemType journalSubsystemType)
		{
			JournalSubsystemType = journalSubsystemType;
			Name = journalSubsystemType.ToDescription();
			var converter = new JournalSubsystemTypeToIconConverter();
			ImageSource = (string)converter.Convert(journalSubsystemType, typeof(JournalSubsystemType), null, null);
			IsSubsystem = true;
		}

		public NameViewModel(JournalEventNameType journalEventNameType)
		{
			JournalEventNameType = journalEventNameType;

			FieldInfo fieldInfo = journalEventNameType.GetType().GetField(journalEventNameType.ToString());
			if (fieldInfo != null)
			{
				EventNameAttribute[] eventNameAttributes = (EventNameAttribute[])fieldInfo.GetCustomAttributes(typeof(EventNameAttribute), false);
				if (eventNameAttributes.Length > 0)
				{
					EventNameAttribute eventNameAttribute = eventNameAttributes[0];
					Name = eventNameAttribute.NameInFilter;
					JournalSubsystemType = eventNameAttribute.JournalSubsystemType;
					StateClass = eventNameAttribute.StateClass;
					if (StateClass == XStateClass.Norm)
						ImageSource = null;
					else
						ImageSource = "/Controls;component/StateClassIcons/" + StateClass.ToString() + ".png";
				}
			}

			IsSubsystem = false;
		}

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

		void PropogateDown(bool value)
		{
			foreach (var child in Children)
			{
				child.SetIsChecked(value);
				child.PropogateDown(value);
			}
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

		void SetIsChecked(bool value)
		{
			_isChecked = value;
			OnPropertyChanged(() => IsChecked);
		}
	}
}