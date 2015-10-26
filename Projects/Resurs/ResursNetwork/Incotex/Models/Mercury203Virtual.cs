using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ResursNetwork.OSI.ApplicationLayer;
using ResursNetwork.OSI.ApplicationLayer.Devices;
using ResursNetwork.OSI.ApplicationLayer.Devices.Collections.ObjectModel;
using ResursNetwork.OSI.ApplicationLayer.Devices.ValueConverters;
using ResursNetwork.Incotex.Models;
using ResursNetwork.Management;
using ResursAPI.Models;
using ResursAPI.ParameterNames;
using ResursAPI.CommandNames;

namespace ResursNetwork.Incotex.Models
{
	public class Mercury203Virtual: IDevice
	{
		#region Fields And Properties

		private ParatemersCollection _Parameters = new ParatemersCollection();
		private Status _Status = Status.Stopped;
		private DeviceErrors _Errors;
		private INetwrokController _NetworkController;

		
        public Guid Id
        {
            get
            {
				return (Guid)_Parameters[ParameterNamesMercury203.Id].Value;
            }
            set
            {
				var id = (Guid)_Parameters[ParameterNamesMercury203.Id.ToString()].Value;

				if (id != value)
				{
					_Parameters[ParameterNamesMercury203.Id.ToString()].Value = value;
					//OnPropertyChanged("Id");
				}
            }
        }

        public DeviceType DeviceType
        {
            get { return DeviceType.VirtualMercury203; }
        }

        public uint Address
        {
            get { return (UInt32)_Parameters[ParameterNamesMercury203.Address].Value; }
			set 
			{
				if (value == 0)
				{
					throw new ArgumentOutOfRangeException(
						"Попытка установить недопустимый адрес равеный 0");
				}

				var address = (UInt32)_Parameters[ParameterNamesMercury203.Address].Value;

				if (address != value)
				{
					_Parameters[ParameterNamesMercury203.Address].Value = value;
					//OnPropertyChanged("Address");
				}
			}
        }

        public INetwrokController Network
        {
            get { return _NetworkController; }
			set { _NetworkController = value; }
        }

        public ParatemersCollection Parameters
        {
            get { return _Parameters; }
        }

        public Status Status
        {
            get
            {
                return _Status;
            }
            set
            {
				if (_Status != value)
				{
					_Status = value;
					OnStatusChanged();
				}
            }
        }

        public DeviceErrors Errors
        {
            get { return _Errors; }
        }

        public bool CommunicationError
        {
            get
            {
                return _Errors.CommunicationError;
            }
            set
            {
				if (_Errors.CommunicationError != value)
				{
					_Errors.CommunicationError = value;
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
                return _Errors.ConfigurationError;
            }
			set
			{
				if (_Errors.ConfigurationError != value)
				{
					_Errors.ConfigurationError = value;
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
                return _Errors.RTCError;
            }
			set
			{
				if (_Errors.RTCError != value)
				{
					_Errors.RTCError = value;
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
				return ((IncotexDateTime)_Parameters[ParameterNamesMercury203Virtual
				.DateTime].Value).ToDateTime(); 
			}
			set
			{
				_Parameters[ParameterNamesMercury203Virtual.DateTime].Value = 
					IncotexDateTime.FromDateTime(value);
			}
		}
		
		#endregion

		#region Constructors
		
		public Mercury203Virtual()
		{
			Initialization();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Инициализирует список свойств для конкретного устройства
		/// </summary>
		private void Initialization()
		{
			_Errors.Reset();

			_Parameters = new ParatemersCollection();

			_Parameters.Add(new Parameter(typeof(Guid))
			{
				Name = ParameterNamesMercury203Virtual.Id,
				Description = "Сетевой адрес устройства",
				PollingEnabled = false,
				ReadOnly = false,
				ValueConverter = null,
				Value = Guid.NewGuid()
			});

			_Parameters.Add(new Parameter(typeof(UInt32))
			{
				Name = ParameterNamesMercury203Virtual.Address,
				Description = "Сетевой адрес устройтсва",
				PollingEnabled = false,
				ReadOnly = false,
				ValueConverter = null,
				Value = (UInt32)1
			});

			_Parameters.Add(new Parameter(typeof(UInt32))
			{
				Name = ParameterNamesMercury203Virtual.GADDR,
				Description = "Групповой адрес счётчика",
				PollingEnabled = true,
				ReadOnly = false,
				ValueConverter = new BigEndianUInt32ValueConverter(),
				Value = (UInt32)0
			});

			_Parameters.Add(new Parameter(typeof(IncotexDateTime))
			{
				Name = ParameterNamesMercury203Virtual.DateTime,
				Description = "Текущее значение часов счётчика",
				PollingEnabled = true,
				ReadOnly = false,
				ValueConverter = new IncotexDataTimeTypeConverter(),
				Value = new IncotexDateTime()
			});

			_Parameters.Add(new Parameter(typeof(UInt16))
			{
				Name = ParameterNamesMercury203Virtual.PowerLimit,
				Description = "Значение лимита мощности",
				PollingEnabled = true,
				ReadOnly = false,
				ValueConverter = new BigEndianUInt16ValueConvertor(),
				Value = (UInt16)0
			});

			_Parameters.Add(new Parameter(typeof(float))
			{
				Name = ParameterNamesMercury203Virtual.CounterTarif1,
				Description = "Счётчик тарифа 1",
				PollingEnabled = true,
				ReadOnly = false,
				ValueConverter = null,
				Value = (float)0
			});

			_Parameters.Add(new Parameter(typeof(float))
			{
				Name = ParameterNamesMercury203Virtual.CounterTarif2,
				Description = "Счётчик тарифа 2",
				PollingEnabled = true,
				ReadOnly = false,
				ValueConverter = null,
				Value = (float)0
			});

			_Parameters.Add(new Parameter(typeof(float))
			{
				Name = ParameterNamesMercury203Virtual.CounterTarif3,
				Description = "Счётчик тарифа 3",
				PollingEnabled = true,
				ReadOnly = false,
				ValueConverter = null,
				Value = (float)0
			});

			_Parameters.Add(new Parameter(typeof(float))
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
		/// Установка нового сетевого адреса счетчика 
		/// </summary>
		/// <param name="addr">Текущий сетевой адрес счётчика</param>
		/// <param name="newaddr">Новый сетевой адрес счётчика</param>
		public void SetNewAddress(uint addr, uint newaddr)
		{
			_Parameters[ParameterNamesMercury203Virtual.Address].Value = newaddr;
		}
		
		/// <summary>
		/// Чтение группового адреса счетчика (CMD=20h)
		/// </summary>
		public uint ReadGroupAddress()
		{
			return (uint)_Parameters[ParameterNamesMercury203Virtual.GADDR].Value;
		}
		
		/// <summary>
		/// Чтение внутренних часов и календаря счетчика (CMD=21h)
		/// </summary>
		/// <returns></returns>
		public System.DateTime ReadDateTime()
		{
			return ((IncotexDateTime)_Parameters[ParameterNamesMercury203Virtual.DateTime].Value)
				.ToDateTime();
		}

		/// <summary>
		/// Чтение лимита мощности (CMD=22h)
		/// </summary>
		/// <returns></returns>
		public ushort ReadPowerLimit()
		{
			return (ushort)_Parameters[ParameterNamesMercury203Virtual.PowerLimit].Value;
		}

		/// <summary>
		/// Чтение лимита энергии за месяц
		/// </summary>
		/// <returns></returns>
		public uint ReadPowerLimitPerMonth()
		{
			throw new NotImplementedException();
			//return (ushort)_Parameters[ParameterNamesMercury203Virtual.PowerLimitPerMonth].Value;
		}

		/// <summary>
		/// Чтение содержимого тарифных аккумуляторов (CMD=27H)
		/// </summary>
		/// <returns></returns>
		public PowerCounters ReadConsumedPower()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Events

		public event EventHandler StatusChanged;
        public event EventHandler<DeviceErrorOccuredEventArgs> ErrorOccurred;
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        #endregion
	}
}
