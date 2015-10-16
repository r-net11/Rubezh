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
using ResursAPI.Models;
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

        private static DeviceType[] _SupportedDevices = 
            new DeviceType[] { DeviceType.Mercury203 };
        private static Type[] _SupportedInterfaces = 
            new Type[] { typeof(Incotex.NetworkControllers.DataLinkLayer.ComPort) };
        private NetworkRequest _CurrentNetworkRequest;
        private int _RequestTimeout = 2000; // Значение по умолчанию
        private int _BroadcastRequestDelay = 2000; // Значение по умолчанию
        private AutoResetEvent _AutoResetEventRequest = new AutoResetEvent(false);
        private AutoResetEvent _AutoResetEventWorker = new AutoResetEvent(false);
        private static object _SyncRoot = new object(); 
        private OutputBuffer _OutputBuffer = new OutputBuffer();
        private int _DataSyncPeriod = 
            Convert.ToInt32(TimeSpan.FromDays(1).TotalMilliseconds); // По умолчнию период синхронизации 1 день;

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
        /// Период (мсек) получения данных от удалённых устройтв
        /// </summary>
        public int DataSyncPeriod
        {
            get { return _DataSyncPeriod; }
            set 
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(
                        "DataSyncPeriod", "Значение не должно быть меньше 1");
                }

                _DataSyncPeriod = value; 
            }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор
        /// </summary>
        public IncotexNetworkController()
        {
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
                    methodInfo.Invoke(device, new object[] { false });
                }
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
            _AutoResetEventRequest.Set();
            _CurrentIncomingMessage = dataMessages[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        protected override void NetwokPollingAction(object cancellationToken)
        {
            //DateTime lastUpdate;
            List<IDevice> faultyDevices = new List<IDevice>();
            NetworkRequest networkRequest;

            var cancel = (CancellationToken)cancellationToken;
            cancel.ThrowIfCancellationRequested();

            while(!cancel.IsCancellationRequested)
            {
                if (!_AutoResetEventWorker.WaitOne(Convert.ToInt32(DataSyncPeriod)))
                {
                    // При срабатывании по таймауту обновляем данные из удалённых устройтств
                    // При условии что контроллер в активном состоянии
                    foreach (DeviceBase device in _Devices)
                    {
                        ReadDeviceParameters(device);
                    }
                }

                // Выполняем все запросы в буфере
                while (_OutputBuffer.Count > 0)
                {
                    lock (_SyncRoot)
                    {
                        networkRequest = _OutputBuffer.Dequeue();

                        if (Status != Status.Running)
                        {
                            networkRequest.CurrentTransaction.Start();
                            networkRequest.CurrentTransaction.Abort(
                                new TransactionError
                                {
                                    ErrorCode = TransactionErrorCodes.DataLinkPortNotInstalled,
                                    Description = "Невозможно выполенить запрос. Не установлен контроллер сети"
                                });
                            continue;
                        }
                    }
                    
                    if (cancel.IsCancellationRequested)
                    {
                        networkRequest.CurrentTransaction.Start();
                        networkRequest.CurrentTransaction.Abort(new TransactionError
                        {
                            ErrorCode = TransactionErrorCodes.RequestWasCancelled,
                            Description = "Выполнение запроса прервано по требованию"
                        });
                        continue;
                    }

                    // Проверяем устройство. Если оно в списке
                    if (faultyDevices.Contains(networkRequest.CurrentTransaction.Sender))
                    {
                        // Если устройство содежится в списке неисправных, то пропускаем его
                        networkRequest.CurrentTransaction.Start();
                        networkRequest.CurrentTransaction.Abort(new TransactionError
                        {
                            ErrorCode = TransactionErrorCodes.RequestTimeout,
                            Description =
                            "Исключено из обработки по причине неудачного предыдущего запроса к этому устройтсву"
                        });
                        continue;
                    }

                    // Выполняем сетевой запрос
                    ExecuteTransaction(networkRequest);

                    var result = networkRequest.AsyncRequestResult;

                    if (!result.IsCompleted)
                    {
                        throw new Exception("Сетевой запрос не выполнен. Это невозможно и никогда не должно появиться!!!");
                    }

                    if (result.HasError)
                    {
                        if (result.LastTransaction.Error.ErrorCode == TransactionErrorCodes.RequestTimeout)
                        {
                            // Запоминаем данное устройство, для того, что бы игнорировать 
                            // все последующие запросы от данного устройства
                            if (!faultyDevices.Contains(result.Sender))
                            {
                                faultyDevices.Add(result.Sender);
                                //Установить ошибку в данном устройстве
                                result.Sender.CommunicationError = true;
                            }
                        }
                    }
                    else
                    {
                        // Проверяем: если запрос выполен успешно, но устройтсво содежит ошибку ComunicationError,
                        // то считаем, что связь с устройтсвом восстановилась и убираем данную ошибку
                        if (result.Sender.Errors.CommunicationError)
                        {
                            // удаляем данное устройтсво из списка неисправных
                            if (faultyDevices.Contains(result.Sender))
                            {
                                faultyDevices.Remove(result.Sender);
                            }
                            // сбросить ошибку в данном устройстве
                            result.Sender.CommunicationError = false;
                        }
                    }
                }

            }
        }

        public override IAsyncRequestResult Write(
            NetworkRequest networkRequest, bool isExternalCall = true)
        {
            lock (_SyncRoot)
            {
                if (Status == Status.Running)
                {
                    networkRequest.TotalAttempts = TotalAttempts;
                    _OutputBuffer.Enqueue(networkRequest, isExternalCall);
                    _AutoResetEventWorker.Set();
                    return (IAsyncRequestResult)networkRequest.AsyncRequestResult;
                }
                else
                {
                    throw new InvalidOperationException(
                        "Невозможно выполнить операцию. Контроллер остановлен");
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
                networkRequest.CurrentTransaction.Abort(new TransactionError
                {
                    ErrorCode = TransactionErrorCodes.TransactionTypeIsWrong,
                    Description = "Попытка запустить сетевую транзакцию с недопустимым типом"
                });
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
                            if (!_AutoResetEventRequest.WaitOne(_RequestTimeout))
                            {
                                // TimeOut!!! Прекращает текущую транзакцию
                                _CurrentNetworkRequest.CurrentTransaction.Abort(new TransactionError
                                {
                                    ErrorCode = TransactionErrorCodes.RequestTimeout,
                                    Description = "Request timeout"
                                });
                                
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

                        if (!_AutoResetEventRequest.WaitOne(_BroadcastRequestDelay))
                        {
                            _CurrentNetworkRequest.CurrentTransaction.Stop(null);
                        }
                        else
                        {
                            _CurrentIncomingMessage = null;
                            throw new Exception(
                                "Принят ответ от удалённого устройтства во время широковещательного запроса");
                        }

                        result.SetCompleted(_CurrentNetworkRequest.TransactionsStack);

                        break;
                    }
                default:
                    {
                        result.SetCompleted(_CurrentNetworkRequest.TransactionsStack);
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
