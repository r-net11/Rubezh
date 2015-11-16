using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Timers;
using ResursNetwork.OSI.ApplicationLayer;
using ResursNetwork.OSI.ApplicationLayer.Devices;
using ResursNetwork.OSI.ApplicationLayer.Devices.Collections.ObjectModel;
using ResursNetwork.OSI.ApplicationLayer.Devices.ValueConverters;
using ResursNetwork.Incotex.Models;
using ResursNetwork.Management;
using ResursNetwork.OSI.Messages.Transactions;
using ResursAPI.Models;
using ResursAPI.ParameterNames;
using ResursAPI.CommandNames;

namespace ResursNetwork.Incotex.Models
{
	public class Mercury203Virtual: IDevice, IDisposable
	{
		#region Fields And Properties

		ParatemersCollection _parameters = new ParatemersCollection();
		Status _status = Status.Stopped;
		DeviceErrors _errors;
		INetwrokController _networkController;
		Timer _timer;


        public Guid Id
        {
            get
            {
				return (Guid)_parameters[ParameterNamesMercury203.Id].Value;
            }
            set
            {
				var id = (Guid)_parameters[ParameterNamesMercury203.Id.ToString()].Value;

				if (id != value)
				{
					_parameters[ParameterNamesMercury203.Id.ToString()].Value = value;
					//OnPropertyChanged("Id");
				}
            }
        }

        public DeviceModel DeviceModel
        {
            get { return DeviceModel.VirtualMercury203; }
        }

        public uint Address
        {
            get { return (UInt32)_parameters[ParameterNamesMercury203.Address].Value; }
			set 
			{
				if (value == 0)
				{
					throw new ArgumentOutOfRangeException(
						"Попытка установить недопустимый адрес равеный 0");
				}

				var address = (UInt32)_parameters[ParameterNamesMercury203.Address].Value;

				if (address != value)
				{
					_parameters[ParameterNamesMercury203.Address].Value = value;
					//OnPropertyChanged("Address");
				}
			}
        }

		public uint GroupAddress
		{
			get { return Convert.ToUInt32(_parameters[ParameterNamesMercury203Virtual.GADDR].Value); }
			set 
			{
				if (value == 0)
				{
					throw new ArgumentOutOfRangeException(
						"Групповой адрес должен быть больше 0");
				}
				_parameters[ParameterNamesMercury203Virtual.GADDR].Value = value; 
			}
		}

        public INetwrokController Network
        {
            get { return _networkController; }
			set { _networkController = value; }
        }

        public ParatemersCollection Parameters
        {
            get { return _parameters; }
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
					OnStatusChanged();
				}
            }
        }

        public DeviceErrors Errors
        {
            get { return _errors; }
        }

        public bool CommunicationError
        {
            get
            {
                return _errors.CommunicationError;
            }
            set
            {
				if (_errors.CommunicationError != value)
				{
					_errors.CommunicationError = value;
					OnErrorOccurred(new DeviceErrorOccuredEventArgs 
					{
 						Id = this.Id,
						Errors = this.Errors 
					});
				}
            }
        }

        public bool ConfigurationError
        {
            get
            {
                return _errors.ConfigurationError;
            }
			set
			{
				if (_errors.ConfigurationError != value)
				{
					_errors.ConfigurationError = value;
					OnErrorOccurred(new DeviceErrorOccuredEventArgs 
					{
 						Id = this.Id,
						Errors = this.Errors 
					});
				}
			}
        }

        public bool RtcError
        {
            get
            {
                return _errors.RTCError;
            }
			set
			{
				if (_errors.RTCError != value)
				{
					_errors.RTCError = value;
					OnErrorOccurred(new DeviceErrorOccuredEventArgs 
					{
 						Id = this.Id,
						Errors = this.Errors 
					});
				}
			}
        }

		public System.DateTime Rtc
		{
			get 
			{ 
				return ((IncotexDateTime)_parameters[ParameterNamesMercury203Virtual
				.DateTime].Value).ToDateTime(); 
			}
			set
			{
				_parameters[ParameterNamesMercury203Virtual.DateTime].Value = 
					IncotexDateTime.FromDateTime(value);
			}
		}
		
		#endregion

		#region Constructors
		
		public Mercury203Virtual()
		{
			Initialization();
			_timer = new Timer { Interval = 1000, AutoReset = true };
			_timer.Elapsed += EventHandler_timer_Elapsed;
			_timer.Start();
		}

		#endregion

		#region Methods
		
		void EventHandler_timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			// Обновляем часы счётчика
			var prm = _parameters[ParameterNamesMercury203Virtual.DateTime];
			var dt = IncotexDateTime.FromIncotexDateTime((IncotexDateTime)prm.Value).AddSeconds(1);
			prm.Value = IncotexDateTime.FromDateTime(dt);
		}

		public void Dispose()
		{
			if (_timer != null)
			{
				_timer.Stop();
				_timer.Dispose();
			}
		}

		/// <summary>
		/// Инициализирует список свойств для конкретного устройства
		/// </summary>
		private void Initialization()
		{
			_errors.Reset();

			_parameters = new ParatemersCollection();

			_parameters.Add(new Parameter(typeof(Guid))
			{
				Name = ParameterNamesMercury203Virtual.Id,
				Description = "Сетевой адрес устройства",
				PollingEnabled = false,
				ReadOnly = false,
				ValueConverter = null,
				Value = Guid.NewGuid()
			});

			_parameters.Add(new Parameter(typeof(UInt32))
			{
				Name = ParameterNamesMercury203Virtual.Address,
				Description = "Сетевой адрес устройтсва",
				PollingEnabled = false,
				ReadOnly = false,
				ValueConverter = null,
				Value = (UInt32)1
			});

			_parameters.Add(new Parameter(typeof(UInt32))
			{
				Name = ParameterNamesMercury203Virtual.GADDR,
				Description = "Групповой адрес счётчика",
				PollingEnabled = true,
				ReadOnly = false,
				ValueConverter = null,
				Value = (UInt32)0
			});

			_parameters.Add(new Parameter(typeof(IncotexDateTime))
			{
				Name = ParameterNamesMercury203Virtual.DateTime,
				Description = "Текущее значение часов счётчика",
				PollingEnabled = true,
				ReadOnly = false,
				ValueConverter = new IncotexDataTimeTypeConverter(),
				Value = IncotexDateTime.FromDateTime(DateTime.Now)
			});

			_parameters.Add(new Parameter(typeof(UInt16))
			{
				Name = ParameterNamesMercury203Virtual.PowerLimit,
				Description = "Значение лимита мощности",
				PollingEnabled = true,
				ReadOnly = false,
				ValueConverter = null,
				Value = (UInt16)0
			});

			_parameters.Add(new Parameter(typeof(float))
			{
				Name = ParameterNamesMercury203Virtual.CounterTarif1,
				Description = "Счётчик тарифа 1",
				PollingEnabled = true,
				ReadOnly = false,
				ValueConverter = null,
				Value = (float)0
			});

			_parameters.Add(new Parameter(typeof(float))
			{
				Name = ParameterNamesMercury203Virtual.CounterTarif2,
				Description = "Счётчик тарифа 2",
				PollingEnabled = true,
				ReadOnly = false,
				ValueConverter = null,
				Value = (float)0
			});

			_parameters.Add(new Parameter(typeof(float))
			{
				Name = ParameterNamesMercury203Virtual.CounterTarif3,
				Description = "Счётчик тарифа 3",
				PollingEnabled = true,
				ReadOnly = false,
				ValueConverter = null,
				Value = (float)0
			});

			_parameters.Add(new Parameter(typeof(float))
			{
				Name = ParameterNamesMercury203Virtual.CounterTarif4,
				Description = "Счётчик тарифа 4",
				PollingEnabled = true,
				ReadOnly = false,
				ValueConverter = null,
				Value = (float)0
			});
		}

		public void Start()
        {
			Status = Status.Running;
        }

		public void Stop()
        {
            Status = Status.Stopped;
        }

		private void OnStatusChanged()
		{
			if (StatusChanged != null)
			{
				StatusChanged(this, new EventArgs());
			}
		}

		private void OnErrorOccurred(DeviceErrorOccuredEventArgs args)
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

        #endregion

		#region Network API

		public OperationResult ReadParameter(string parameterName)
		{
			switch (parameterName)
			{
				case ParameterNamesMercury203Virtual.Address:
					{

						return new OperationResult
						{
							Result = new TransactionError 
							{ 
								ErrorCode = TransactionErrorCodes.NoError, 
								Description = String.Empty 
							},
							Value = Address
						};
					}
				case ParameterNamesMercury203Virtual.DateTime:
					{
						return new OperationResult
						{
							Result = new TransactionError
							{
								ErrorCode = TransactionErrorCodes.NoError,
								Description = String.Empty
							},
							Value = ReadDateTime()
						};
					}
				case ParameterNamesMercury203Virtual.GADDR:
					{
						return new OperationResult
						{
							Result = new TransactionError
							{
								ErrorCode = TransactionErrorCodes.NoError,
								Description = String.Empty
							},
							Value = ReadGroupAddress()
						};
					}
				case ParameterNamesMercury203Virtual.PowerLimit:
					{
						return new OperationResult
						{
							Result = new TransactionError
							{
								ErrorCode = TransactionErrorCodes.NoError,
								Description = String.Empty
							},
							Value = ReadPowerLimit()
						};
					}
				case ParameterNamesMercury203Virtual.PowerLimitPerMonth:
					{
 						return new OperationResult
						{
							Result = new TransactionError
							{
								ErrorCode = TransactionErrorCodes.NoError,
								Description = String.Empty
							},
							Value = ReadPowerLimitPerMonth()
						};
					}
				case ParameterNamesMercury203Virtual.CounterTarif1:
					{
						return new OperationResult
						{
							Result = new TransactionError
							{
								ErrorCode = TransactionErrorCodes.NoError,
								Description = String.Empty
							},
							Value = _parameters[ParameterNamesMercury203Virtual.CounterTarif1].Value
						};
					}
				case ParameterNamesMercury203Virtual.CounterTarif2:
					{
						return new OperationResult
						{
							Result = new TransactionError
							{
								ErrorCode = TransactionErrorCodes.NoError,
								Description = String.Empty
							},
							Value = _parameters[ParameterNamesMercury203Virtual.CounterTarif2].Value
						};
					}
				case ParameterNamesMercury203Virtual.CounterTarif3:
					{
						return new OperationResult
						{
							Result = new TransactionError
							{
								ErrorCode = TransactionErrorCodes.NoError,
								Description = String.Empty
							},
							Value = _parameters[ParameterNamesMercury203Virtual.CounterTarif3].Value
						};
					}
				case ParameterNamesMercury203Virtual.CounterTarif4:
					{
						return new OperationResult
						{
							Result = new TransactionError
							{
								ErrorCode = TransactionErrorCodes.NoError,
								Description = String.Empty
							},
							Value = _parameters[ParameterNamesMercury203Virtual.CounterTarif4].Value
						};
					}
				default:
					{
						throw new NotSupportedException(String.Format(
							"Чтение параметра {0} не поддерживается", parameterName));
					}
			}
		}

		public OperationResult WriteParameter(string parameterName, ValueType value)
		{
			switch (parameterName)
			{
				case ParameterNamesMercury203Virtual.Address:
					{
						Address = (UInt32)value;
						return new OperationResult
						{
							Result = new TransactionError
							{
								ErrorCode = TransactionErrorCodes.NoError,
								Description = String.Empty
							},
							Value = Address
						};
					}
				case ParameterNamesMercury203Virtual.DateTime:
					{
						WriteDateTime((DateTime)value);

						return new OperationResult
						{
							Result = new TransactionError
							{
								ErrorCode = TransactionErrorCodes.NoError,
								Description = String.Empty
							}
						};
					}
				case ParameterNamesMercury203Virtual.GADDR:
					{
						_parameters[ParameterNamesMercury203Virtual.GADDR].Value =
							(uint)value;

						return new OperationResult
						{
							Result = new TransactionError
							{
								ErrorCode = TransactionErrorCodes.NoError,
								Description = String.Empty
							}
						};
					}
				case ParameterNamesMercury203Virtual.PowerLimit:
					{
						_parameters[ParameterNamesMercury203Virtual.PowerLimit].Value =
							(ushort)value;
						
						return new OperationResult
						{
							Result = new TransactionError
							{
								ErrorCode = TransactionErrorCodes.NoError,
								Description = String.Empty
							}
						};
					}
				case ParameterNamesMercury203Virtual.PowerLimitPerMonth:
					{
						_parameters[ParameterNamesMercury203Virtual.PowerLimit].Value =
							(ushort)value;

						return new OperationResult
						{
							Result = new TransactionError
							{
								ErrorCode = TransactionErrorCodes.NoError,
								Description = String.Empty
							}
						};
					}
				case ParameterNamesMercury203Virtual.CounterTarif1:
					{
						_parameters[ParameterNamesMercury203Virtual.CounterTarif1].Value =
							(float)value;
						
						return new OperationResult
						{
							Result = new TransactionError
							{
								ErrorCode = TransactionErrorCodes.NoError,
								Description = String.Empty
							}
						};
					}
				case ParameterNamesMercury203Virtual.CounterTarif2:
					{
						_parameters[ParameterNamesMercury203Virtual.CounterTarif2].Value =
							(float)value;

						return new OperationResult
						{
							Result = new TransactionError
							{
								ErrorCode = TransactionErrorCodes.NoError,
								Description = String.Empty
							}
						};
					}
				case ParameterNamesMercury203Virtual.CounterTarif3:
					{
						_parameters[ParameterNamesMercury203Virtual.CounterTarif3].Value =
							(float)value;

						return new OperationResult
						{
							Result = new TransactionError
							{
								ErrorCode = TransactionErrorCodes.NoError,
								Description = String.Empty
							}
						};
					}
				case ParameterNamesMercury203Virtual.CounterTarif4:
					{
						_parameters[ParameterNamesMercury203Virtual.CounterTarif4].Value =
							(float)value;

						return new OperationResult
						{
							Result = new TransactionError
							{
								ErrorCode = TransactionErrorCodes.NoError,
								Description = String.Empty
							}
						};
					}
				default:
					{
						throw new NotSupportedException(String.Format(
							"Чтение параметра {0} не поддерживается", parameterName));
					}
			}
		}

		public void ExecuteCommand(string commandName)
		{
			if (Status == Management.Status.Running)
			{
				switch (commandName)
				{
					case CommandNamesMercury203Virtual.SetCommunicationError:
						{ CommunicationError = true; break; }
					case CommandNamesMercury203Virtual.ResetCommunicationError:
						{ CommunicationError = false; break; }
					case CommandNamesMercury203Virtual.SetConfigurationError:
						{ ConfigurationError = true; break; }
					case CommandNamesMercury203Virtual.ResetConfigurationError:
						{ ConfigurationError = false; break; }
					case CommandNamesMercury203Virtual.SetRtcError:
						{ RtcError = true; break; }
					case CommandNamesMercury203Virtual.ResetRtcError:
						{ RtcError = false; break; }
					case CommandNamesMercury203Virtual.SwitchReleOn:
					case CommandNamesMercury203Virtual.SwitchReleOff:
					default:
						{
							throw new NotSupportedException(String.Format(
								"Попытка выполнить устройством Id={} неизвестную команду cmd={0}",
								Id, commandName));
						}
				}
			}
		}

		/// <summary>
		/// Установка нового сетевого адреса счетчика (CMD=00h)
		/// </summary>
		/// <param name="addr">Текущий сетевой адрес счётчика</param>
		/// <param name="newaddr">Новый сетевой адрес счётчика</param>
		public void WriteNewAddress(uint addr, uint newaddr)
		{
			_parameters[ParameterNamesMercury203Virtual.Address].Value = newaddr;
		}

		/// <summary>
		/// Запись группового адреса счётчика (CMD=01h)
		/// </summary>
		public void WriteGroupAddress(UInt32 address)
		{
			_parameters[ParameterNamesMercury203Virtual.GADDR].Value = address;
		}

		/// <summary>
		/// Установка внутренних часов и календаря счетчика (CMD=02h)
		/// </summary>
		/// <returns></returns>
		public void WriteDateTime(DateTime dateTime)
		{
			_parameters[ParameterNamesMercury203Virtual.DateTime].Value =
				IncotexDateTime.FromDateTime(dateTime);
		}

		/// <summary>
		/// Чтение группового адреса счетчика (CMD=20h)
		/// </summary>
		public uint ReadGroupAddress()
		{
			return (uint)_parameters[ParameterNamesMercury203Virtual.GADDR].Value;
		}

		/// <summary>
		/// Чтение внутренних часов и календаря счетчика (CMD=21h)
		/// </summary>
		/// <returns></returns>
		public System.DateTime ReadDateTime()
		{
			return ((IncotexDateTime)_parameters[ParameterNamesMercury203Virtual.DateTime].Value)
				.ToDateTime();
		}

		/// <summary>
		/// Чтение лимита мощности (CMD=22h)
		/// </summary>
		/// <returns></returns>
		public ushort ReadPowerLimit()
		{
			return (ushort)_parameters[ParameterNamesMercury203Virtual.PowerLimit].Value;
		}

		/// <summary>
		/// Чтение лимита энергии за месяц
		/// </summary>
		/// <returns></returns>
		public uint ReadPowerLimitPerMonth()
		{
			return (ushort)_parameters[ParameterNamesMercury203Virtual.PowerLimitPerMonth].Value;
		}

		/// <summary>
		/// Чтение содержимого тарифных аккумуляторов (CMD=27H)
		/// </summary>
		/// <returns></returns>
		public TariffCounters ReadConsumedPower()
		{
			return new TariffCounters
			{
				ValueTotalTarif1 = (float)_parameters[ParameterNamesMercury203Virtual.CounterTarif1].Value,
				ValueTotalTarif2 = (float)_parameters[ParameterNamesMercury203Virtual.CounterTarif2].Value,
				ValueTotalTarif3 = (float)_parameters[ParameterNamesMercury203Virtual.CounterTarif3].Value,
				ValueTotalTarif4 = (float)_parameters[ParameterNamesMercury203Virtual.CounterTarif4].Value
			};
		}

		#endregion

		#region Events

		public event EventHandler StatusChanged;
        public event EventHandler<DeviceErrorOccuredEventArgs> ErrorOccurred;
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        #endregion

	}
}
