using System;
using System.Linq;
using System.Reflection;
using Controls.Converters;
using StrazhAPI;
using StrazhAPI.GK;
using StrazhAPI.Journal;
using StrazhAPI.SKD;
using Infrastructure.Common.TreeList;
using StrazhAPI.Models;

namespace SKDModule.Reports.ViewModels
{
	public class SKDObjectViewModel : TreeNodeViewModel<SKDObjectViewModel>
	{
		public Guid UID { get; private set; }

		public SKDObjectViewModel(JournalSubsystemType journalSubsystemType)
		{
			JournalSubsystemType = journalSubsystemType;
			IsSubsystem = true;
			Name = journalSubsystemType.ToDescription();
			var converter = new JournalSubsystemTypeToIconConverter();
			ImageSource = (string)converter.Convert(journalSubsystemType, typeof(JournalSubsystemType), null, null);
		}
		public SKDObjectViewModel(JournalObjectType journalObjectType)
		{
			JournalObjectType = journalObjectType;
			Name = journalObjectType.ToDescription();
			IsObjectGroup = true;
			switch(journalObjectType)
			{
				case JournalObjectType.SKDDevice:
					ImageSource = "/Controls;component/SKDIcons/Controller.png";
					break;

				case JournalObjectType.SKDZone:
					ImageSource = "/Controls;component/Images/Zone.png";
					break;

				case JournalObjectType.SKDDoor:
					ImageSource = "/Controls;component/Images/Door.png";
					break;

				case JournalObjectType.VideoDevice:
					ImageSource = "/Controls;component/Images/Camera.png";
					break;
			}
		}
		public SKDObjectViewModel(SKDDevice device)
		{
			Name = device.Name;
			UID = device.UID;
			ImageSource = device.Driver.ImageSource;
		}
		public SKDObjectViewModel(SKDZone zone)
		{
			Name = zone.Name;
			UID = zone.UID;
			ImageSource = "/Controls;component/Images/Zone.png";
		}
		public SKDObjectViewModel(SKDDoor door)
		{
			Name = door.Name;
			UID = door.UID;
			ImageSource = "/Controls;component/Images/Door.png";
		}
		public SKDObjectViewModel(Camera camera)
		{
			Name = camera.Name;
			UID = camera.UID;
			ImageSource = "/Controls;component/Images/Camera.png";
		}

		public JournalEventNameType JournalEventNameType { get; private set; }
		public string Name { get; private set; }
		public string ImageSource { get; private set; }
		public XStateClass StateClass { get; private set; }
		public JournalSubsystemType JournalSubsystemType { get; private set; }
		public bool IsSubsystem { get; private set; }
		public bool IsObjectGroup { get; private set; }
		public JournalObjectType JournalObjectType { get; private set; }

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
}