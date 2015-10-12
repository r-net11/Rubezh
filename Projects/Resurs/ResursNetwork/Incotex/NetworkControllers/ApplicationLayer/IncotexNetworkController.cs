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
using ResursNetwork.OSI.Messages.Transactions;
using ResursNetwork.OSI.ApplicationLayer.Devices;
using ResursNetwork.OSI.ApplicationLayer.Devices.Collections.ObjectModel;
using ResursNetwork.Management;
using ResursNetwork.Incotex.Models;
using ResursNetwork.Incotex.NetworkControllers.Messages;
using Common;

namespace ResursNetwork.Incotex.NetworkControllers.ApplicationLayer
{
    /// <summary>
    /// Сетевой контроллер для работы со устройствами производства Incotex
    /// </summary>
    public class IncotexNetworkController: NetworkControllerBase
    {
        /// <summary>
        /// Класс буфера исходящих запросов к удалённым устройтсвам
        /// </summary>
        internal class OutputBuffer
        {
            #region Fields And Properties

            /// <summary>
            /// Буфер исходящих запросов, для внешних вызовов
            /// (со стороны UI интерфейса и т.п.) Данные запросы
            /// имеют приоритет над внутренними вызовами 
            /// </summary>
            private Queue<NetworkRequest> _OutputBufferExternalCalls = 
                new Queue<NetworkRequest>();

            /// <summary>
            /// Буфер исходящих запросов, для внутренних вызовов
            /// (переодический автоматизированный опрос удалённых устройств)
            /// </summary>
            private Queue<NetworkRequest> _OutputBufferInternalCalls = 
                new Queue<NetworkRequest>();
            
            /// <summary>
            /// Возвращает состояние исходящего буфера (наличие в нём запросов)
            /// </summary>
            internal bool IsEmpty
            {
                get { return ((_OutputBufferExternalCalls.Count == 0) && 
                    (_OutputBufferInternalCalls.Count == 0)) ? true : false; }
            }

            internal int Count
            {
                get { return _OutputBufferExternalCalls.Count + _OutputBufferInternalCalls.Count; }
            }
            #endregion

            /// <summary>
            /// Записывает запрос в выходной буфер 
            /// </summary>
            /// <param name="request">Сетевой запрос</param>
            /// <param name="isExternalCall">Признак внешнего вызова</param>
            internal void Enqueue(NetworkRequest request, bool isExternalCall) 
            { 
                if (isExternalCall)
                {
                    _OutputBufferExternalCalls.Enqueue(request);
                }
                else
                {
                    _OutputBufferInternalCalls.Enqueue(request);
                }
            }

            /// <summary>
            /// Читает запрос из выходного буфера
            /// </summary>
            /// <returns>null- если буфер пуст</returns>
            internal NetworkRequest Dequeue()
            {
                // Вынешние вызовы имеют приоритет перед внутренними. Поэтому,
                // выбираем сообщение сначала из буфера внешних вызовов и только,
                // затем, если этот буфер пуст, выбирает из буфера внутренних вызовов 
                if (_OutputBufferExternalCalls.Count != 0)
                {
                    return _OutputBufferExternalCalls.Dequeue();
                }
                
                if ( _OutputBufferInternalCalls.Count != 0)
                {
                    return _OutputBufferInternalCalls.Dequeue();
                }

                return null;
            }
        }

        #region Fields And Properties

        private static DeviceType[] _SupportedDevices = new DeviceType[] { DeviceType.Mercury203 };
        private static Type[] _SupportedInterfaces = new Type[] { typeof(Incotex.NetworkControllers.DataLinkLayer.ComPort) };
        private NetworkRequest _CurrentNetworkRequest;
        private int _RequestTimeout = 2000; // Значение по умолчанию
        private int _BroadcastRequestDelay = 2000; // Значение по умолчанию
        private System.Timers.Timer _DataSyncTimer;
        private int _TotalAttempts = 1;
        private AutoResetEvent _AutoResetEvent;
        private static object _SyncRoot = new object(); 
        private OutputBuffer _OutputBuffer;

        /// <summary>
        /// Хранит устройтво для работы в монопольном режиме доступа к устройству
        /// </summary>
        //private DeviceBase _DeviceForExclusiveMode; 

        /// <summary>
        /// Хранит входящее сообщение от удалённого устройтсва во 
        /// время действия сетевой транзакции
        /// </summary>
        private DataMessage _CurrentIncomingMessage;

        /// <summary>
        /// Возвращает состояние текущей сетевой транзакции
        /// </summary>
        public NetworkRequest CurrentNetworkRequest
        {
            get { return _CurrentNetworkRequest; }
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

        /// <summary>
        /// Выдержка времени (мсек) после выполнения широковешательного запроса
        /// </summary>
        public int BroadcastRequestDelay
        {
            get { return _BroadcastRequestDelay; }
            set 
            {
                if (value > 0)
                {
                    _BroadcastRequestDelay = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("BroadcastRequestDelay", 
                        "Недопустимое значение параметра");
                }
            }
        }

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

        /// <summary>
        /// Период (мсек) получения данных от удалённых устройтв
        /// </summary>
        public double DataSyncPeriod
        {
            get { return _DataSyncTimer.Interval; }
            set { _DataSyncTimer.Interval = value; }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор
        /// </summary>
        public IncotexNetworkController()
        {
            _OutputBuffer = new OutputBuffer();
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
                    methodInfo.Invoke(device, new object[0]);
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
                    _Id, msg.Code, msg.Description));

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

            if ((_CurrentNetworkRequest == null) || 
                (_CurrentNetworkRequest.Status != NetworkRequestStatus.Running))
            {
                throw new Exception("Принято сообщение в отсутствии запроса");
            }

            // Обрабатывает сообщение
            _AutoResetEvent.Set();
            _CurrentIncomingMessage = dataMessages[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancelToken"></param>
        protected override void NetwokPollingAction(object cancelToken)
        {
            NetworkRequest networkRequest;

            var cancel = (CancellationToken)cancelToken;
            cancel.ThrowIfCancellationRequested();

            while (!cancel.IsCancellationRequested)
            {
                while (_OutputBuffer.Count > 0)
                {
                    lock (_SyncRoot)
                    {
                        networkRequest = _OutputBuffer.Dequeue();

                        if (Status != Status.Running)
                        {
                            networkRequest.CurrentTransaction.Start();
                            networkRequest.CurrentTransaction.Abort("Невозможно выполнить запрос, контроллер сети остановлен");
                            continue;
                        }
                    }
                    if (cancel.IsCancellationRequested)
                    {
                        networkRequest.CurrentTransaction.Start();
                        networkRequest.CurrentTransaction.Abort("Выполнение запроса прервано по требованию");
                    }
                    ExecuteTransaction(networkRequest);

                }
                Thread.Sleep(200);
            }
        }

        public override IAsyncRequestResult Write(NetworkRequest networkRequest, bool isExternalCall = true)
        {
            lock (_SyncRoot)
            {
                if (Status == Status.Running)
                {
                    networkRequest.TotalAttempts = TotalAttempts;
                    _OutputBuffer.Enqueue(networkRequest, isExternalCall);
                    return (IAsyncRequestResult)networkRequest.AsyncRequestResult;
                }
                else
                {
                    throw new InvalidOperationException("Невозможно выполнить операцию. Контроллер остановлен");
                }
            }
        }

        /// <summary>
        /// Выполняет сетевую транзакцию 
        /// </summary>
        /// <param name="networkRequest"></param>
        public void ExecuteTransaction(NetworkRequest networkRequest)
        {
            if ((CurrentNetworkRequest != null) &&
                (CurrentNetworkRequest.Status == NetworkRequestStatus.Running))
            {
                throw new InvalidOperationException(
                    "Попытка выполнить транзакцию во время действия другой");
            }

            if ((networkRequest.CurrentTransaction.TransactionType != TransactionType.BroadcastMode) &&
                (networkRequest.CurrentTransaction.TransactionType != TransactionType.UnicastMode))
            {
                networkRequest.CurrentTransaction.Start();
                networkRequest.CurrentTransaction.Abort("Попытка запустить сетевую транзакцию с недопустимым типом");
                throw new InvalidOperationException(
                    String.Format("Попытка запустить сетевую транзакцию с недопустимым типом: {0}",
                    networkRequest.CurrentTransaction.TransactionType));
            }

            // Устанавливаем транзакцию в качестве текущей
            _CurrentNetworkRequest = networkRequest;
            var result = _CurrentNetworkRequest.AsyncRequestResult;

            // Если запрос адресованный, то ждём ответа
            // Если запрос широковещательный выдерживаем установленную паузу
            switch (_CurrentNetworkRequest.CurrentTransaction.TransactionType)
            {
                case TransactionType.UnicastMode:
                    {
                        var disconnected = false;

                        for (int i = 0; i < TotalAttempts; i++)
                        {
                            // Отправляем запрос к удалённому устройтву
                            _CurrentNetworkRequest.CurrentTransaction.Start();
                            _Connection.Write(_CurrentNetworkRequest.CurrentTransaction.Request);

                            // Ждём ответа от удалённого устройтва или тайм аут
                            if (!_AutoResetEvent.WaitOne(_RequestTimeout))
                            {
                                // TimeOut!!! Прекращает текущую транзакцию
                                _CurrentNetworkRequest.CurrentTransaction.Abort("Request timeout");
                                
                                Transaction trn;
                                // Повторяем запрос
                                _CurrentNetworkRequest.NextAttempt(out trn);
                                disconnected = true;
                                continue;                               
                            }
                            else
                            {
                                // Ответ получен
                                _CurrentNetworkRequest.CurrentTransaction.Stop(_CurrentIncomingMessage);
                                _CurrentIncomingMessage = null;
                                disconnected = false;
                                break;
                            }
                        }
                        
                        // Кол-во попыток доступа к устройтсву исчерпано
                        if (disconnected)
                        {

                            //var errors = ((DeviceBase)_CurrentNetworkRequest.CurrentTransaction.Sender).Errors;
                            //errors.CommunicationError = true;
                            //((DeviceBase)_CurrentNetworkRequest.CurrentTransaction.Sender).SetError(errors);
                        }
                        else
                        {
                            // Если ранее была установлена ошибка связи, то сбрасываем её
                            //var errors = ((DeviceBase)_CurrentNetworkRequest.CurrentTransaction.Sender).Errors;
                            //if (errors.CommunicationError == true)
                            //{
                            //    errors.CommunicationError = false;
                            //    ((DeviceBase)_CurrentNetworkRequest.CurrentTransaction.Sender).SetError(errors);
                            //}
                        }

                        OnNetwrokRequestCompleted(
                            new NetworkRequestCompletedArgs { NetworkRequest = _CurrentNetworkRequest });

                        result.SetCompleted(_CurrentNetworkRequest.TransactionsStack);

                        break;
                    }
                case TransactionType.BroadcastMode:
                    {
                        // Отправляем запрос к удалённому устройтву
                        _CurrentNetworkRequest.CurrentTransaction.Start();
                        _Connection.Write(_CurrentNetworkRequest.CurrentTransaction.Request);

                        if (!_AutoResetEvent.WaitOne(_BroadcastRequestDelay))
                        {
                            _CurrentNetworkRequest.CurrentTransaction.Stop(null);
                        }
                        else
                        {
                            _CurrentIncomingMessage = null;
                            throw new Exception(
                                "Принят ответ от удалённого устройтства во время широковещательного запроса");
                        }

                        result.SetCompleted();

                        break;
                    }
                default:
                    {
                        result.SetCompleted();
                        throw new NotSupportedException();
                    }
            }
        }

        public override void SyncDateTime()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Events
        #endregion
    }
}
