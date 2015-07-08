﻿using System;
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
					ImageSource = "/Controls;component/GKIcons/RSR2_RM_1.png";
					break;

				case JournalObjectType.GKZone:
					ImageSource = "/Controls;component/Images/Zone.png";
					break;

				case JournalObjectType.GKDirection:
					ImageSource = "/Controls;component/Images/BDirection.png";
					break;

				case JournalObjectType.GKMPT:
					ImageSource = "/Controls;component/Images/BMPT.png";
					break;

				case JournalObjectType.GKPumpStation:
					ImageSource = "/Controls;component/Images/BPumpStation.png";
					break;

				case JournalObjectType.GKDelay:
					ImageSource = "/Controls;component/Images/Delay.png";
					break;

				case JournalObjectType.GKGuardZone:
					ImageSource = "/Controls;component/Images/GuardZone.png";
					break;

				case JournalObjectType.GKSKDZone:
					ImageSource = "/Controls;component/Images/Zone.png";
					break;

				case JournalObjectType.GKDoor:
					ImageSource = "/Controls;component/Images/Door.png";
					break;

				case JournalObjectType.VideoDevice:
					ImageSource = "/Controls;component/Images/Camera.png";
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
			ImageSource = "/Controls;component/Images/Zone.png";
		}

		public FilterObjectViewModel(GKDirection direction)
		{
			Name = direction.PresentationName;
			UID = direction.UID;
			ImageSource = "/Controls;component/Images/BDirection.png";
		}

		public FilterObjectViewModel(GKMPT mpt)
		{
			Name = mpt.PresentationName;
			UID = mpt.UID;
			ImageSource = "/Controls;component/Images/BMPT.png";
		}

		public FilterObjectViewModel(GKPumpStation pumpStation)
		{
			Name = pumpStation.PresentationName;
			UID = pumpStation.UID;
			ImageSource = "/Controls;component/Images/BPumpStation.png";
		}

		public FilterObjectViewModel(GKDelay delay)
		{
			Name = delay.PresentationName;
			UID = delay.UID;
			ImageSource = "/Controls;component/Images/Delay.png";
		}

		public FilterObjectViewModel(GKGuardZone guardZone)
		{
			Name = guardZone.PresentationName;
			UID = guardZone.UID;
			ImageSource = "/Controls;component/Images/GuardZone.png";
		}

		public FilterObjectViewModel(GKSKDZone skdZone)
		{
			Name = skdZone.PresentationName;
			UID = skdZone.UID;
			ImageSource = "/Controls;component/Images/Zone.png";
		}

		public FilterObjectViewModel(GKDoor door)
		{
			Name = door.PresentationName;
			UID = door.UID;
			ImageSource = "/Controls;component/Images/Door.png";
		}

		public FilterObjectViewModel(FiresecAPI.Models.Camera camera)
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