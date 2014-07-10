using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.TreeList;
using Infrastructure.Common;
using FiresecAPI.Journal;
using System.Reflection;
using FiresecAPI.GK;
using FiresecAPI;
using FiresecAPI.SKD;

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
				EventDescriptionAttribute[] descriptionAttributes = (EventDescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(EventDescriptionAttribute), false);
				if (descriptionAttributes.Length > 0)
				{
					EventDescriptionAttribute eventDescriptionAttribute = descriptionAttributes[0];
					Name = eventDescriptionAttribute.Name;
					JournalSubsystemType = eventDescriptionAttribute.JournalSubsystemType;
					StateClass = eventDescriptionAttribute.StateClass;
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
			switch (journalSubsystemType)
			{
				case JournalSubsystemType.GK:
					ImageSource = "/Controls;component/GKIcons/GK.png";
					break;

				case JournalSubsystemType.SKD:
					ImageSource = "/Controls;component/SKDIcons/Controller.png";
					break;

				case JournalSubsystemType.Video:
					ImageSource = "/Controls;component/Images/Camera.png";
					break;
			}
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

				case JournalObjectType.SKDDevice:
					ImageSource = "/Controls;component/SKDIcons/Controller.png";
					break;

				case JournalObjectType.SKDZone:
					ImageSource = "/Controls;component/Images/Zone.png";
					break;

				case JournalObjectType.VideoDevice:
					ImageSource = "/Controls;component/Images/Camera.png";
					break;
			}
		}

		public FilterObjectViewModel(FiresecAPI.GK.XDevice device)
		{
			Name = device.PresentationName;
			UID = device.UID;
			ImageSource = device.Driver.ImageSource;
		}

		public FilterObjectViewModel(FiresecAPI.GK.XZone zone)
		{
			Name = zone.PresentationName;
			UID = zone.UID;
			ImageSource = "/Controls;component/Images/Zone.png";
		}

		public FilterObjectViewModel(FiresecAPI.GK.XDirection direction)
		{
			Name = direction.PresentationName;
			UID = direction.UID;
			ImageSource = "/Controls;component/Images/BDirection.png";
		}

		public FilterObjectViewModel(FiresecAPI.GK.XMPT mpt)
		{
			Name = mpt.PresentationName;
			UID = mpt.BaseUID;
			ImageSource = "/Controls;component/Images/BMPT.png";
		}

		public FilterObjectViewModel(FiresecAPI.GK.XPumpStation pumpStation)
		{
			Name = pumpStation.PresentationName;
			UID = pumpStation.UID;
			ImageSource = "/Controls;component/Images/BPumpStation.png";
		}

		public FilterObjectViewModel(FiresecAPI.GK.XDelay delay)
		{
			Name = delay.PresentationName;
			UID = delay.BaseUID;
			ImageSource = "/Controls;component/Images/Delay.png";
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
			ImageSource = "/Controls;component/Images/Zone.png";
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



				//if (IsSubsystem)
				//{
				//    foreach (var child in Children)
				//    {
				//        child.IsChecked = value;
				//    }
				//}

				//if (IsObjectGroup)
				//{
				//    foreach (var child in Children)
				//    {
				//        child.SetIsChecked(value);
				//    }
				//}
				
				//if (Parent != null && (Parent.IsSubsystem || Parent.IsObjectGroup))
				//{
				//    Parent.UpdateIsChecked();
				//}
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

		//public void UpdateIsChecked()
		//{
		//    var isAllChecked = Children.All(x => x.IsChecked);
		//    _isChecked = isAllChecked;
		//    OnPropertyChanged(() => IsChecked);

		//    if (Parent != null && (Parent.IsSubsystem || Parent.IsObjectGroup))
		//    {
		//        Parent.UpdateIsChecked();
		//    }
		//}

		public void SetIsChecked(bool value)
		{
			_isChecked = value;
			OnPropertyChanged(() => IsChecked);
		}
	}
}