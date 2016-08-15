using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure.Common.Services;
using StrazhAPI.SKD;
using StrazhAPI.SKD.Device;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using StrazhModule.Events;

namespace StrazhModule.ViewModels
{
	public class SearchDevicesViewModel : SaveCancelDialogViewModel
	{
		private readonly SKDDevice _parentDevice;

		public SearchDevicesViewModel(DeviceViewModel parentDeviceViewModel)
		{
			Title = "Автопоиск устройств";
			_parentDevice = parentDeviceViewModel.Device;
			StartSearchCommand = new RelayCommand(OnStartSearch);
			Devices = new ObservableCollection<SearchDeviceViewModel>();
			AddedDevices = new List<SKDDevice>();
			ServiceFactoryBase.Events.GetEvent<SKDSearchDeviceEvent>().Unsubscribe(OnNewSearchDevice);
			ServiceFactoryBase.Events.GetEvent<SKDSearchDeviceEvent>().Subscribe(OnNewSearchDevice);
			StartSearchDevices();
		}

		public RelayCommand StartSearchCommand { get; private set; }
		private void OnStartSearch()
		{
			Devices.Clear();
			StopSearchDevices();
			StartSearchDevices();
		}

		private void StartSearchDevices()
		{
			FiresecManager.FiresecService.SKDStartSearchDevices();
		}

		private void StopSearchDevices()
		{
			FiresecManager.FiresecService.SKDStopSearchDevices();
		}

		public ObservableCollection<SearchDeviceViewModel> Devices { get; private set; }

		public List<SKDDevice> AddedDevices;

		/// <summary>
		/// Признак того, что нашлись устройства, находящиеся в разных подсетях с сервером
		/// </summary>
		public bool HasDevicesFromDifferentSubnet
		{
			get { return Devices.Any(d => d.IsFromDifferentSubnet); }
		}

		public override void OnClosed()
		{
			StopSearchDevices();
			base.OnClosed();
		}

		private void OnNewSearchDevice(List<SKDDeviceSearchInfo> deviceSearchInfos)
		{
			foreach (var deviceSearchInfo in deviceSearchInfos)
			{
				// Предотвращаем повторное добавление устройства в список
				if (Devices.Any(x => x.IpAddress == deviceSearchInfo.IpAddress))
					continue;
				
				var device = new SearchDeviceViewModel(deviceSearchInfo);
				device.IsFromDifferentSubnet = !CheckDeviceSubnetEqualityToHost(device);
				var deviceInConfig = _parentDevice.Children.FirstOrDefault(x => x.Address == device.IpAddress);
				
				// Если найденное устройство уже содержится в конфигурации, то 
				if (deviceInConfig != null)
				{
					// отключаем возможность его повторного добавления в конфигурацию
					device.IsSelected = true;
					device.IsEnabled = false;
					
					// наследуем параметры устройства из конфигурации
					device.Name = deviceInConfig.Name;
				}
				
				Devices.Add(device);
				OnPropertyChanged(() => HasDevicesFromDifferentSubnet);
			}
		}

		private bool CheckDeviceSubnetEqualityToHost(SearchDeviceViewModel device)
		{
			var hostIps = NetworkHelper.GetIp4NetworkInterfacesInfo();
			return hostIps.Any(hostIp => NetworkHelper.IsSubnetEqual(device.IpAddress, device.Mask, hostIp.Address.ToString(), hostIp.IPv4Mask.ToString()));
		}

		private SKDDriverType? GetDriverType(SKDDeviceType deviceType)
		{
			SKDDriverType? res = null;
			
			switch (deviceType)
			{
				case SKDDeviceType.DahuaBsc1221A:
					res = SKDDriverType.ChinaController_1;
					break;
				case SKDDeviceType.DahuaBsc1201B:
					res = SKDDriverType.ChinaController_2;
					break;
				case SKDDeviceType.DahuaBsc1202B:
					res = SKDDriverType.ChinaController_4;
					break;
			}
			return res;
		}

		private string GenerateDeviceName(List<SKDDevice> devices, string defaultName)
		{
			if (!devices.Any(x => x.Name.ToUpperInvariant() == defaultName.ToUpperInvariant()))
				return defaultName;

			string newName;
			var i = 0;
			do
			{
				i++;
				newName = String.Format("{0} ({1})", defaultName, i);
			} while (devices.Any(x => x.Name.ToUpperInvariant() == newName.ToUpperInvariant()));
			return newName;
		}

		private SKDDevice CreateDevice(SearchDeviceViewModel deviceViewModel)
		{
			var driverType = GetDriverType(deviceViewModel.DeviceType);
			if (driverType.HasValue)
			{
				var driver = SKDManager.Drivers.FirstOrDefault(x => x.IsController && x.DriverType == driverType.Value);

				var device = new SKDDevice()
				{
					Driver = driver,
					DriverUID = driver.UID,
					Name = GenerateDeviceName(_parentDevice.Children, "Контроллер"),
					Parent = _parentDevice
				};

				foreach (var autocreationItem in driver.AutocreationItems)
				{
					var childDriver = SKDManager.Drivers.FirstOrDefault(x => x.DriverType == autocreationItem.DriverType);

					for (var i = 0; i < autocreationItem.Count; i++)
					{
						var childDevice = new SKDDevice()
						{
							Driver = childDriver,
							DriverUID = childDriver.UID,
							IntAddress = i,
							Name = String.Format("{0} {1}", childDriver.Name, i + 1),
							Parent = device
						};
						device.Children.Add(childDevice);
					}
				}

				foreach (var driverProperty in device.Driver.Properties)
				{
					var property = device.Properties.FirstOrDefault(x => x.Name == driverProperty.Name);
					if (property == null)
					{
						property = new SKDProperty
						{
							DriverProperty = driverProperty,
							Name = driverProperty.Name,
							Value = driverProperty.Default,
							StringValue = driverProperty.StringDefault
						};
						device.Properties.Add(property);
					}
					switch (property.Name.ToUpperInvariant())
					{
						case "ADDRESS":
							property.StringValue = deviceViewModel.IpAddress;
							break;
						case "GATEWAY":
							property.StringValue = deviceViewModel.Gateway;
							break;
						case "PORT":
							property.Value = deviceViewModel.Port;
							break;
						case "MASK":
							property.StringValue = deviceViewModel.Mask;
							break;
					}
				}

				return device;
			}

			return null;
		}

		protected override bool Save()
		{
			// Добавляем в конфигурацию все выбранные устройства
			foreach (var device in Devices.Where(x => x.IsEnabled && x.IsSelected))
			{
				var createdDevice = CreateDevice(device);
				_parentDevice.Children.Add(createdDevice);
				AddedDevices.Add(createdDevice);
			}
			// Обновляем конфигурацию
			SKDManager.SKDConfiguration.Update();
			
			return true;
		}
	}
}
