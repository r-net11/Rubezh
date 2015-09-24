using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RubezhResurs.Devices.Collections.ObjectModel;
using RubezhResurs.Management;
using RubezhResurs.OSI.DataLinkLayer;

namespace RubezhResurs.OSI.ApplicationLayer
{
    /// <summary>
    /// Базовый класс сетевого контроллера
    /// </summary>
    public abstract class NetworkControllerBase: INetwrokController
    {
        #region Fields And Properties
        //private static List<UInt32> RegisteredControllerIds = new List<uint>();

        protected uint _ControllerId;
        /// <summary>
        /// Id контроллера
        /// </summary>
        public uint ControllerId
        {
            get { return _ControllerId; }
            set { _ControllerId = value; }
        }
        /// <summary>
        /// Возвращает список типов устройств с которыми может работать данный контроллер
        /// </summary>
        public abstract IEnumerable<Devices.DeviceType> SuppotedDevices { get; }

        protected DevicesCollection _Devices;
        /// <summary>
        /// Возвращает список устройств
        /// </summary>
        public DevicesCollection Devices
        {
            get { return _Devices; }
        }

        protected Status _Status;
        /// <summary>
        /// Возвращает или устанавливает статус контроллера
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
                    switch (value)
                    {
                        case Status.Running:
                            { 
                                _Status = value;
                                OnStatusWasChanged();
                                break; 
                            }
                        case Status.Stopped:
                            { 
                                _Status = value;
                                OnStatusWasChanged();
                                break; 
                            }
                        default:
                            { throw new NotSupportedException(); }
                    }
                }
            }
        }

        protected IDataLinkPort _Connection;
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
        public NetworkControllerBase()
        {
            _Devices = new DevicesCollection();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Запускат сетевой контроллер
        /// </summary>
        public virtual void Start()
        {
            Status = Status.Running;
        }
        /// <summary>
        /// Останавливает сетевой контроллер
        /// </summary>
        public virtual void Stop()
        {
            Status = Status.Stopped;
        }
        /// <summary>
        /// Приостанавливает сетевой контроллер
        /// </summary>
        public virtual void Suspend()
        {
            throw new NotSupportedException();
        }
        /// <summary>
        /// Генерирует событие изменения состояния контроллера
        /// </summary>
        protected virtual void OnStatusWasChanged()
        {
            if (StatusWasChanged != null)
            {
                StatusWasChanged(this, new EventArgs());
            }
        }
        /// <summary>
        /// Метод выполняет сетевой опрос устройств
        /// </summary>
        protected virtual void DoNetwokPollingAsync(Action method)
        {
            // Выполняем запросы к сетевым устройствам
            while(_Status == Status.Running)
            {
                Task task;
                //task.
                method();
            }
        }
        #endregion

        #region Events
        public event EventHandler StatusWasChanged;
        #endregion
    }
}
