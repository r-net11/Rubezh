using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using ResursNetwork.OSI.ApplicationLayer.Devices.Collections.ObjectModel;
using ResursNetwork.OSI.ApplicationLayer;
using ResursNetwork.Management;
using ResursAPI.ParameterNames;
using ResursAPI.Models;

namespace ResursNetwork.OSI.ApplicationLayer.Devices
{
    /// <summary>
    /// Сетевое устройство
    /// </summary>
    public abstract class DeviceBase: IDevice
    {
        #region Fields And Properties
        private Status _Status;
        protected INetwrokController _NetworkController;
        protected ParatemersCollection _Parameters;
        protected DeviceErrors _Errors;

        public Guid Id
        {
            get { return (Guid)_Parameters[ParameterNamesMercury203.Id.ToString()].Value; }
            set 
            {
                var id = (Guid)_Parameters[ParameterNamesMercury203.Id.ToString()].Value;

                if (id != value)
                {
                    _Parameters[ParameterNamesMercury203.Id.ToString()].Value = value;
                    OnPropertyChanged("Id");
                }
            }
        }

        public abstract DeviceType DeviceType { get; }

        public virtual UInt32 Address
        {
            get { return (UInt32)_Parameters[ParameterNamesMercury203.Address].Value; }
            set  
            {
                var address = (UInt32)_Parameters[ParameterNamesMercury203.Address].Value;
                
                if (address != value)
                {
                    _Parameters[ParameterNamesMercury203.Address].Value = value;
                    OnPropertyChanged("Address");
                }
            }
        }

        /// <summary>
        /// Сосотояние устройства
        /// </summary>
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
                    OnPropertyChanged("Status");
                }
            }
        }

        /// <summary>
        /// Сетевой контроллер которому принадлежит данное устройтво
        /// </summary>
        public INetwrokController Network
        {
            get { return _NetworkController; }
            set 
            {

                if (_NetworkController != value)
                {
                    if (_NetworkController != null)
                    {
                        _NetworkController.NetwrokRequestCompleted -=
                            EventHandler_NetworkController_NetwrokRequestCompleted;
                    }
                    
                    _NetworkController = value;
                    
                    if (_NetworkController != null)
                    {
                        _NetworkController.NetwrokRequestCompleted +=
                            EventHandler_NetworkController_NetwrokRequestCompleted;
                    }

                    OnPropertyChanged("Network");
                }
            }
        }

        public ParatemersCollection Parameters
        {
            get { return _Parameters; }
        }

        public DeviceErrors Errors
        {
            get { return _Errors; }
        }

        public bool CommunicationError
        {
            get { return _Errors.CommunicationError; }
            set 
            { 
                if (_Errors.CommunicationError != value)
                {
                    _Errors.CommunicationError = value;

                    OnErrorOccurred(new DeviceErrorOccuredEventArgs
                    {
                        Errors = _Errors
                    });
                }
            }
        }

        public bool ConfigurationError
        {
            get { return _Errors.ConfigurationError; }
            set
            {
                if (_Errors.ConfigurationError != value)
                {
                    _Errors.ConfigurationError = value;

                    OnErrorOccurred(new DeviceErrorOccuredEventArgs
                    {
                        Errors = _Errors
                    });
                }
            }
        }

        public bool RtcError
        {
            get { return _Errors.RTCError; }
            set
            {
                if (_Errors.RTCError != value)
                {
                    _Errors.RTCError = value;

                    OnErrorOccurred(new DeviceErrorOccuredEventArgs
                    {
                        Errors = _Errors
                    });
                }
            }
        }

		public abstract DateTime Rtc { get; set; }

        #endregion

        #region Constructors

        protected DeviceBase()
        {
            _Status = Status.Stopped;
            _Errors.Reset();
            Initialization();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Создаёт устройство на основе типа
        /// </summary>
        /// <param name="deviceType">Тип устройства</param>
        /// <returns></returns>
        public static DeviceBase CreateDevice(DeviceType deviceType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Инициализирует структуру устройства
        /// </summary>
		protected virtual void Initialization()
		{
			_Parameters = new ParatemersCollection();

			_Parameters.Add(new Parameter(typeof(Guid))
			{
				Name = ParameterNamesBase.Id,
				Description = "Сетевой адрес устройства",
				PollingEnabled = false,
				ReadOnly = false,
				ValueConverter = null,
				Value = Guid.NewGuid()
			});

			_Parameters.Add(new Parameter(typeof(UInt32))
			{
				Name = ParameterNamesBase.Address,
				Description = "Сетевой адрес устройтсва",
				PollingEnabled = false,
				ReadOnly = false,
				ValueConverter = null,
				Value = (UInt32)1
			});
		}

        public void Start()
        {
            _Status = Status.Running;
        }

        public void Stop()
        {
            _Status = Status.Stopped;
        }

		public abstract void ExecuteCommand(string commandName);

        protected virtual void OnStatusChanged()
        {
            if (StatusChanged != null)
            {
                StatusChanged(this, new EventArgs());
            }
        }

		public abstract OperationResult ReadParameter(string parameterName);

		public abstract void WriteParameter(string parameterName, ValueType value);

        protected virtual void OnErrorOccurred(DeviceErrorOccuredEventArgs args)
        {
            if (ErrorOccurred != null)
            {
                ErrorOccurred(this, 
                    args == null ? new DeviceErrorOccuredEventArgs() : args);
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        
        /// <summary>
        /// Устанавливает или сбрасывает соответсвующие флаги ошибок устройтсва
        /// </summary>
        /// <param name="errors"></param>
        //internal void SetError(DeviceErrors errors)
        //{
        //    if (errors != _Errors)
        //    {
        //        _Errors = errors;                
        //        OnErrorOccurred(new ErrorOccuredEventArgs { Errors = errors });
        //    }
        //}

        /// <summary>
        /// Обработчик события завершения обработки сетевого запроса от контроллера сети 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void EventHandler_NetworkController_NetwrokRequestCompleted(
            object sender, NetworkRequestCompletedArgs e)
        {
        }

        #endregion

        #region Events

        public event EventHandler StatusChanged;
        public event EventHandler<DeviceErrorOccuredEventArgs> ErrorOccurred;
        public event PropertyChangedEventHandler PropertyChanged;
        
        #endregion

	}
}
