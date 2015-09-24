using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RubezhResurs.Devices.Collections.ObjectModel;
using RubezhResurs.Management;

namespace RubezhResurs.Devices
{
    /// <summary>
    /// Сетевое устройство
    /// </summary>
    public abstract class Device: IDevice, IManageable
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
        protected ParatemersCollection _Parameters;
        public ParatemersCollection Parameters
        {
            get { return _Parameters; }
        }
        #endregion

        #region Constructors
        protected Device()
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
        public static Device CreateDevice(DeviceType deviceType)
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

        private void OnStatusWasChanged()
        {
            if (StatusWasChanged != null)
            {
                StatusWasChanged(this, new EventArgs());
            }
        }

        #endregion

        #region Events
        public event EventHandler StatusWasChanged;
        #endregion
    }
}
