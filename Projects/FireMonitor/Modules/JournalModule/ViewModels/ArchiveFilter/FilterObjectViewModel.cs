﻿using System;
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
		}

		public FilterObjectViewModel(FiresecAPI.GK.XDevice device)
		{
			Name = device.PresentationName;
			ImageSource = device.Driver.ImageSource;
		}

		public GlobalEventNameEnum GlobalEventNameEnum { get; private set; }
		public string Name { get; private set; }
		public string ImageSource { get; private set; }
		public XStateClass StateClass { get; private set; }
		public GlobalSubsystemType SubsystemType { get; private set; }
		public bool IsSubsystem { get; private set; }

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
						child.SetIsChecked(value);
					}
				}
				else if (Parent != null)
				{
					var isAllChecked = Parent.Children.All(x => x.IsChecked == true);
					Parent.SetIsChecked(isAllChecked);
				}
			}
		}

		public void SetIsChecked(bool value)
		{
			_isChecked = value;
			OnPropertyChanged(() => IsChecked);
		}
	}
}