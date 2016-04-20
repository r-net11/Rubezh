using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Ribbon;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Infrastructure.ViewModels;

namespace OPCModule.ViewModels
{
	public class OPCDevicesViewModel : MenuViewPartViewModel, ISelectable<Guid>
	{
		public OPCDevicesViewModel()
		{
			ConvertCommand = new RelayCommand(OnConvert);
			SetRibbonItems();
		}

		public void Initialize()
		{
			BuildTree();
			if (RootDevice != null)
			{
				RootDevice.IsExpanded = true;
				SelectedDevice = RootDevice;
			}
			OnPropertyChanged(() => RootDevices);
		}

		#region DeviceSelection
		public List<OPCDeviceViewModel> AllDevices;

		public void FillAllDevices()
		{
			AllDevices = new List<OPCDeviceViewModel>();
			AddChildPlainDevices(RootDevice);
		}

		void AddChildPlainDevices(OPCDeviceViewModel parentViewModel)
		{
			AllDevices.Add(parentViewModel);
			foreach (var childViewModel in parentViewModel.Children)
				AddChildPlainDevices(childViewModel);
		}

		public void Select(Guid deviceUID)
		{
			if (deviceUID != Guid.Empty)
			{
				var deviceViewModel = AllDevices.FirstOrDefault(x => x.Device.UID == deviceUID);
				if (deviceViewModel != null)
					deviceViewModel.ExpandToThis();
				SelectedDevice = deviceViewModel;
			}
		}
		#endregion

		OPCDeviceViewModel _selectedDevice;
		public OPCDeviceViewModel SelectedDevice
		{
			get { return _selectedDevice; }
			set
			{
				_selectedDevice = value;
				if (value != null)
					value.ExpandToThis();
				OnPropertyChanged(() => SelectedDevice);
			}
		}

		OPCDeviceViewModel _rootDevice;
		public OPCDeviceViewModel RootDevice
		{
			get { return _rootDevice; }
			private set
			{
				_rootDevice = value;
				OnPropertyChanged(() => RootDevice);
			}
		}

		public OPCDeviceViewModel[] RootDevices
		{
			get { return new OPCDeviceViewModel[] { RootDevice }; }
		}

		void BuildTree()
		{
			RootDevice = AddDeviceInternal(FiresecManager.FiresecConfiguration.DeviceConfiguration.RootDevice, null);
			FillAllDevices();
		}

		private OPCDeviceViewModel AddDeviceInternal(Device device, OPCDeviceViewModel parentDeviceViewModel)
		{
			var deviceViewModel = new OPCDeviceViewModel(device);
			if (parentDeviceViewModel != null)
				parentDeviceViewModel.AddChild(deviceViewModel);

			foreach (var childDevice in device.Children)
				AddDeviceInternal(childDevice, deviceViewModel);
			return deviceViewModel;
		}

		public override void OnShow()
		{
			//Initialize();
			base.OnShow();
		}

		public RelayCommand ConvertCommand { get; private set; }
		void OnConvert()
		{
			var fs1ConvertationHelper = new FS1ConvertationHelper();
			fs1ConvertationHelper.ConvertConfiguration();
			Initialize();
		}

		void SetRibbonItems()
		{
			RibbonItems = new List<RibbonMenuItemViewModel>()
			{
				new RibbonMenuItemViewModel("Редактирование", new ObservableCollection<RibbonMenuItemViewModel>()
				{
					new RibbonMenuItemViewModel("Конвертировать из Firesec-1", ConvertCommand, false, "BSettings")
				}, "BEdit") { Order = 2 }
			};
		}
	}
}