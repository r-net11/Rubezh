using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResursNetwork.OSI.ApplicationLayer.Devices;
using ResursNetwork.OSI.ApplicationLayer.Devices.ValueConverters;
using ResursNetwork.OSI.Messages;
using ResursNetwork.OSI.Messages.Transactions;
using ResursNetwork.OSI.ApplicationLayer;
using ResursNetwork.Incotex.Models.DateTime;
using ResursNetwork.Incotex.NetworkControllers.Messages;
using ResursNetwork.Incotex.NetworkControllers.ApplicationLayer;
using ResursNetwork.Management;
using ResursAPI.ParameterNames;
using Common;

namespace ResursNetwork.Incotex.Models
{
    /// <summary>
    /// Модель данных счётчика Меркурий M203
    /// </summary>
    public class Mercury203: DeviceBase
    {
        #region Fields And Properties

        /// <summary>
        /// Хранит все активные операции по данному устройтву
        /// </summary>
        private List<NetworkRequest> _ActiveRequests = new List<NetworkRequest>();

        public override DeviceType DeviceType
        {
            get { return DeviceType.Mercury203; }
        }

        #endregion

        #region Constructors

        public Mercury203(): base()
        {
        }

        #endregion

        #region Methods
        /// <summary>
        /// Инициализирует список свойств для конкретного устройства
        /// </summary>
        protected override void Initialization()
        {
            _Parameters.Add(new Parameter(typeof(UInt32))
            {
                Name = ParameterNamesMercury203.GADDR,
                Description = "Групповой адрес счётчика",
                PollingEnabled = true,
                ReadOnly = false,
                ValueConverter = new BigEndianUInt32ValueConverter(),
                Value = (UInt32)0
            });

            _Parameters.Add(new Parameter(typeof(IncotexDateTime)) 
            {
                Name = ParameterNamesMercury203.DateTime, 
                Description = "Текущее значение часов счётчика",
                PollingEnabled = true, 
                ReadOnly = false,
                ValueConverter = new IncotexDataTimeTypeConverter(),
                Value = new IncotexDateTime()
            });

            _Parameters.Add(new Parameter(typeof(UInt16)) 
            {
                Name = ParameterNamesMercury203.PowerLimit,
                Description = "Значение лимита мощности",
                PollingEnabled = true,
                ReadOnly = false,
                ValueConverter = new BigEndianUInt16ValueConvertor(),
                Value = (UInt16)0
            });
        }

        private void GetAnswer(NetworkRequest networkRequest)
        {
            var request = (DataMessage)networkRequest.Request.Request;
            //ищем устройтво
            var device = (Mercury203)_NetworkController.Devices[request.Address];

            switch ((Mercury203CmdCode)request.CmdCode)
            {
                case Mercury203CmdCode.SetNetworkAddress:
                    {
                        GetAnswerNetwokAdderss(networkRequest); break;
                    }
                case Mercury203CmdCode.ReadGroupAddress:
                    {
                        GetReadGroupAddress(networkRequest); break;
                    }
                default:
                    {
                        throw new NotImplementedException(
                            String.Format("Устройтво Mercury 203 не поддерживает команду с кодом {0}",
                            request.CmdCode));
                    }
            }
        }

        public override void EventHandler_NetworkController_NetwrokRequestCompleted(
            object sender, NetworkRequestCompletedArgs e)
        {
            // Ищем запрос в буфере
            var request = _ActiveRequests.FirstOrDefault(r => r.Id == e.NetworkRequest.Id);

            if (request == null)
            {
                return;
            }

            switch (request.Status)
            {
                case NetworkRequestStatus.Completed:
                    {
                        if (Errors.CommunicationError)
                        {
                            _Errors.CommunicationError = true;
                            OnErrorOccurred(new ErrorOccuredEventArgs { Errors = _Errors });
                        }
                        // Разбираем транзакцию
                        GetAnswer(e.NetworkRequest);
                        break;
                    }
                case NetworkRequestStatus.Failed:
                    {
                        if (!Errors.CommunicationError)
                        {
                            _Errors.CommunicationError = true;
                            OnErrorOccurred(new ErrorOccuredEventArgs { Errors = _Errors });
                        }
                        // Записываем в журнал причину
                        //TODO: Logger.Error(transaction.ToString());
                        break;
                    }
                default:
                    {
                        // Другие варианты в принципе не возможны...
                        throw new Exception();
                    }
            }

        }
        #endregion

        #region Network API

        /// <summary>
        /// Установка нового сетевого адреса счетчика 
        /// </summary>
        /// <param name="addr">Текущий сетевой адрес счётчика</param>
        /// <param name="newaddr">Новый сетевой адрес счётчика</param>
        /// <returns></returns>
        public Transaction SetNewAddress(UInt32 addr, UInt32 newaddr)
        {
            var request = new DataMessage()
            {
                Address = addr,
                CmdCode = Convert.ToByte(Mercury203CmdCode.SetNetworkAddress)
            };
            var transaction = new Transaction(this, TransactionType.UnicastMode, request);
            var networkRequest = new NetworkRequest(transaction);

            if (_NetworkController == null)
            {
                transaction.Start();
                transaction.Abort("Невозможно выполенить запрос. Не установлен контроллер сети");
            }
            else
            {
                _ActiveRequests.Add(networkRequest);
                _NetworkController.Write(networkRequest);
            }
            return transaction;
        }

        /// <summary>
        /// Разбирает ответ от удалённого устройтва по запросу SetNewAddress
        /// </summary>
        /// <param name="networkRequest"></param>
        private void GetAnswerNetwokAdderss(NetworkRequest networkRequest)
        {
            var request = (DataMessage)networkRequest.Request.Request;

            // Разбираем ответ
            if (networkRequest.Status == NetworkRequestStatus.Completed)
            {
                var requestArray = networkRequest.Request.Request.ToArray();
                var answerArray = networkRequest.CurrentTransaction.Answer.ToArray();
                var command = _ActiveRequests.FirstOrDefault(
                    p => p.Id == networkRequest.Id);

                if (command == null)
                {
                    throw new Exception("Не найдена команда с указанной транзакцией");
                }

                if (answerArray.Length != 7)
                {
                    //command.Status = Result.Error;
                    //command.ErrorDescription = "Неверная длина ответного сообщения";
                    //OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
                    //TODO: Пишем в лог
                    _ActiveRequests.Remove(command);
                }

                if (answerArray[4] != request.CmdCode)
                {
                    //command.Status = Result.Error;
                    //command.ErrorDescription = "Код команды в ответе не соответствует коду в запросе";
                    //OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
                    //TODO: Пишем в лог
                    _ActiveRequests.Remove(command);
                }

                // Проверяем новый адрес в запросе и в ответе
                if ((requestArray[6] != answerArray[0]) ||
                    (requestArray[7] != answerArray[1]) ||
                    (requestArray[8] != answerArray[2]) ||
                    (requestArray[9] != answerArray[3]))
                {
                    //command.Status = Result.Error;
                    //command.ErrorDescription =
                    //    "Новый адрес счётчика в ответе не соответствует устанавливаемому";
                    //OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
                    //TODO: Пишем в лог
                    _ActiveRequests.Remove(command);
                }
                
                //Всё в порядке выполняем изменение сетевого адреса
                var converter = new BigEndianUInt32ValueConverter();
                var adr = (UInt32)converter.FromArray(
                    new Byte[] { answerArray[0], answerArray[1], answerArray[2], answerArray[3] });

                Address = adr;
                //command.Status = Result.OK;
                _ActiveRequests.Remove(command);
            }
            else
            {
                // Транзакция выполнена с ошибкам
                var command = _ActiveRequests.FirstOrDefault(
                    p => p.Id == networkRequest.Id);

                //command.Status = Result.Error;
                //OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
                //TODO: Пишем в лог
                _ActiveRequests.Remove(command);
            }
        }

        /// <summary>
        /// Чтение группового адреса счетчика (CMD=20h)
        /// </summary>
        [PeriodicReadEnabled]
        public Transaction ReadGroupAddress()
        {
            var request = new DataMessage()
            {
                Address = Address,
                CmdCode = Convert.ToByte(Mercury203CmdCode.ReadGroupAddress)
            };
            var transaction = new Transaction(this, TransactionType.UnicastMode, request);
            var networkRequest = new NetworkRequest(transaction);

            if (_NetworkController == null)
            {
                transaction.Start();
                transaction.Abort("Невозможно выполенить запрос. Не установлен контроллер сети");
            }
            else
            {
                _ActiveRequests.Add(networkRequest);
                _NetworkController.Write(networkRequest);
            }
            return transaction;
        }

        /// <summary>
        /// Разбирает ответ от удалённого устройтва 
        /// по запросу ReadGroupAddress
        /// </summary>
        /// <param name="transaction"></param>
        private void GetReadGroupAddress(NetworkRequest networkRequest)
        {
            // Разбираем ответ
            if (networkRequest.Status == NetworkRequestStatus.Completed)
            {
                var command = _ActiveRequests.FirstOrDefault(
                    p => p.Id == networkRequest.Id);

                if (command == null)
                {
                    throw new Exception("Не найдена команда с указанной транзакцией");
                }

                if (networkRequest.CurrentTransaction.Answer.ToArray().Length != 11)
                {
                    //command.Status = Result.Error;
                    //command.ErrorDescription = "Неверная длина ответного сообщения";
                    //OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
                    //TODO:
                    _ActiveRequests.Remove(command);
                }

                var request = (DataMessage)networkRequest.Request.Request;
                var answer = (DataMessage)networkRequest.CurrentTransaction.Answer;

                // Проверяем новый адрес в запросе и в ответе
                if (request.Address != answer.Address)
                {
                    //command.Status = Result.Error;
                    //command.ErrorDescription = "Адрес команды в ответе не соответствует адресу в запросе";
                    //OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
                    //TODO:
                    _ActiveRequests.Remove(command);
                }

                if (answer.CmdCode != request.CmdCode)
                {
                    //command.Status = Result.Error;
                    //command.ErrorDescription = "Код команды в ответе не соответствует коду в запросе";
                    //OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
                    //TODO:
                    _ActiveRequests.Remove(command);
                }

                // Получаем параметр
                // Присваиваем новое значение параметру
                var parameter = _Parameters[ParameterNamesMercury203.GADDR];
                parameter.Value = parameter.ValueConverter.FromArray(
                    new byte[] 
                    {
                        answer.Data[0],
                        answer.Data[1],
                        answer.Data[2],
                        answer.Data[3]
                    });

                //command.Status = Result.OK;
                _ActiveRequests.Remove(command);
            }
            else
            {
                // Транзакция выполнена с ошибкам
                var command = _ActiveRequests.FirstOrDefault(
                    p => p.Id == networkRequest.Id);
                //command.Status = Result.Error;
                //OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
                //TODO:
                _ActiveRequests.Remove(command);
            }
        }

        /// <summary>
        /// Чтение внутренних часов и календаря счетчика (CMD=21h)
        /// </summary>
        /// <returns></returns>
        [PeriodicReadEnabled]
        public Transaction ReadDateTime()
        {
            var request = new DataMessage()
            {
                Address = Address,
                CmdCode = Convert.ToByte(Mercury203CmdCode.ReadGroupAddress)
            };
            var transaction = new Transaction(this, TransactionType.UnicastMode, request);
            var networkRequest = new NetworkRequest(transaction);

            if (_NetworkController == null)
            {
                transaction.Start();
                transaction.Abort("Невозможно выполенить запрос. Не установлен контроллер сети");
            }
            else
            {
                _ActiveRequests.Add(networkRequest);
                _NetworkController.Write(networkRequest);
            }
            return transaction;
        }

        /// <summary>
        /// Чтение лимита мощности (CMD=22h)
        /// </summary>
        /// <returns></returns>
        [PeriodicReadEnabled]
        public Transaction ReadPowerLimit()
        {
            var request = new DataMessage()
            {
                Address = Address,
                CmdCode = Convert.ToByte(Mercury203CmdCode.ReadPowerLimit)
            };
            var transaction = new Transaction(this, TransactionType.UnicastMode, request);
            var networkRequest = new NetworkRequest(transaction);

            if (_NetworkController == null)
            {
                transaction.Start();
                transaction.Abort("Невозможно выполенить запрос. Не установлен контроллер сети");
            }
            else
            {
                _ActiveRequests.Add(networkRequest);
                _NetworkController.Write(networkRequest);
            }
            return transaction;
        }

        /// <summary>
        /// Чтение лимита энергии за месяц
        /// </summary>
        /// <returns></returns>
        [PeriodicReadEnabled]
        public Transaction ReadPowerLimitPerMonth()
        {
            var request = new DataMessage()
            {
                Address = Address,
                CmdCode = Convert.ToByte(Mercury203CmdCode.ReadPowerLimitPerMonth)
            };
            var transaction = new Transaction(this, TransactionType.UnicastMode, request);
            var networkRequest = new NetworkRequest(transaction);

            if (_NetworkController == null)
            {
                transaction.Start();
                transaction.Abort("Невозможно выполенить запрос. Не установлен контроллер сети");
            }
            else
            {
                _ActiveRequests.Add(networkRequest);
                _NetworkController.Write(networkRequest);
            }
            return transaction;
        }

        #endregion
    }
}
