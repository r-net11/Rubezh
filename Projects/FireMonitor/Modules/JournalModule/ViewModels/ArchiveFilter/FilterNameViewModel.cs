using System.Linq;
using System.Reflection;
using Controls.Converters;
using StrazhAPI;
using StrazhAPI.GK;
using StrazhAPI.Journal;
using Infrastructure.Common.TreeList;

namespace JournalModule.ViewModels
{
	public class FilterNameViewModel : TreeNodeViewModel<FilterNameViewModel>
	{
		public FilterNameViewModel(JournalSubsystemType journalSubsystemType)
		{
			JournalSubsystemType = journalSubsystemType;
			IsSubsystem = true;
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

		public JournalEventNameType JournalEventNameType { get; private set; }
		public string Name { get; private set; }
		public string ImageSource { get; private set; }
		public XStateClass StateClass { get; private set; }
		public JournalSubsystemType JournalSubsystemType { get; private set; }
		public bool IsSubsystem { get; private set; }

		private bool _isChecked;
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

		private void PropogateDown(bool value)
		{
			foreach (var child in Children)
			{
				child.SetIsChecked(value);
				child.PropogateDown(value);
			}
		}

		private void PropogateUp(bool value)
		{
			if (Parent == null)
				return;

			Parent.SetIsChecked(Parent.Children.All(x => x.IsChecked));
			Parent.PropogateUp(value);
		}
	}
}