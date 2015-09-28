﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ResursNetwork.Devices.Collections.ObjectModel;
using ResursNetwork.Management;
using ResursNetwork.OSI.DataLinkLayer;
using Common;

namespace ResursNetwork.OSI.ApplicationLayer
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

        protected CancellationTokenSource _CancellationTokenSource;
        protected Task _NetworkPollingTask;
        
        /// <summary>
        /// Возвращает или устанавливает статус контроллера
        /// </summary>
        public Status Status
        {
            get
            {
                if (_NetworkPollingTask == null)
                {
                    return Status.Stopped;
                }
                else
                {
                    switch(_NetworkPollingTask.Status)
                    {
                        case TaskStatus.Canceled:
                        case TaskStatus.RanToCompletion:
                        case TaskStatus.Faulted:
                            { return Status.Stopped; }
                        case TaskStatus.Created:
                        case TaskStatus.Running:
                        case TaskStatus.WaitingForActivation:
                        case TaskStatus.WaitingForChildrenToComplete:
                        case TaskStatus.WaitingToRun:
                            { return Status.Running; }
                        default:
                            { throw new NotImplementedException(); }
                    }
                }
            }
            set
            {
                if (Status != value)
                {
                    switch (value)
                    {
                        case Status.Running:
                            {
                                if (_Connection == null)
                                {
                                    throw new InvalidOperationException(
                                        "Невозможно запустить контроллер. Не установлено соединение");
                                }
                                else
                                {
                                    _Connection.Open();
                                }

                                if (_CancellationTokenSource == null)
                                {
                                    _CancellationTokenSource = new CancellationTokenSource();
                                }
                                // Запускаем сетевой обмен данными
                                _NetworkPollingTask = Task.Factory.StartNew(NetwokPollingAction, 
                                    _CancellationTokenSource.Token);

                                Logger.Info(String.Format("Controller Id={0} | Изменил состояние на новое Status={1}",
                                    _ControllerId, Status.Running));
                                
                                OnStatusWasChanged();
                                break; 
                            }
                        case Status.Stopped:
                            { 
                                // Останавливаем сетевой обмен данными
                                try
                                {
                                    _CancellationTokenSource.Cancel();
                                    _NetworkPollingTask.Wait(); // Ждём завершения операции отмены задачи
                                }
                                catch (AggregateException)
                                {
                                    if (!_NetworkPollingTask.IsCanceled)
                                    {
                                        throw;
                                    }
                                }
                                catch (Exception)
                                {
                                    throw;
                                }
                                finally
                                {
                                    _NetworkPollingTask.Dispose();
                                    _CancellationTokenSource.Dispose();

                                    Logger.Info(String.Format("Controller Id={0} | {Изменил состояние на новое {0}}",
                                        _ControllerId, Status.Stopped));
                                    
                                    _Connection.Close();
                                    OnStatusWasChanged();
                                }
                                break; 
                            }
                        default:
                            { throw new NotSupportedException(); }
                    }
                }
            }
        }

        protected EventHandler _MessageReceived;

        protected IDataLinkPort _Connection;
        /// <summary>
        /// Объетк для соединения с физическим интерфейсом
        /// </summary>
        public virtual IDataLinkPort Connection
        {
            get { return _Connection; }
            set
            {
                if ((Status == Status.Running) || (Status == Status.Paused))
                {
                    throw new InvalidOperationException(
                        "Невозможно выполенить установку порта, контроллер в активном состоянии");
                }

                if (_Connection != null)
                {
                    if (_Connection.IsOpen)
                    {
                        throw new InvalidOperationException();
                    }
                    else
                    {
                        if (_Connection != null)
                        {
                            _Connection.MessageReceived -= _MessageReceived;
                        }
                        _Connection = value;
                        _Connection.MessageReceived += _MessageReceived;
                    }
                }
                else
                {
                    _Connection = value;
                    
                    if (_Connection != null)
                    {
                        _Connection.MessageReceived += _MessageReceived;
                    }
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
            _MessageReceived = new EventHandler(EventHandler_Connection_MessageReceived);
            _Devices = new DevicesCollection();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Запускает опрос удалённых устройств 
        /// </summary>
        public virtual void Start()
        {
            Status = Status.Running;
        }
        /// <summary>
        /// Останавливает опрос удалённых устройств)
        /// </summary>
        public virtual void Stop()
        {
            Status = Status.Stopped;
        }
        /// <summary>
        /// Приостанавливает опрос удалённых устройств
        /// </summary>
        public virtual void Suspend()
        {
            Status = Status.Paused;
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
        /// Член IDisposable
        /// </summary>
        public virtual void Dispose()
        {
            Stop();
        }
        /// <summary>
        /// Обработчик приёма сообщения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected abstract void EventHandler_Connection_MessageReceived(object sender, EventArgs e);
        /// <summary>
        /// Метод выполняет сетевой опрос устройств
        /// </summary>
        protected abstract void NetwokPollingAction(Object cancelToken);
        #endregion

        #region Events
        public event EventHandler StatusWasChanged;
        #endregion
    }
}
