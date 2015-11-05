using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Specialized;
using ResursNetwork.OSI.ApplicationLayer.Devices.Collections.ObjectModel;
using ResursNetwork.Management;
using ResursNetwork.OSI.DataLinkLayer;
using ResursNetwork.OSI.Messages;
using ResursNetwork.OSI.Messages.Transactions;
using ResursAPI.Models;
using Common;

namespace ResursNetwork.OSI.ApplicationLayer
{
    /// <summary>
    /// Базовый класс сетевого контроллера
    /// </summary>
    public abstract class NetworkControllerBase: INetwrokController
    {
        #region Fields And Properties

        protected Guid _Id;
        protected DevicesCollection _devices;
        protected CancellationTokenSource _CancellationTokenSource;
        protected Task _NetworkPollingTask;
        protected Status _Status = Status.Stopped;
        protected EventHandler _MessageReceived;
        protected IDataLinkPort _Connection;
        protected int _TotalAttempts;
		protected int _pollingPeriod =
			Convert.ToInt32(TimeSpan.FromDays(1).TotalMilliseconds); // По умолчнию период синхронизации 1 день;
		protected NetworkControllerErrors _errors;

        /// <summary>
        /// Id контроллера
        /// </summary>
        public Guid Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        /// <summary>
        /// Возвращает список типов устройств с которыми может работать данный контроллер
        /// </summary>
        public abstract IEnumerable<Devices.DeviceModel> SuppotedDevices { get; }

        /// <summary>
        /// Возвращает список устройств
        /// </summary>
        public DevicesCollection Devices
        {
            get { return _devices; }
        }

        /// <summary>
        /// Возвращает или устанавливает статус контроллера
        /// </summary>
        public Status Status
        {
            get
            {
                //return _Status;
                if (_NetworkPollingTask == null)
                {
                    return Status.Stopped;
                }
                else
                {
                    switch (_NetworkPollingTask.Status)
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
                    //_Status = value;
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
                                _NetworkPollingTask = Task.Factory.StartNew(NetworkPollingAction,
                                    _CancellationTokenSource.Token);

                                Logger.Info(String.Format("Controller Id={0} | Изменил состояние на новое Status={1}",
                                    _Id, Status.Running));

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
#if !DEBUG
                                    Logger.Info(String.Format("Controller Id={0} | {Изменил состояние на новое {0}}",
                                        _Id, Status.Stopped));
#endif
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

        /// <summary>
        /// Объетк для соединения с физическим интерфейсом
        /// </summary>
        public virtual IDataLinkPort Connection
        {
            get { return _Connection; }
            set
            {
                if (Status == Status.Running)
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

        /// <summary>
        /// Кол-во попыток доступа к устройтсву прежде
        /// чем устройство переводится в ошибка соединения 
        /// </summary>
        public int TotalAttempts
        {
            get { return _TotalAttempts; }
            set
            {
                if (value > 0)
                {
                    _TotalAttempts = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

		/// <summary>
		/// Период (мсек) получения данных от удалённых устройтв
		/// </summary>
		public virtual int PollingPeriod
		{
			get { return _pollingPeriod; }
			set
			{
				_pollingPeriod = value;
			}
		}

		public NetworkControllerErrors Errors
		{
			get { return _errors; }
		}

        #endregion
        
        #region Constructors

        /// <summary>
        /// Конструктор
        /// </summary>
        public NetworkControllerBase()
        {
            _Id = Guid.NewGuid();
            _TotalAttempts = 1;
            _MessageReceived = new EventHandler(EventHandler_Connection_MessageReceived);
            _devices = new DevicesCollection(this);
			_devices.CollectionChanged += EventHandler_Devices_CollectionChanged;
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

		private void EventHandler_Devices_CollectionChanged(
			object sender, DevicesCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					{
						OnConfigurationChanged(
							new ConfigurationChangedEventArgs 
							{ 
								Device = e.Device, 
								Action = ConfigurationChangedAction.DeviceAdded 
							});
						break;
					}
				case NotifyCollectionChangedAction.Remove:
					{
						OnConfigurationChanged(
							new ConfigurationChangedEventArgs
							{
								Device = e.Device,
								Action = ConfigurationChangedAction.DeviceRemoved
							});
						break;
					}
				default:
					{
						throw new NotImplementedException();
					}
			}
		}

		private void OnConfigurationChanged(ConfigurationChangedEventArgs args)
		{
			if (args == null)
			{
				throw new ArgumentNullException();
			}

			if (ConfigurationChanged != null)
			{
				ConfigurationChanged(this, args);
			}
		}

        /// <summary>
        /// Генерирует событие изменения состояния контроллера
        /// </summary>
        protected virtual void OnStatusWasChanged()
        {
            if (StatusChanged != null)
            {
                StatusChanged(this, new EventArgs());
            }
        }

        protected virtual void OnNetwrokRequestCompleted(NetworkRequestCompletedArgs args)
        {
            if (NetwrokRequestCompleted != null)
            {
                NetwrokRequestCompleted(this, args);
            }
        }

		protected void OnParameterChanged(ParameterChangedEventArgs args)
		{
			if (ParameterChanged != null)
			{
				ParameterChanged(this, args);
			}
		}

		protected void OnErrorOccured(NetworkControllerErrorOccuredEventArgs args)
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
        protected abstract void NetworkPollingAction(Object cancellationToken);

        /// <summary>
        /// Записывает транзакцию в буфер исходящих сообщений
        /// </summary>
        /// <param name="request"></param>
        /// <param name="isExternalCall"></param>
        public abstract IAsyncRequestResult Write(NetworkRequest request, bool isExternalCall);

        /// <summary>
        /// Синхронизирует время в сети во всех устройтсвах
        /// </summary>
		/// <param name="broadcastAddress">Широковещательный адрес данной системы</param>
        public abstract void SyncDateTime(ValueType broadcastAddress);

		public abstract void SyncDateTime();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="commandName"></param>
		public virtual void ExecuteCommand(string commandName) {}

		public abstract OperationResult ReadParameter(string parameterName);

		public abstract void WriteParameter(string parameterName, ValueType value);

		#endregion

        #region Events

		public event EventHandler StatusChanged;
        public event EventHandler<NetworkRequestCompletedArgs> NetwrokRequestCompleted;
		public event EventHandler<ParameterChangedEventArgs> ParameterChanged;
		public event EventHandler<ConfigurationChangedEventArgs> ConfigurationChanged;
		public event EventHandler<NetworkControllerErrorOccuredEventArgs> ErrorOccurred;

		#endregion
	}
}
