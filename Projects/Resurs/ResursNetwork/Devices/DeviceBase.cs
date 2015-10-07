using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using ResursNetwork.Devices.Collections.ObjectModel;
using ResursNetwork.OSI.ApplicationLayer;
using ResursNetwork.Management;
using ResursAPI.ParameterNames;

namespace ResursNetwork.Devices
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

        public UInt32 Address
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
            internal set 
            {
                if (_NetworkController != value)
                {
                    _NetworkController = value;
                    OnPropertyChanged("Network");
                }
            }
        }

        public ParatemersCollection Parameters
        {
            get { return _Parameters; }
        }
        #endregion

        #region Constructors

        protected DeviceBase()
        {
            _Status = Status.Stopped;
            _Parameters = new ParatemersCollection();

            _Parameters.Add(new Parameter(typeof(Guid))
            {
                Name = ParameterNamesMercury203.Id,
                Description = "Сетевой адрес устройства",
                PollingEnabled = false,
                ReadOnly = false,
                ValueConverter = null,
                Value = Guid.NewGuid()
            });

            _Parameters.Add(new Parameter(typeof(UInt32))
            {
                Name = ParameterNamesMercury203.Address,
                Description = "Сетевой адрес устройтсва",
                PollingEnabled = false,
                ReadOnly = false,
                ValueConverter = null,
                Value = 1
            });

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
        protected abstract void Initialization();

        public void Start()
        {
            _Status = Status.Running;
        }

        public void Stop()
        {
            _Status = Status.Stopped;
        }

        public void Suspend()
        {
            throw new NotSupportedException();
        }

        protected virtual void OnStatusChanged()
        {
            if (StatusChanged != null)
            {
                StatusChanged(this, new EventArgs());
            }
        }

        protected virtual void OnErrorOccurred(ErrorOccuredEventArgs args)
        {
            if (ErrorOccurred != null)
            {
                ErrorOccurred(this, 
                    args == null ? new ErrorOccuredEventArgs() : args);
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

        #endregion

        #region Events

        public event EventHandler StatusChanged;
        public event EventHandler<ErrorOccuredEventArgs> ErrorOccurred;
        public event PropertyChangedEventHandler PropertyChanged;
        
        #endregion
    }
}
