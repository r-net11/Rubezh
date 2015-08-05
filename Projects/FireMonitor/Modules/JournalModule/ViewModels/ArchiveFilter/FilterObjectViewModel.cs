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
		public FilterObjectViewModel(JournalSubsystemType journalSubsystemType)
		{
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

		public FilterObjectViewModel (GKBase device)
		{
			Name = device.PresentationName;
			UID = device.UID;
			ImageSource = device.ImageSource;
		}

		public FilterObjectViewModel(FiresecAPI.Models.Camera camera)
		{
			Name = camera.Name;
			UID = camera.UID;
			ImageSource = "/Controls;component/Images/Camera.png";
		}

		public string Name { get; private set; }
		public string ImageSource { get; private set; }
		public XStateClass StateClass { get; private set; }
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