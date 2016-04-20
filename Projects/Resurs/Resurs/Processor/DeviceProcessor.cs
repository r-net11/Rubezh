using Infrastructure.Common.Windows.Windows;
using ResursAPI;
using ResursAPI.Models;
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
			_networksManager.NetworkControllerHasError -= OnNetworksManagerNetworkControllerHasError;
			_networksManager.NetworkControllerHasError += OnNetworksManagerNetworkControllerHasError;
		}

		#region NetworksManagerCommands
		public bool AddToMonitoring(Device device)
		{
			return InTryCatch(() =>
			{
				if (device.CanMonitor)
				{
					if (device.DeviceType == DeviceType.Network)
						_networksManager.AddNetwork(device);
					else if (device.DeviceType == DeviceType.Counter)
						_networksManager.AddDevice(device);
				}
			});
		}

		public bool DeleteFromMonitoring(Device device)
		{
			return InTryCatch(() =>
			{
				if (device.CanMonitor)
				{
					if (device.DeviceType == DeviceType.Network)
						_networksManager.RemoveNetwork(device.UID);
					else if (device.DeviceType == DeviceType.Counter)
						_networksManager.RemoveDevice(device.UID);
				}
			});
		}

		public bool SetStatus(Device device, bool isActive)
		{
			return InTryCatch(() => _networksManager.SetSatus(device.UID, isActive));
		}

		public bool WriteParameters(Device device)
		{
			return InTryCatch(() =>
			{
				if (device.CanMonitor)
				{
					foreach (var parameter in device.Parameters.Where(x => !x.DriverParameter.IsReadOnly))
					{
						_networksManager.WriteParameter(device.UID, parameter.DriverParameter.Name, parameter.ValueType);
					}
				}
			});
		}

		public bool WriteParameter(Guid deviceUID, string parameterName, ValueType parameterValue)
		{
			return InTryCatch(() =>
			{
				var device = DbCache.Devices.FirstOrDefault(x => x.UID == deviceUID);
				if (device.CanMonitor)
					_networksManager.WriteParameter(deviceUID, parameterName, parameterValue);
			});
		}

		public bool ReadParameters(Device device)
		{
			return InTryCatch(() =>
			{
				if (device.CanMonitor)
				{
					foreach (var parameter in device.Parameters)
					{
						var result = _networksManager.ReadParameter(device.UID, parameter.DriverParameter.Name);
						if (!result.HasError())
						{
							parameter.SetValue(result.Value);
						}
					}
				}
			});
		}

		public ValueTypeContainer ReadParameter(Device device, string parameterName)
		{
			return InTryCatch(() =>
			{
				if (device.CanMonitor)
				{
					var result = _networksManager.ReadParameter(device.UID, parameterName);
					if (!result.HasError())
					{
						return new ValueTypeContainer { ValueType = result.Value };
					}
				}
				return null;
			}, null);
		}

		public bool SendCommand(Guid guid, string p)
		{
			return InTryCatch(() => _networksManager.SendCommand(guid, p));
		}

		public bool SyncDateTime(Guid deviceUID)
		{
			return InTryCatch(() =>
			{
				var device = DbCache.Devices.FirstOrDefault(x => x.UID == deviceUID);
				if (device.CanMonitor)
					_networksManager.SyncDateTime(deviceUID);
			});
		}
		#endregion

		#region EventHandlers
		public void OnNetworksManagerParameterChanged(object sender, ParameterChangedEventArgs args)
		{
			var device = DbCache.Devices.FirstOrDefault(x => x.UID == args.DeviceId);
			switch (args.ParameterName)
			{
				case (ResursAPI.ParameterNames.ParameterNamesBase.CounterTarif1):
					DbCache.AddMeasure(CreateMeasure(device, args, 0));
					return;
				case (ResursAPI.ParameterNames.ParameterNamesBase.CounterTarif2):
					DbCache.AddMeasure(CreateMeasure(device, args, 1));
					return;
				case (ResursAPI.ParameterNames.ParameterNamesBase.CounterTarif3):
					DbCache.AddMeasure(CreateMeasure(device, args, 2));
					return;
				case (ResursAPI.ParameterNames.ParameterNamesBase.CounterTarif4):
					DbCache.AddMeasure(CreateMeasure(device, args, 3));
					return;
				case (ResursAPI.ParameterNames.ParameterNamesBase.CounterTarif5):
					DbCache.AddMeasure(CreateMeasure(device, args, 4));
					return;
				case (ResursAPI.ParameterNames.ParameterNamesBase.CounterTarif6):
					DbCache.AddMeasure(CreateMeasure(device, args, 5));
					return;
				case (ResursAPI.ParameterNames.ParameterNamesBase.CounterTarif7):
					DbCache.AddMeasure(CreateMeasure(device, args, 6));
					return;
				case (ResursAPI.ParameterNames.ParameterNamesBase.CounterTarif8):
					DbCache.AddMeasure(CreateMeasure(device, args, 7));
					return;
				case (ResursAPI.ParameterNames.ParameterNamesBase.DateTime):
					device.DateTime = (DateTime)args.NewValue;
					return;
				default:
					break;
			}
			var parameter = device.Parameters.FirstOrDefault(x => x.DriverParameter.Name == args.ParameterName);
			if (parameter != null)
			{
				switch (parameter.DriverParameter.ParameterType)
				{
					case ParameterType.Enum:
						parameter.IntValue = (int)args.NewValue;
						break;
					case ParameterType.String:
						parameter.StringValue = ((ParameterStringContainer)args.NewValue).Value;
						break;
					case ParameterType.Int:
						parameter.IntValue = Convert.ToInt32(args.NewValue);
						break;
					case ParameterType.Double:
						parameter.DoubleValue = (double)args.NewValue;
						break;
					case ParameterType.Bool:
						parameter.BoolValue = (bool)args.NewValue;
						break;
					case ParameterType.DateTime:
						parameter.DateTimeValue = (DateTime)args.NewValue;
						break;
					default:
						break;
				}
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

		void OnNetworksManagerNetworkControllerHasError(object sender, ResursNetwork.OSI.ApplicationLayer.NetworkControllerErrorOccuredEventArgs args)
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

		bool InTryCatch(Action action)
		{
			try
			{
				action();
				return true;
			}
			catch (Exception e)
			{
				MessageBoxService.Show(e.Message);
				return false;
			}
		}

		T InTryCatch<T>(Func<T> action, T defaultValue)
		{
			try
			{
				return action();
			}
			catch (Exception e)
			{
				MessageBoxService.Show(e.Message);
				return defaultValue;
			}
		}
	}

	public static class ExtensionMethods
	{
		public static bool HasError(this IOperationResult result)
		{
			return result.Result.ErrorCode != TransactionErrorCodes.NoError;
		}
	}

	public class ValueTypeContainer
	{
		public ValueType ValueType { get; set; }
	}
}