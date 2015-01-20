using System;
using System.Linq;
using System.Reflection;
using Controls.Converters;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using FiresecAPI.SKD;
using Infrastructure.Common.TreeList;

namespace JournalModule.ViewModels
{
	public class FilterObjectViewModel : TreeNodeViewModel<FilterObjectViewModel>
	{
		public Guid UID { get; private set; }

		public FilterObjectViewModel(JournalEventNameType journalEventNameType)
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

					ImageSource = "/Controls;component/StateClassIcons/" + StateClass.ToString() + ".png";
				}
			}
			IsSubsystem = false;
		}

		public FilterObjectViewModel(JournalSubsystemType journalSubsystemType)
		{
			JournalSubsystemType = journalSubsystemType;
			IsSubsystem = true;
			Name = journalSubsystemType.ToDescription();
			var converter = new JournalSubsystemTypeToIconConverter();
			ImageSource = (string)converter.Convert(journalSubsystemType, typeof(JournalSubsystemType), null, null);
		}

		public FilterObjectViewModel(JournalObjectType journalObjectType)
		{
			JournalObjectType = journalObjectType;
			Name = journalObjectType.ToDescription();
			IsObjectGroup = true;
			switch(journalObjectType)
			{
				case JournalObjectType.GKDevice:
					ImageSource = "/Controls;component/GKIcons/RM_1.png";
					break;

				case JournalObjectType.GKZone:
					ImageSource = "Zone";
					break;

				case JournalObjectType.GKDirection:
					ImageSource = "BDirection";
					break;

				case JournalObjectType.GKMPT:
					ImageSource = "BMPT";
					break;

				case JournalObjectType.GKPumpStation:
					ImageSource = "BPumpStation";
					break;

				case JournalObjectType.GKDelay:
					ImageSource = "Delay";
					break;

				case JournalObjectType.GKGuardZone:
					ImageSource = "GuardZone";
					break;

				case JournalObjectType.GKDoor:
					ImageSource = "Door";
					break;

				case JournalObjectType.SKDDevice:
					ImageSource = "/Controls;component/SKDIcons/Controller.png";
					break;

				case JournalObjectType.SKDZone:
					ImageSource = "Zone";
					break;

				case JournalObjectType.SKDDoor:
					ImageSource = "Door";
					break;

				case JournalObjectType.VideoDevice:
					ImageSource = "Camera";
					break;
			}
		}

		public FilterObjectViewModel(GKDevice device)
		{
			Name = device.PresentationName;
			UID = device.UID;
			ImageSource = device.Driver.ImageSource;
		}

		public FilterObjectViewModel(GKZone zone)
		{
			Name = zone.PresentationName;
			UID = zone.UID;
			ImageSource = "Zone";
		}

		public FilterObjectViewModel(GKDirection direction)
		{
			Name = direction.PresentationName;
			UID = direction.UID;
			ImageSource = "BDirection";
		}

		public FilterObjectViewModel(GKMPT mpt)
		{
			Name = mpt.PresentationName;
			UID = mpt.UID;
			ImageSource = "BMPT";
		}

		public FilterObjectViewModel(GKPumpStation pumpStation)
		{
			Name = pumpStation.PresentationName;
			UID = pumpStation.UID;
			ImageSource = "BPumpStation";
		}

		public FilterObjectViewModel(GKDelay delay)
		{
			Name = delay.PresentationName;
			UID = delay.UID;
			ImageSource = "Delay";
		}

		public FilterObjectViewModel(GKGuardZone guardZone)
		{
			Name = guardZone.PresentationName;
			UID = guardZone.UID;
			ImageSource = "GuardZone";
		}

		public FilterObjectViewModel(GKDoor door)
		{
			Name = door.PresentationName;
			UID = door.UID;
			ImageSource = "Door";
		}

		public FilterObjectViewModel(SKDDevice device)
		{
			Name = device.Name;
			UID = device.UID;
			ImageSource = device.Driver.ImageSource;
		}

		public FilterObjectViewModel(SKDZone zone)
		{
			Name = zone.Name;
			UID = zone.UID;
			ImageSource = "Zone";
		}

		public FilterObjectViewModel(SKDDoor door)
		{
			Name = door.Name;
			UID = door.UID;
			ImageSource = "Door";
		}

		public FilterObjectViewModel(FiresecAPI.Models.Camera camera)
		{
			Name = camera.Name;
			UID = camera.UID;
			ImageSource = "Camera";
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