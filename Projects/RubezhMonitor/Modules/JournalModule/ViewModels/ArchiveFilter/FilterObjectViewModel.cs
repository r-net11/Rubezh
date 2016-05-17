using Controls.Converters;
using Infrastructure.Common.TreeList;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using System;
using System.Linq;

namespace JournalModule.ViewModels
{
	public class FilterObjectViewModel : TreeNodeViewModel<FilterObjectViewModel>
	{
		public Guid UID { get; private set; }
		public FilterObjectViewModel(JournalSubsystemType journalSubsystemType)
		{
			Name = journalSubsystemType.ToDescription();
			var converter = new JournalSubsystemTypeToIconConverter();
			ImageSource = (string)converter.Convert(journalSubsystemType, typeof(JournalSubsystemType), null, null);
			FilterObjectType = FilterObjectType.Subsystem;
		}

		public FilterObjectViewModel(JournalObjectType journalObjectType)
		{
			JournalObjectType = journalObjectType;
			Name = journalObjectType.ToDescription();
			FilterObjectType = FilterObjectType.ObjectType;
			ImageSource = JournalItem.GetImageSource(journalObjectType);
		}

		public FilterObjectViewModel(GKBase gkBase)
		{
			Name = gkBase.PresentationName;
			UID = gkBase.UID;
			ImageSource = gkBase.ImageSource;
			FilterObjectType = FilterObjectType.Object;
		}

		public FilterObjectViewModel(RubezhAPI.Models.Camera camera)
		{
			Name = camera.PresentationName;
			UID = camera.UID;
			ImageSource = "/Controls;component/Images/Camera.png";
			FilterObjectType = FilterObjectType.Camera;
		}

		public FilterObjectViewModel(RubezhAPI.SKD.IHRListItem item)
		{
			Name = item.Name;
			UID = item.UID;
			ImageSource = item.ImageSource;
			FilterObjectType = FilterObjectType.HR;
		}

		public string Name { get; private set; }
		public string ImageSource { get; private set; }
		public XStateClass StateClass { get; private set; }
		public JournalObjectType JournalObjectType { get; private set; }
		public FilterObjectType FilterObjectType { get; private set; }

		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged(() => IsChecked);

				foreach (var child in Children)
				{
					child.IsChecked = value;
				}
				UpdateParent();
			}
		}

		public void UpdateParent()
		{
			if (Parent != null)
			{
				var isAllChecked = Parent.Children.All(x => x.IsChecked);
				Parent.SetIsChecked(isAllChecked);
				Parent.UpdateParent();
			}
		}

		public void SetIsChecked(bool value)
		{
			_isChecked = value;
			OnPropertyChanged(() => IsChecked);
		}
	}

	public enum FilterObjectType
	{
		Subsystem,
		ObjectType,
		Object,
		Camera,
		HR
	}
}