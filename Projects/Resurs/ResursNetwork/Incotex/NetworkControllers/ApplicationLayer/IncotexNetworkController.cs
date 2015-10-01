using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using System.Timers;
using ResursNetwork.OSI.ApplicationLayer;
using ResursNetwork.OSI.DataLinkLayer;
using ResursNetwork.OSI.Messages;
using ResursNetwork.OSI.Messages.Transaction;
using ResursNetwork.Devices;
using ResursNetwork.Devices.Collections.ObjectModel;
using ResursNetwork.Management;
using ResursNetwork.Incotex.Model;
using ResursNetwork.Incotex.NetworkControllers.Messages;
using Common;

namespace ResursNetwork.Incotex.NetworkControllers.ApplicationLayer
{
    /// <summary>
    /// Сетевой контроллер для работы со устройствами производства Incotex
    /// </summary>
    public class IncotexNetworkController: NetworkControllerBase
    {
        #region Fields And Properties
        private static DeviceType[] _SupportedDevices = new DeviceType[] { DeviceType.Mercury203 };
        private static Type[] _SupportedInterfaces = new Type[] { typeof(Incotex.NetworkControllers.DataLinkLayer.ComPort) }; 

        private Transaction _CurrentTransaction;
        /// <summary>
        /// Возвращает состояние текущей сетевой транзакции
        /// </summary>
        public Transaction CurrentTransaction
        {
            get { return _CurrentTransaction; }
        }
        /// <summary>
        /// Готовность котроллера выполнить сетевую транзакцию.
        /// </summary>
        public Boolean IsReady
        {
            get
            {
                return ((Status == Status.Running) || (CurrentTransaction == null) || 
                    (!CurrentTransaction.IsRunning)) ? true : false;
            }
        }
        /// <summary>
        /// Хранит устройтво для работы в монопольном режиме доступа к устройству
        /// </summary>
        private DeviceBase _DeviceForExclusiveMode; 
        /// <summary>
        /// Возвращает режим опроса сетевых устройств:
        /// true - опрос единственног устройства в монопольном режиме (режим рельного времени)
        /// false - опрос всех устройтсв в назначенное время (общий режим работы)
        /// </summary>
        public Boolean PollingExclusiveMode
        {
            get { return _DeviceForExclusiveMode == null ? false : true; }
        }
        public override IEnumerable<DeviceType> SuppotedDevices
        {
            get { return _SupportedDevices; }
        }
        public override IDataLinkPort Connection
        {
            get
            {
                return base.Connection;
            }
            set
            {
                if (value != null)
                {
                    // Закоментировал из unit-тестов
                    //if (!_SupportedInterfaces.Contains(value.GetType()))
                    //{
                    //    throw new NotSupportedException("Данный интерфейс не поддреживается контроллером");
                    //}
                }
                base.Connection = value;
            }
        }

        private int _RequestTimeout = 1000; // Значение по умолчанию
        /// <summary>
        /// Время (мсек), в течении которого удалённое 
        /// должно ответить на запрос
        /// </summary>
        public int RequestTimeout
        {
            get { return _RequestTimeout; }
            set 
            {
                if (value > 0)
                {
                    _RequestTimeout = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        private int _BroadcastRequestDelay = 2000; // Значение по умолчанию
        /// <summary>
        /// Выдержка времени после выполнения широковешательного запроса
        /// </summary>
        public int BroadcastRequestDelay
        {
            get { return _BroadcastRequestDelay; }
            set { _BroadcastRequestDelay = value; }
        }

        private int _TotalAttempts;
        /// <summary>
        /// Количество попыток доспупа к устройтву
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

        //private AutoResetEvent _AutoResetEvent;

        private System.Timers.Timer _DataSyncTimer;

        /// <summary>
        /// Период (мсек) получения данных от удалённых устройтв
        /// </summary>
        public double DataSyncPeriod
        {
            get { return _DataSyncTimer.Interval; }
            set { _DataSyncTimer.Interval = value; }
        }

        private AutoResetEvent _AutoResetEvent;
        /// <summary>
        /// Буфер исходящих сообщений
        /// </summary>
        private Queue<Transaction> _OutputBuffer;

        private static object _SyncRoot = new object(); 
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор
        /// </summary>
        public IncotexNetworkController()
        {
            _OutputBuffer = new Queue<Transaction>();
            _AutoResetEvent = new AutoResetEvent(false);

            _DataSyncTimer = new System.Timers.Timer()
            {
                AutoReset = true,
                Interval = TimeSpan.FromDays(1).TotalMilliseconds, // По умолчнию период синхронизации 1 день
            };
            _DataSyncTimer.Elapsed += EventHandler_DataSyncTimer_Elapsed;
            _DataSyncTimer.Start();
        }


        #endregion

        #region Methods
        /// <summary>
        /// Принимает сетевое устройтсво ищет в нём методы с атрибутом 
        /// PeriodicPollingEnabledAttribute и выполняет их.
        /// </summary>
        /// <param name="device"></param>
        private void ReadDeviceParameters(DeviceBase device)
        {
            foreach (var methodInfo in device.GetType().GetMethods())
            {
                // Проверяем атрибут PeriodicPollingEnabledAttribute у метода
                // если присутствует, проверяем аргументы метода и тип возвращаемого значения
                if ((methodInfo.GetCustomAttributes(typeof(PeriodicReadEnabledAttribute), false).Length > 0) ||
                    (methodInfo.GetParameters().Length == 0) ||
                    (methodInfo.ReturnType == typeof(Transaction)))
                {
                    // Записываем транзакцию в выходной буфер
                    Write((Transaction)methodInfo.Invoke(device, new object[0]));
                }
            }
        }
        /// <summary>
        /// Обработчик события срабатываения таймера периодического опроса
        /// сетевых устройтств
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_DataSyncTimer_Elapsed(
            object sender, System.Timers.ElapsedEventArgs e)
        {
            // При срабатывании таймера обновляем данные из удалённых устройтств
            // При условии что контроллер в активном состоянии
            foreach(DeviceBase device in _Devices)
            {
                ReadDeviceParameters(device);
            }
        }
        /// <summary>
        /// Переводит контроллер в режим монопольного опроса указанного
        /// устройства (мониторинг параметров устройства в режиме рельного времени)
        /// </summary>
        /// <param name="device"></param>
        public void PollingExclusiveModeEnable(DeviceBase device)
        {
            // Проверяем устройство на пренадлежность данному контроллеру
            if (_Devices.Contains(device))
            {
                _DeviceForExclusiveMode = device;
            }
            else
            {
                throw new ArgumentException(
                    "Устройтво не принадлежит данному контроллеру", "device");
            }
        }
        /// <summary>
        /// Прекращает монопольный доступ к устройтву 
        /// </summary>
        public void PollingExclusiveModeDisable()
        {
            _DeviceForExclusiveMode = null;
        }
        /// <summary>
        /// Обработчик приёма сообщения из сети
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void EventHandler_Connection_MessageReceived(
            object sender, EventArgs e)
        {
            IDataLinkPort port = (IDataLinkPort)sender;
            var messages = new List<MessageBase>();
            
            // Читаем входящие сообщения из входного буфера порта 
            while(port.MessagesToRead > 0)
            {
                var msg = port.Read();
                messages.Add((MessageBase)msg);
            }

            // Обрабатываем сервисные сообщения
            var serviceErrorMessages = messages
                .Where(y => y.MessageType == MessageType.ServiceErrorMessage)
                .Select(z => (ServiceErrorMessage)z);

            foreach (var msg in serviceErrorMessages)
            {
                //запись в лог ошибок
                Logger.Error(String.Format("Controller Id={0} | Ошибка Code={1} | Description={2}",
                    _ControllerId, msg.Code, msg.Description));

                //TODO: Сделать обработчик ошибок, если потребуется
                //switch (msg.SpecificErrorCode)
                //{
                //    case ErrorCode.
                //}
            }

            // TODO: сделать сервистные сообщения, если понадобятся 
            //var serviceInfoMessages = messages
            //    .Where(y => y.MessageType == MessageType.ServiceInfoMessage)
            //    .Select(z => (....));

            var dataMessages = messages
                .Where(y => y.MessageType == MessageType.IncomingMessage)
                .Select(z => (DataMessage)z).ToArray();

            if (dataMessages.Length > 1)
            {
                throw new Exception(
                    "Сетевой контроллер принял одновременно более одного сообщения из сети");
            }

            if ((_CurrentTransaction == null) || 
                (_CurrentTransaction.Status != TransactionStatus.Running))
            {
                throw new Exception("Принято сообщение в отсутствии запроса");
            }

            // Обрабатывает сообщение
            //_AutoResetEvent.Set();
            _CurrentTransaction.Stop(dataMessages[0]);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancelToken"></param>
        protected override void NetwokPollingAction(object cancelToken)
        {
            var cancel = (CancellationToken)cancelToken;
            cancel.ThrowIfCancellationRequested();

            //var deviceContextList = _Devices.Select(p => new DeviceContext(p));

            while(!cancel.IsCancellationRequested)
            {
            //    // Делаем опрос устройства
            //    foreach(DeviceContext deviceContext in deviceContextList)
            //    {
            //        if (deviceContext.Cout > 0)
            //        {
            //            var p = deviceContext.Next();
                        
            //            if (p.PollingEnabled)
            //            {
            //                // TODO:
            //            }
            //        }
            //    }
                while (_OutputBuffer.Count > 0)
                {
                    Transaction trn;

                    if (cancel.IsCancellationRequested)
                    {
                        break;
                    }

                    lock(_SyncRoot)
                    {
                        trn = _OutputBuffer.Dequeue();
                    }
                    _Connection.Write(trn.Request);
                }
                Thread.Sleep(200);
            }
        }
        /// <summary>
        /// Выполняет сетевую транзакцию 
        /// </summary>
        /// <param name="trn"></param>
        private void Execute(Transaction trn)
        {
            if ((trn.TransactionType != TransactionType.BroadcastMode) ||
                (trn.TransactionType != TransactionType.UnicastMode))
            {
                throw new InvalidOperationException(
                    String.Format("Попытка запустить сетевую транзакцию с недопустимым типом: {}", 
                    trn.TransactionType));
            }

            // Устанавливаем транзакцию в качестве текущей
            _CurrentTransaction = trn;

            // Отправляем запрос к удалённому устройтву
            _CurrentTransaction.Start();
            _Connection.Write(_CurrentTransaction.Request);

            // Если запрос адресованный, то ждём ответа
            // Если запрос широковещательный выдерживаем установленную паузу
            switch (_CurrentTransaction.TransactionType)
            {
                case TransactionType.UnicastMode:
                    {
                        // Ждём ответа от удалённого устройтва или тайм аут
                        if (!_AutoResetEvent.WaitOne(_RequestTimeout))
                        {
                            // TimeOut!!! Прекращает текущую транзакцию
                            _CurrentTransaction.Abort("Request timeout");
                        }
                        break;
                    }
                case TransactionType.BroadcastMode:
                    {
                        Thread.Sleep(100);
                        _CurrentTransaction.Stop(null);
                        break;
                    }
                default:
                    {
                        throw new NotSupportedException();
                    }
            }
        }
        
        /// <summary>
        /// Записывает исходящее сообщение в выходной буфер
        /// </summary>
        /// <param name="transaction">Сетевая транзакция</param>
        public override void Write(Transaction transaction)
        {
            if (Status == Management.Status.Running)
            {
                lock(_SyncRoot)
                {
                    transaction.Start();
                    _CurrentTransaction = transaction;
                    //_OutputBuffer.Enqueue(transaction);
                    _Connection.Write(transaction.Request);
                }
            }
            else
            {
                throw new InvalidOperationException("Контроллер сети не активен");
            }
        }

        #endregion

        #region Events
        #endregion
    }
}
