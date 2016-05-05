using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Controls.Converters;
using StrazhAPI.SKD;
using StrazhAPI.SKD.Device;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using StrazhModule.Events;

namespace StrazhModule.ViewModels
{
	public class SearchDeviceViewModel : BaseViewModel
	{
		public SearchDeviceViewModel(SKDDeviceSearchInfo deviceSearchInfo)
		{
			IsEnabled = true;
			IsSelected = false;
			DeviceType = deviceSearchInfo.DeviceType;
			IpAddress = deviceSearchInfo.IpAddress;
			Gateway = deviceSearchInfo.Gateway;
			Port = deviceSearchInfo.Port;
			Mask = deviceSearchInfo.Submask;
		}

		private bool _isEnabled;
		/// <summary>
		/// Доступность функции добавления устройства в конфигурацию. True - устройство отсутствует в конфигурации,
		/// поэтому можно его добавить. False - устройство уже содержится в конфигурации, его дабавление запрещено.
		/// </summary>
		public bool IsEnabled
		{
			get { return _isEnabled; }
			set
			{
				if (_isSelected == value)
					return;
				_isEnabled = value;
				OnPropertyChanged(() => IsEnabled);
			}
		}

		private bool _isSelected;
		/// <summary>
		/// Флаг, сигнализирующий о необходимости добавления устройства в конфигурацию. True - добавить в конфигурацию,
		/// False - не добавлять в конфигурацию.
		/// </summary>
		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				if (_isSelected == value)
					return;
				_isSelected = value;
				OnPropertyChanged(() => IsSelected);
			}
		}

		private string _name;
		/// <summary>
		/// Название устройства. Если не задано, то определяется исходя из типа устройства.
		/// </summary>
		public string Name
		{
			get
			{
				return String.IsNullOrEmpty(_name)
					? new SKDDeviceTypeEnumToAttributeValueConverter().Convert(DeviceType, null, "type", null).ToString()
					: _name;
			}
			set
			{
				if (_name == value)
					return;
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		private SKDDeviceType _deviceType;
		/// <summary>
		/// Тип устройства
		/// </summary>
		public SKDDeviceType DeviceType
		{
			get { return _deviceType; }
			set
			{
				if (_deviceType == value)
					return;
				_deviceType = value;
				OnPropertyChanged(() => DeviceType);
			}
		}

		private string _ipAddress;
		/// <summary>
		/// IP-адрес
		/// </summary>
		public string IpAddress
		{
			get { return _ipAddress; }
			set
			{
				if (_ipAddress == value)
					return;
				_ipAddress = value;
				OnPropertyChanged(() => IpAddress);
			}
		}

		private string _gateway;
		/// <summary>
		/// Шлюз
		/// </summary>
		public string Gateway
		{
			get { return _gateway; }
			set
			{
				if (_gateway == value)
					return;
				_gateway = value;
				OnPropertyChanged(() => Gateway);
			}
		}

		private int _port;
		/// <summary>
		/// Номер порта
		/// </summary>
		public int Port
		{
			get { return _port; }
			set
			{
				if (_port == value)
					return;
				_port = value;
				OnPropertyChanged(() => Port);
			}
		}

		private string _mask;
		/// <summary>
		/// Маска подсети
		/// </summary>
		public string Mask
		{
			get { return _mask; }
			set
			{
				if (_mask == value)
					return;
				_mask = value;
				OnPropertyChanged(() => Mask);
			}
		}
	}
}
