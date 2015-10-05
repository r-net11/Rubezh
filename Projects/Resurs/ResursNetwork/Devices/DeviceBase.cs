using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using ResursNetwork.Devices.Collections.ObjectModel;
using ResursNetwork.OSI.ApplicationLayer;
using ResursNetwork.Management;

namespace ResursNetwork.Devices
{
    /// <summary>
    /// Сетевое устройство
    /// </summary>
    public abstract class DeviceBase: IDevice, IManageable
    {
        #region Fields And Properties
        private Guid _Id;
        private UInt32 _Address;
        private Status _Status;
        protected INetwrokController _NetworkController;
        protected ParatemersCollection _Parameters;

        public Guid Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        public abstract DeviceType DeviceType { get; }

        public UInt32 Address
        {
            get { return _Address; }
            set 
            {
                if (_Address != value)
                {
                    _Address = value;
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
            _Id = Guid.NewGuid();
            _Status = Status.Stopped;
            _Parameters = new ParatemersCollection();
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
