using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using Infrastructure.Common.TreeList;

namespace GKModule.ViewModels
{
	public class ArchiveDeviceViewModel : TreeNodeViewModel<ArchiveDeviceViewModel>, ICheckBoxItem
	{
		public ArchiveDeviceViewModel(XDevice device)
		{
			Device = device;
			Name = device.PresentationName;
		}

		public XDevice Device { get; private set; }
		public string Name { get; private set; }

		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged(() => IsChecked);
				if (ItemsList != null)
					ItemsList.OnPropertyChanged(() => ItemsList.HasCheckedItems);
			}
		}
		
		public CheckBoxItemList ItemsList { get; set; }

		public bool CanCheck
		{
			get { return Device.IsRealDevice; }
		}
	}
}