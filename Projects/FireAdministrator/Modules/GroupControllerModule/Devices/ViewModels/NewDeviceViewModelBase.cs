using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class NewDeviceViewModelBase : SaveCancelDialogViewModel
	{
		public NewDeviceViewModelBase(DeviceViewModel deviceViewModel)
		{
			Title = "Новое устройство";
			ParentDeviceViewModel = deviceViewModel;
			ParentDevice = ParentDeviceViewModel.Device;
			Drivers = new ObservableCollection<XDriver>();
			AvailableShleifs = new ObservableCollection<byte>();
			Count = 1;
		}

		protected DeviceViewModel ParentDeviceViewModel;
		protected XDevice ParentDevice;
		public DeviceViewModel AddedDevice { get; protected set; }
		public ObservableCollection<XDriver> Drivers { get; protected set; }
		public ObservableCollection<byte> AvailableShleifs { get; protected set; }

		int _count;
		public int Count
		{
			get { return _count; }
			set
			{
				_count = value;
				OnPropertyChanged("Count");
			}
		}
	}
}