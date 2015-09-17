using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RubezhResurs.OSI.ApplicationLayer;
using RubezhResurs.OSI.DataLinkLayer;
using RubezhResurs.Devices;
using RubezhResurs.Devices.Collections.ObjectModel;
using RubezhResurs.Management;

namespace RubezhResurs.Incotex.NetworkControllers
{
    /// <summary>
    /// Сетевой контроллер для работы со устройствами производства Incotex
    /// </summary>
    public class IncotexNetworkController: INetwrokController
    {
        #region Fields And Properties

        public uint _ControllerId;
        /// <summary>
        /// Идентификатор сетевого контроллера
        /// </summary>
        public uint ControllerId
        {
            get { return _ControllerId; }
            set { _ControllerId = value; }
        }
        /// <summary>
        /// Список типов устройств поддерживаемых данным контроллерм
        /// </summary>
        public IEnumerable<DeviceType> DevicesSuppoted
        {
            get 
            {
                return new DeviceType[] { DeviceType.Mercury203 };
            }
        }

        private DevicesCollection _Devices;
        /// <summary>
        /// Коллекция устройств в сети
        /// </summary>
        public DevicesCollection Devices
        {
            get { return _Devices; }
        }

        private Status _Status;
        /// <summary>
        /// Состояние контроллера
        /// </summary>
        public Status Status
        {
            get
            {
                return _Status;
            }
            set
            {
                if ((value == Status.Running) || (value == Status.Stopped))
                {
                    _Status = value;
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }

        IDataLinkPort _Connection;
        /// <summary>
        /// Объетк для соединения с физическим интерфейсом
        /// </summary>
        public IDataLinkPort Connection
        {
            get { return _Connection; }
            set 
            {
                if (_Connection != null) 
                {
                    if (_Connection.IsOpen)
                    {
                        throw new InvalidOperationException();
                    }
                    else
                    {
                        _Connection = value;
                    }
                }
                else
                {
                    _Connection = value;
                }
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор
        /// </summary>
        public IncotexNetworkController()
        {
            _Devices = new DevicesCollection();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Запускат сетевой контроллер
        /// </summary>
        public void Start()
        {
            Status = Status.Running;
        }
        /// <summary>
        /// Останавливает сетевой контроллер
        /// </summary>
        public void Stop()
        {
            Status = Status.Stopped;
        }
        /// <summary>
        /// Приостанавливает сетевой контроллер
        /// </summary>
        public void Suspend()
        {
            throw new NotSupportedException();
        }
        /// <summary>
        /// Генерирует событие
        /// </summary>
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
