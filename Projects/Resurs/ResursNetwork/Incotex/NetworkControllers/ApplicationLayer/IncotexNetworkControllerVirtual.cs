using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Specialized;
using ResursNetwork.OSI.ApplicationLayer;
using ResursNetwork.OSI.ApplicationLayer.Devices;
using ResursNetwork.OSI.ApplicationLayer.Devices.Collections.ObjectModel;
using ResursNetwork.OSI.DataLinkLayer;
using ResursNetwork.OSI.Messages;
using ResursNetwork.OSI.Messages.Transactions;
using ResursNetwork.Management;
using ResursNetwork.Incotex.Models;
using ResursNetwork.Networks;
using ResursAPI;
using ResursAPI.Models;
using ResursAPI.CommandNames;
using ResursAPI.ParameterNames;

namespace ResursNetwork.Incotex.NetworkControllers.ApplicationLayer
{
    public class IncotexNetworkControllerVirtual : INetwrokController
    {
        #region Fields And Properties

		const int MIN_POLLING_PERIOD = 1000;

		static DeviceModel[] _suppotedDevices =
			new DeviceModel[] { DeviceModel.VirtualMercury203 };

        Guid _id = Guid.NewGuid();
        DevicesCollection _devices;
        Status _status = Status.Stopped;
        IDataLinkPort _connection;
		CancellationTokenSource _cancellationTokenSource;
		Task _networkPollingTask;
		int _pollingPeriod;
		NetworkControllerErrors _Errors;
		int _requestTimeout = 2000; // Значение по умолчанию

		#region нужен только для отладки

		int _bautRate = 9600;
		int _broadcastRequestDelay = 2000;
		string _portName = "VIRTUAL COM1";
	
		#endregion



        public Guid Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public IEnumerable<DeviceModel> SuppotedDevices
        {
            get { return _suppotedDevices; }
        }

        public DevicesCollection Devices
        {
            get { return _devices; }
        }

        public IDataLinkPort Connection
        {
            get { return _connection; }
            set 
            {                
                if (Status == Status.Running)
                {
                    throw new InvalidOperationException(
                        "Невозможно установить порт, контроллер в активном состоянии");
                }

                if (_connection != null)
                {
                    if (_connection.IsOpen)
                    {
                        throw new InvalidOperationException(
                            "Невозможно установить порт, порт в активном состоянии");
                    }
                    else
                    {
                        _connection = value;
                    }
                }
                else
                {
                    _connection = value;                    
                }
            }
        }

        public Status Status
        {
            get
            {
                return _status;
            }
            set
            {
                if (_status != value)
                {
                    _status = value;

					if (_status == Status.Running)
					{
						if (_cancellationTokenSource == null)
						{
							_cancellationTokenSource = new CancellationTokenSource();
						}
						_networkPollingTask =
							Task.Factory.StartNew(NetwokPollingAction, _cancellationTokenSource.Token);
					}
					else
					{
						try
						{
							_cancellationTokenSource.Cancel();
							_networkPollingTask.Wait();
						}
						catch (AggregateException)
						{
							if (!_networkPollingTask.IsCanceled) throw;
						}
						finally
						{
							_cancellationTokenSource.Dispose();
							_cancellationTokenSource = null;
						}
					}

					OnStatusChanged();
                }
            }
        }

		/// <summary>
        /// Период (мсек) получения данных от удалённых устройтв
        /// </summary>
		public int PollingPeriod
		{
			get { return _pollingPeriod; }
			set 
			{
				if (value >= MIN_POLLING_PERIOD)
				{
					_pollingPeriod = value;
				}
				else
				{
					throw new ArgumentOutOfRangeException("DataSyncPeriod", String.Empty);
				}
			}
		}

		public NetworkControllerErrors Errors
		{
			get { return _Errors; }
		}

		public bool PortError
		{
			get { return _Errors.PortError; }
			set 
			{
 				if (_Errors.PortError != value)
				{
					_Errors.PortError = value;
					OnErrorOccured(
						new NetworkControllerErrorOccuredEventArgs 
						{ 
							Id = this.Id, 
							Errors = _Errors 
						});
				}
			}
		}

        #endregion

        #region Constructors
        
        public IncotexNetworkControllerVirtual()
        {
            _devices = new DevicesCollection(this);
			_devices.CollectionChanged += 
				EventHandler_devices_CollectionChanged;
        }

        #endregion

        #region Methods

		private void EventHandler_devices_CollectionChanged(
			object sender, DevicesCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					{
						OnConfigurationChanged(
							new ConfigurationChangedEventArgs
							{
								Device = e.Device,
								Action = ConfigurationChangedAction.DeviceAdded
							});
						break;
					}
				case NotifyCollectionChangedAction.Remove:
					{
						OnConfigurationChanged(
							new ConfigurationChangedEventArgs
							{
								Device = e.Device,
								Action = ConfigurationChangedAction.DeviceRemoved
							});
						break;
					}
				default:
					{
						throw new NotImplementedException();
					}
			}
		}

		public IAsyncRequestResult Write(
			NetworkRequest request, bool isExternalCall)
		{
			throw new NotImplementedException();
		}

        public void SyncDateTime(ValueType groupAddress)
        {
			uint gadr = Convert.ToUInt32(groupAddress);
			var dt = DateTime.Now;

			var devices = _devices
				.Where(p => p is Mercury203Virtual)
				.Where(l => ((Mercury203Virtual)l).GroupAddress == gadr);
							
			foreach (var device in devices)
			{
				device.Rtc = dt;
			}
        }

        public void Start()
        {
            Status = Status.Running;
        }

        public void Stop()
        {
            Status = Status.Stopped;
        }

		public void Dispose()
		{
			Stop();
		}

        private void OnStatusChanged()
        {
            if (StatusChanged != null)
            {
                StatusChanged(this, new EventArgs());
            }
        }

		private void OnParameterChanged(ParameterChangedEventArgs args)
		{
			if (args == null)
			{
				throw new ArgumentNullException("args", "");
			}

			if (ParameterChanged != null)
			{
				ParameterChanged(this, args);
			}
		}

		private void OnConfigurationChanged(ConfigurationChangedEventArgs args)
		{
			if (args == null)
			{
				throw new ArgumentNullException("args", "");
			}

			if (ConfigurationChanged != null)
			{
				ConfigurationChanged(this, args);
			}
		}

		private void OnErrorOccured(NetworkControllerErrorOccuredEventArgs args)
		{
			if (args == null)
			{
				throw new ArgumentNullException();
			}
			if (ErrorOccurred != null)
			{
				ErrorOccurred(this, args);
			}
		}

		private void NetwokPollingAction(Object cancellationToken)
		{
			Debug.WriteLine("Поток на обработку запущен");

			var nextPolling = DateTime.Now;
			// Симулируем работу счётчика: инкрементируем счётчики тарифов
			var cancel = (CancellationToken)cancellationToken;
			
			if (cancel.IsCancellationRequested)
			{
				return;
			}
			
			cancel.ThrowIfCancellationRequested();


			while(!cancel.IsCancellationRequested)
			{
				while (nextPolling > DateTime.Now)
				{
					Thread.Sleep(300);
				}

				foreach (var device in _devices)
				{
					if (cancel.IsCancellationRequested)
					{
						break;
					}

					if (device.Status == Management.Status.Stopped)
					{
						continue;
					}

					var x = (float)device.Parameters[ParameterNamesMercury203Virtual.CounterTarif1].Value;
					var newValue  = x + 1;
					device.Parameters[ParameterNamesMercury203Virtual.CounterTarif1].Value = newValue;
					OnParameterChanged(new ParameterChangedEventArgs(device.Id, ParameterNamesMercury203Virtual.CounterTarif1,
						newValue));

					if (cancel.IsCancellationRequested)
					{
						break;
					}

					x = (float)device.Parameters[ParameterNamesMercury203Virtual.CounterTarif2].Value;
					newValue = x + 1;
					device.Parameters[ParameterNamesMercury203Virtual.CounterTarif2].Value = newValue;
					OnParameterChanged(new ParameterChangedEventArgs(device.Id, ParameterNamesMercury203Virtual.CounterTarif2,
						newValue));

					if (cancel.IsCancellationRequested)
					{
						break;
					}

					x = (float)device.Parameters[ParameterNamesMercury203Virtual.CounterTarif3].Value;
					newValue = x + 1;
					device.Parameters[ParameterNamesMercury203Virtual.CounterTarif3].Value = newValue;
					OnParameterChanged(new ParameterChangedEventArgs(device.Id, ParameterNamesMercury203Virtual.CounterTarif3,
						newValue));

					if (cancel.IsCancellationRequested)
					{
						break;
					}

					x = (float)device.Parameters[ParameterNamesMercury203Virtual.CounterTarif4].Value;
					newValue = x + 1;
					device.Parameters[ParameterNamesMercury203Virtual.CounterTarif4].Value = newValue;
					OnParameterChanged(new ParameterChangedEventArgs(device.Id, ParameterNamesMercury203Virtual.CounterTarif3,
						newValue));
				}

				nextPolling = DateTime.Now.AddMilliseconds(_pollingPeriod);
			}

			Debug.WriteLine("Поток на обработку остановлен");
		}

		public void ExecuteCommand(string commandName)
		{
			switch (commandName)
			{
				case CommandNamesIncotexNetworkControllerVirtual.SetPortError:
					{
						PortError = true; break; 
					}
				case CommandNamesIncotexNetworkControllerVirtual.ResetPortError:
					{
						PortError = false; break;
					}
				default:
					{
						throw new NotSupportedException(String.Format(
						  "Попытка выполнить неподдерживаемую команду {0}", commandName));
					}
			}
		}

		public OperationResult ReadParameter(string parameterName)
		{
			switch (parameterName)
			{
				case ParameterNamesIncotexNetworkVirtual.BautRate:
					{
						return new OperationResult
						{
							Result =
								new TransactionError 
								{ 
									ErrorCode = TransactionErrorCodes.NoError, 
									Description = String.Empty 
								},
							Value = _bautRate
						};
					}
				case ParameterNamesIncotexNetworkVirtual.BroadcastDelay:
					{
						return new OperationResult
						{
							Result =
								new TransactionError
								{
									ErrorCode = TransactionErrorCodes.NoError,
									Description = String.Empty
								},
							Value = _broadcastRequestDelay
						};
					}
				case ParameterNamesIncotexNetworkVirtual.PollInterval:
					{
						return new OperationResult
						{
							Result =
								new TransactionError
								{
									ErrorCode = TransactionErrorCodes.NoError,
									Description = String.Empty
								},
							Value = _pollingPeriod
						};
					}
				case ParameterNamesIncotexNetworkVirtual.PortName:
					{
						return new OperationResult
						{
							Result =
								new TransactionError
								{
									ErrorCode = TransactionErrorCodes.NoError,
									Description = String.Empty
								},
							Value = new ParameterStringContainer { Value = _portName }
						};
					}
				case ParameterNamesIncotexNetworkVirtual.Timeout:
					{
						return new OperationResult
						{
							Result =
								new TransactionError
								{
									ErrorCode = TransactionErrorCodes.NoError,
									Description = String.Empty
								},
							Value = _requestTimeout
						}; 
					}
				default:
					{
 						throw new InvalidOperationException(String.Format(
							"Ошибка чтения параметра. Параметр {0} не найден", parameterName));
					}
			}
		}

		public void WriteParameter(string parameterName, ValueType value)
		{
			switch (parameterName)
			{
				case ParameterNamesIncotexNetworkVirtual.BautRate:
					{
						_bautRate = (int)value; break;
					}
				case ParameterNamesIncotexNetworkVirtual.BroadcastDelay:
					{
						_broadcastRequestDelay = (int)value; break;
					}
				case ParameterNamesIncotexNetworkVirtual.PollInterval:
					{
						_pollingPeriod = (int)value; break;
					}
				case ParameterNamesIncotexNetworkVirtual.PortName:
					{
						_portName = ((ParameterStringContainer)value).Value; break;
					}
				case ParameterNamesIncotexNetworkVirtual.Timeout:
					{
						_requestTimeout = (int)value; break;
					}
				default:
					{
						throw new InvalidOperationException(String.Format(
							"Ошибка записи параметра. Параметр {0} не найден", parameterName));
					}
			}
		}

        #endregion

        #region Events

		public event EventHandler StatusChanged;
		public event EventHandler<NetworkRequestCompletedArgs> NetwrokRequestCompleted;
		public event EventHandler<ParameterChangedEventArgs> ParameterChanged;
		public event EventHandler<ConfigurationChangedEventArgs> ConfigurationChanged;
		public event EventHandler<NetworkControllerErrorOccuredEventArgs> ErrorOccurred;

		#endregion
	}
}
