using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ResursNetwork.OSI.ApplicationLayer;
using ResursNetwork.OSI.ApplicationLayer.Devices;
using ResursNetwork.OSI.ApplicationLayer.Devices.Collections.ObjectModel;
using ResursNetwork.OSI.ApplicationLayer.Devices.ValueConverters;
using ResursNetwork.Incotex.Models;
using ResursNetwork.Incotex.Models.DateTime;
using ResursNetwork.Management;
using ResursAPI.Models;
using ResursAPI.ParameterNames;

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
				return (Guid)_Parameters[ParameterNamesMercury203.Address].Value;
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
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
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
                _Errors.CommunicationError = value;
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
                _Errors.ConfigurationError = value;
            }
        }

        public bool RTCError
        {
            get
            {
                return _Errors.RTCError;
            }
            set
            {
                _Errors.RTCError = value;
            }
        }

		public System.DateTime RTC
		{
			get 
			{ 
				return (System.DateTime)_Parameters[ParameterNamesMercury203Virtual
				.DateTime].Value; 
			}
			set
			{
				_Parameters[ParameterNamesMercury203Virtual.DateTime].Value = value;
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

			_Parameters.Add(new Parameter(typeof(UInt32))
			{
				Name = ParameterNamesMercury203Virtual.CounterTarif1,
				Description = "Счётчик тарифа 1",
				PollingEnabled = true,
				ReadOnly = false,
				ValueConverter = null,
				Value = (UInt32)0
			});

			_Parameters.Add(new Parameter(typeof(UInt32))
			{
				Name = ParameterNamesMercury203Virtual.CounterTarif2,
				Description = "Счётчик тарифа 2",
				PollingEnabled = true,
				ReadOnly = false,
				ValueConverter = null,
				Value = (UInt32)0
			});

			_Parameters.Add(new Parameter(typeof(UInt32))
			{
				Name = ParameterNamesMercury203.CounterTarif3,
				Description = "Счётчик тарифа 3",
				PollingEnabled = true,
				ReadOnly = false,
				ValueConverter = null,
				Value = (UInt32)0
			});

			_Parameters.Add(new Parameter(typeof(UInt32))
			{
				Name = ParameterNamesMercury203.CounterTarif4,
				Description = "Счётчик тарифа 3",
				PollingEnabled = true,
				ReadOnly = false,
				ValueConverter = null,
				Value = (UInt32)0
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

        #endregion

        #region Events

        public event EventHandler StatusChanged;
        public event EventHandler<ErrorOccuredEventArgs> ErrorOccurred;
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
