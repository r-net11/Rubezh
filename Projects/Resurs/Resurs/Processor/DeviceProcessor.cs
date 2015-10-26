﻿using Infrastructure.Common.Windows;
using ResursAPI;
using ResursDAL;
using ResursNetwork.OSI.ApplicationLayer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Resurs.Processor
{
	public class DeviceProcessor
	{
		ResursNetwork.Networks.NetworksManager _networksManager;

		static DeviceProcessor _instance;

		public static DeviceProcessor Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new DeviceProcessor();
				}
				return _instance;
			}
		}


		public DeviceProcessor()
		{
			_networksManager = ResursNetwork.Networks.NetworksManager.Instance;
			_networksManager.StatusChanged -= OnNetworksManagerStatusChanged;
			_networksManager.StatusChanged += OnNetworksManagerStatusChanged;
			_networksManager.ParameterChanged -= OnNetworksManagerParameterChanged;
			_networksManager.ParameterChanged += OnNetworksManagerParameterChanged;
			_networksManager.DeviceHasError -= OnNetworksManagerDeviceHasError;
			_networksManager.DeviceHasError += OnNetworksManagerDeviceHasError;
		}

		#region NetworksManagerCommands
		public bool AddToMonitoring(Device device)
		{
			try
			{
				if (device.CanMonitor)
				{
					if (device.DeviceType == DeviceType.Network)
						_networksManager.AddNetwork(device);
					else if (device.DeviceType == DeviceType.Counter)
						_networksManager.AddDevice(device);
				}
				return true;
			}
			catch (Exception e)
			{
				MessageBoxService.Show(e.Message);
				return false;
			}
		}

		public bool DeleteFromMonitoring(Device device)
		{
			try
			{
				if (device.CanMonitor)
				{
					if (device.DeviceType == DeviceType.Network)
						_networksManager.RemoveNetwork(device.UID);
					else if (device.DeviceType == DeviceType.Counter)
						_networksManager.RemoveDevice(device.UID);
				}
				return true;
			}
			catch (Exception e)
			{
				MessageBoxService.Show(e.Message);
				return false;
			}
		}

		public bool SetStatus(Device device, bool isActive)
		{
			try
			{
				_networksManager.SetSatus(device.UID, isActive);
				return true;
			}
			catch (Exception e)
			{
				MessageBoxService.Show(e.Message);
				return false;
			}
		}

		public bool WriteParameters(Device device)
		{
			return true;
		}

		public bool WriteParameter(Guid deviceUID, string parameterName, ValueType parameterValue)
		{
			return true;
		}

		public bool SendCommand(Guid guid, string p)
		{
			try
			{
				_networksManager.SendCommand(guid, p);
				return true;
			}
			catch (Exception e)
			{
				MessageBoxService.Show(e.Message);
				return false;
			}
		}

		public bool SyncDateTime(Guid deviceUID)
		{
			try
			{
				var device = DBCash.Devices.FirstOrDefault(x => x.UID == deviceUID);
				if (!device.CanMonitor)
					return true;
				if(device.DeviceType == DeviceType.Network)
					_networksManager.SyncDateTime(deviceUID);
				return true;
			}
			catch (Exception e)
			{
				MessageBoxService.Show(e.Message);
				return false;
			}
		}
		#endregion

		#region EventHandlers
		public void OnNetworksManagerParameterChanged(object sender, ParameterChangedEventArgs args)
		{
			var device = DBCash.Devices.FirstOrDefault(x => x.UID == args.DeviceId);
			switch (args.ParameterName)
			{
				case (ResursAPI.ParameterNames.ParameterNamesBase.CounterTarif1):
					DBCash.AddMeasure(CreateMeasure(device, args, 0));
					break;
				case (ResursAPI.ParameterNames.ParameterNamesBase.CounterTarif2):
					DBCash.AddMeasure(CreateMeasure(device, args, 1));
					break;
				case (ResursAPI.ParameterNames.ParameterNamesBase.CounterTarif3):
					DBCash.AddMeasure(CreateMeasure(device, args, 2));
					break;
				case (ResursAPI.ParameterNames.ParameterNamesBase.CounterTarif4):
					DBCash.AddMeasure(CreateMeasure(device, args, 3));
					break;
				case (ResursAPI.ParameterNames.ParameterNamesBase.CounterTarif5):
					DBCash.AddMeasure(CreateMeasure(device, args, 4));
					break;
				case (ResursAPI.ParameterNames.ParameterNamesBase.CounterTarif6):
					DBCash.AddMeasure(CreateMeasure(device, args, 5));
					break;
				case (ResursAPI.ParameterNames.ParameterNamesBase.CounterTarif7):
					DBCash.AddMeasure(CreateMeasure(device, args, 6));
					break;
				case (ResursAPI.ParameterNames.ParameterNamesBase.CounterTarif8):
					DBCash.AddMeasure(CreateMeasure(device, args, 7));
					break;
				default:
					break;
			}
		}

		public void OnNetworksManagerStatusChanged(object sender, ResursNetwork.Networks.StatusChangedEventArgs args)
		{
			var handler = IsActiveChanged;

			if (handler != null)
			{
				var isActiveChangedEventArgs = new IsActiveChangedEventArgs(args);
				handler(this, isActiveChangedEventArgs);
			}
		}

		void OnNetworksManagerDeviceHasError(object sender, ResursNetwork.OSI.ApplicationLayer.Devices.DeviceErrorOccuredEventArgs args)
		{
			var handler = ErrorsChanged;

			if (handler != null)
			{
				var isActiveChangedEventArgs = new ErrorsChangedEventArgs(args);
				handler(this, isActiveChangedEventArgs);
			}
		}

		Measure CreateMeasure(Device device, ParameterChangedEventArgs args, int tariffPartNo)
		{
			//TODO
			//Probably need to store discounted value separate from main
			float currentValue = Convert.ToSingle(args.NewValue);
			//float previousValue = DBCash.GetLastMeasure(device.UID).Value;
			float split = currentValue;// -previousValue;
			float thresholdOverflow = 0;

			double? moneyValue = null;
			if (device.Tariff != null)
			{
				var tariffParts = device.Tariff.TariffParts;
				// Тарифные интервалы не отсортированы по StartTime в Tariff. .OrderBy(x => x.StartTime) - Вероятно, лишнее.
				var tariffPart = device.Tariff.TariffParts.OrderBy(x => x.StartTime).ElementAt(tariffPartNo);
				
				//Проверяем льготный порог
				if (tariffPart.Threshold > 0)
				{
					//Если льготный порог больше или равен количеству потреблённого ресурса
					if (tariffPart.Threshold >= split)
					{
						moneyValue = split * tariffPart.Discount;
						tariffPart.Discount -= split;
					} 
					else
					{
						thresholdOverflow = (float)((double)split - tariffPart.Threshold);
						moneyValue += tariffPart.Threshold * tariffPart.Discount;
						tariffPart.Threshold = 0;
						moneyValue += thresholdOverflow * tariffPart.Price;
					}
				}
			}
			return new Measure
			{
				DateTime = DateTime.Now,
				DeviceUID = device.UID,
				MoneyValue = moneyValue,
				TariffPartNo = tariffPartNo,
				Value = currentValue,
				Split = split,
			};
		}
		#endregion

		#region Events
		public event EventHandler<IsActiveChangedEventArgs> IsActiveChanged;
		public event EventHandler<ErrorsChangedEventArgs> ErrorsChanged;
		#endregion
	}
}