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

namespace FiltersModule.ViewModels
{
	public class FilterNameViewModel : TreeNodeViewModel<FilterNameViewModel>
	{
		public FilterNameViewModel(GlobalEventNameEnum globalEventNameEnum)
		{
			AddCommand = new RelayCommand(OnAdd);
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

		public FilterNameViewModel(GlobalSubsystemType subsystemType)
		{
			SubsystemType = subsystemType;
			IsSubsystem = true;
			Name = subsystemType.ToDescription();
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
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{

		}
	}
}