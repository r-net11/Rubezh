using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.TreeList;
using Infrastructure.Common;
using FiresecAPI.Events;
using System.Reflection;
using FiresecAPI.GK;
using FiresecAPI;
using FiresecAPI.SKD;

namespace JournalModule.ViewModels
{
	public class FilterObjectViewModel : TreeNodeViewModel<FilterObjectViewModel>
	{
		public FilterObjectViewModel(GlobalEventNameEnum globalEventNameEnum)
		{
			GlobalEventNameEnum = globalEventNameEnum;

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
					if (StateClass == XStateClass.Norm)
						ImageSource = null;

					ImageSource = "/Controls;component/StateClassIcons/" + StateClass.ToString() + ".png";
				}
			}
			IsSubsystem = false;
		}

		public FilterObjectViewModel(GlobalSubsystemType subsystemType)
		{
			SubsystemType = subsystemType;
			IsSubsystem = true;
			Name = subsystemType.ToDescription();
			switch (subsystemType)
			{
				case GlobalSubsystemType.GK:
					ImageSource = "/Controls;component/GKIcons/GK.png";
					break;

				case GlobalSubsystemType.SKD:
					ImageSource = "/Controls;component/SKDIcons/Controller.png";
					break;

				case GlobalSubsystemType.Video:
					ImageSource = "/Controls;component/Images/Camera.png";
					break;
			}
		}

		public FilterObjectViewModel(SKDJournalItemType journalItemType)
		{
			JournalItemType = journalItemType;
			Name = journalItemType.ToDescription();
			IsObjectGroup = true;
			switch(journalItemType)
			{
				case SKDJournalItemType.GKDevice:
					ImageSource = "/Controls;component/GKIcons/RM_1.png";
					break;

				case SKDJournalItemType.GKZone:
					ImageSource = "/Controls;component/Images/Zone.png";
					break;

				case SKDJournalItemType.GKDirection:
					ImageSource = "/Controls;component/Images/BDirection.png";
					break;

				case SKDJournalItemType.GKMPT:
					ImageSource = "/Controls;component/Images/BMPT.png";
					break;

				case SKDJournalItemType.GKPumpStation:
					ImageSource = "/Controls;component/Images/BPumpStation.png";
					break;

				case SKDJournalItemType.GKDelay:
					ImageSource = "/Controls;component/Images/Delay.png";
					break;

				case SKDJournalItemType.SKDDevice:
					ImageSource = "/Controls;component/SKDIcons/Controller.png";
					break;

				case SKDJournalItemType.SKDZone:
					ImageSource = "/Controls;component/Images/Zone.png";
					break;

				case SKDJournalItemType.VideoDevice:
					ImageSource = "/Controls;component/Images/Camera.png";
					break;
			}
		}

		public FilterObjectViewModel(FiresecAPI.GK.XDevice device)
		{
			Name = device.PresentationName;
			ImageSource = device.Driver.ImageSource;
		}

		public FilterObjectViewModel(FiresecAPI.GK.XZone zone)
		{
			Name = zone.PresentationName;
			ImageSource = "/Controls;component/Images/Zone.png";
		}

		public FilterObjectViewModel(FiresecAPI.GK.XDirection direction)
		{
			Name = direction.PresentationName;
			ImageSource = "/Controls;component/Images/BDirection.png";
		}

		public FilterObjectViewModel(FiresecAPI.GK.XMPT mpt)
		{
			Name = mpt.PresentationName;
			ImageSource = "/Controls;component/Images/BMPT.png";
		}

		public FilterObjectViewModel(FiresecAPI.GK.XPumpStation pumpStation)
		{
			Name = pumpStation.PresentationName;
			ImageSource = "/Controls;component/Images/BPumpStation.png";
		}

		public FilterObjectViewModel(FiresecAPI.GK.XDelay delay)
		{
			Name = delay.PresentationName;
			ImageSource = "/Controls;component/Images/Delay.png";
		}

		public FilterObjectViewModel(SKDDevice device)
		{
			Name = device.Name;
			ImageSource = device.Driver.ImageSource;
		}

		public FilterObjectViewModel(SKDZone zone)
		{
			Name = zone.Name;
			ImageSource = "/Controls;component/Images/Zone.png";
		}

		public FilterObjectViewModel(FiresecAPI.Models.Camera camera)
		{
			Name = camera.Name;
			ImageSource = "/Controls;component/Images/Camera.png";
		}

		public GlobalEventNameEnum GlobalEventNameEnum { get; private set; }
		public string Name { get; private set; }
		public string ImageSource { get; private set; }
		public XStateClass StateClass { get; private set; }
		public GlobalSubsystemType SubsystemType { get; private set; }
		public bool IsSubsystem { get; private set; }
		public bool IsObjectGroup { get; private set; }
		public SKDJournalItemType JournalItemType { get; private set; }

		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged(() => IsChecked);

				if (IsSubsystem)
				{
					foreach (var child in Children)
					{
						child.IsChecked = value;
					}
				}

				if (IsObjectGroup)
				{
					foreach (var child in Children)
					{
						child.SetIsChecked(value);
					}
				}
				
				if (Parent != null && (Parent.IsSubsystem || Parent.IsObjectGroup))
				{
					Parent.UpdateIsChecked();
				}
			}
		}

		public void UpdateIsChecked()
		{
			var isAllChecked = Children.All(x => x.IsChecked);
			_isChecked = isAllChecked;
			OnPropertyChanged(() => IsChecked);

			if (Parent != null && (Parent.IsSubsystem || Parent.IsObjectGroup))
			{
				Parent.UpdateIsChecked();
			}
		}

		public void SetIsChecked(bool value)
		{
			_isChecked = value;
			OnPropertyChanged(() => IsChecked);
		}
	}
}