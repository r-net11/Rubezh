using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public abstract DeviceType DeviceType { get; }
        private UInt32 _Address;
        public UInt32 Address
        {
            get { return _Address; }
            set { _Address = value; }
        }
        private Status _Status;
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
                _Status = value;
            }
        }

        protected INetwrokController _NetworkController;
        /// <summary>
        /// Сетевой контроллер которому принадлежит данное устройтво
        /// </summary>
        public INetwrokController Network
        {
            get { return _NetworkController; }
            internal set { _NetworkController = value; }
        }
        protected ParatemersCollection _Parameters;
        public ParatemersCollection Parameters
        {
            get { return _Parameters; }
        }
        #endregion

        #region Constructors
        protected DeviceBase()
        {
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
        protected virtual void OnStatusWasChanged()
        {
            if (StatusWasChanged != null)
            {
                StatusWasChanged(this, new EventArgs());
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
        #endregion

        #region Events
        public event EventHandler StatusWasChanged;
        public event EventHandler<ErrorOccuredEventArgs> ErrorOccurred;
        #endregion

    }
}
