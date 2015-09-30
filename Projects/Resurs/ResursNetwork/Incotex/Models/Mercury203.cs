using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResursNetwork.Devices;
using ResursNetwork.OSI.Messages;
using ResursNetwork.OSI.Messages.Transaction;
using ResursNetwork.OSI.ApplicationLayer;
using ResursNetwork.Incotex.NetworkControllers.Messages;
using ResursNetwork.Incotex.NetworkControllers.ApplicationLayer;
using Common;

namespace ResursNetwork.Incotex.Model
{
    /// <summary>
    /// Модель данных счётчика Меркурий M203
    /// </summary>
    public class Mercury203: DeviceBase
    {
        #region List of Parameter indexes
        /// <summary>
        /// Индексы параметров устройства
        /// </summary>
        public enum ParameterIndexes: uint
        {
            /// <summary>
            /// Групповой адрес устройтсва
            /// </summary>
            GADDR = 1,
            /// <summary>
            /// Дата и время устройства
            /// </summary>
            DATETIME = 2
        }
        #endregion

        #region Fields And Properties
        public override DeviceType DeviceType
        {
            get { return Devices.DeviceType.Mercury203; }
        }
        /// <summary>
        /// Хранит все активные операции по данному устройтву
        /// </summary>
        private List<DeviceCommand> _ActiveCommands = new List<DeviceCommand>();

        private EventHandler _TransactionHandler;

        #endregion

        #region Constructors
        public Mercury203(): base()
        {
            _TransactionHandler = 
                new EventHandler(EventHandler_TransactionWasEnded);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Инициализирует список свойств для конкретного устройства
        /// </summary>
        protected override void Initialization()
        {
            _Parameters.Add(new Parameter()
            {
                Index = (UInt32)ParameterIndexes.GADDR,
                Name = "Групповой адрес",
                Description = "Групповой адрес счётчика",
                PollingEnabled = true,
                ReadOnly = false,
                ValueConverter = null,
                Value = 0
            });
            _Parameters.Add(new Parameter() 
            { 
                Index = (UInt32)ParameterIndexes.DATETIME, 
                Name = "Дата и время", 
                Description = "Текущее значение часов счётчика",
                PollingEnabled = true, 
                ReadOnly = false,  
                ValueConverter = null,
                Value = 0
            });
        }
        /// <summary>
        /// Возвращает параметр по индексу
        /// </summary>
        /// <param name="index">Индекс параметра</param>
        /// <returns>null - если праметр не найден</returns>
        private Parameter GetParameter(ParameterIndexes index)
        {
            return _Parameters.FirstOrDefault(p => p.Index == (UInt32)index);
        }
        /// <summary>
        /// Обработчик завершения сетевой транзакции
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_TransactionWasEnded(object sender, EventArgs e)
        {
            var transaction = (Transaction)sender;

            switch (transaction.Status)
            {
                case TransactionStatus.Completed:
                    {
                        // Разбираем транзакцию
                        GetAnswer(transaction);
                        break;
                    }
                case TransactionStatus.Aborted:
                    {
                        // Записываем в журнал причину
                        Logger.Error(transaction.ToString());
                        break;
                    }
                default:
                    {
                        // Другие варианты в принципе не возможны...
                        throw new Exception();
                    }
            }
        }
        private void GetAnswer(Transaction transaction)
        {
            var request = (DataMessage)transaction.Request;
            //ищем устройтво
            var device = (Mercury203)_NetworkController.Devices[request.Address];

            switch ((Mercury203CmdCode)request.CmdCode)
            {
                case Mercury203CmdCode.SetNetworkAddress:
                    {
                        GetAnswerNetwokAdderss(transaction); break;
                    }
                default:
                    {
                        throw new NotImplementedException(
                            String.Format("Устройтво Mercury 203 не поддерживает команду с кодом {0}",
                            request.CmdCode));
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
            var transaction = new Transaction(TransactionType.UnicastMode, request);
            transaction.TransactionWasEnded += _TransactionHandler;
            _ActiveCommands.Add(new DeviceCommand() { Transaction = transaction });
            return transaction;
        }
        /// <summary>
        /// Разбирает ответ от удалённого устройтва по запросу SetNewAddress
        /// </summary>
        /// <param name="transaction"></param>
        private void GetAnswerNetwokAdderss(Transaction transaction)
        {
            var request = (DataMessage)transaction.Request;

            // Разбираем ответ
            if (transaction.Status == TransactionStatus.Completed)
            {
                var requestArray = transaction.Request.ToArray();
                var answerArray = transaction.Answer.ToArray();
                var command = _ActiveCommands.FirstOrDefault(
                    p => p.Transaction.Identifier == transaction.Identifier);

                if (command == null)
                {
                    throw new Exception("Не найдена команда с указанной транзакцией");
                }

                if (answerArray.Length != 7)
                {
                    command.Status = Result.Error;
                    command.ErrorDescription = "Неверная длина ответного сообщения";
                    OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
                    _ActiveCommands.Remove(command);
                }

                if (answerArray[4] != request.CmdCode)
                {
                    command.Status = Result.Error;
                    command.ErrorDescription = "Код команды в ответе не соответствует коду в запросе";
                    OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
                    _ActiveCommands.Remove(command);
                }

                // Проверяем новый адрес в запросе и в ответе
                if ((requestArray[6] != answerArray[0]) ||
                    (requestArray[7] != answerArray[1]) ||
                    (requestArray[8] != answerArray[2]) ||
                    (requestArray[9] != answerArray[3]))
                {
                    command.Status = Result.Error;
                    command.ErrorDescription =
                        "Новый адрес счётчика в ответе не соответствует устанавливаемому";
                    OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
                    _ActiveCommands.Remove(command);
                }
                
                //Всё в порядке выполняем изменение сетевого адреса
                UInt32 adr = 0;
                adr = ((UInt32)answerArray[0]) << 24;
                adr |= ((UInt32)answerArray[1]) << 16;
                adr |= ((UInt32)answerArray[2]) << 8;
                adr |= answerArray[3];

                Address = adr;
                command.Status = Result.OK;
                _ActiveCommands.Remove(command);
            }
            else
            {
                // Транзакция выполнена с ошибкам
                var command = _ActiveCommands.FirstOrDefault(
                    p => p.Transaction.Identifier == transaction.Identifier);
                command.Status = Result.Error;
                OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
                _ActiveCommands.Remove(command);
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
            var transaction = new Transaction(TransactionType.UnicastMode, request);
            transaction.TransactionWasEnded += _TransactionHandler;
            _ActiveCommands.Add(new DeviceCommand() { Transaction = transaction });
            return transaction;
        }
        /// <summary>
        /// Разбирает ответ от удалённого устройтва 
        /// по запросу ReadGroupAddress
        /// </summary>
        /// <param name="transaction"></param>
        private void GetReadGroupAddress(Transaction transaction)
        {
            // Разбираем ответ
            if (transaction.Status == TransactionStatus.Completed)
            {
                var command = _ActiveCommands.FirstOrDefault(
                    p => p.Transaction.Identifier == transaction.Identifier);

                if (command == null)
                {
                    throw new Exception("Не найдена команда с указанной транзакцией");
                }

                if (transaction.Answer.ToArray().Length != 11)
                {
                    command.Status = Result.Error;
                    command.ErrorDescription = "Неверная длина ответного сообщения";
                    OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
                    _ActiveCommands.Remove(command);
                }

                var request = (DataMessage)transaction.Request;
                var answer = (DataMessage)transaction.Answer;

                // Проверяем новый адрес в запросе и в ответе
                if (request.Address != answer.Address)
                {
                    command.Status = Result.Error;
                    command.ErrorDescription = "Адрес команды в ответе не соответствует адресу в запросе";
                    OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
                    _ActiveCommands.Remove(command);
                }

                if (answer.CmdCode != request.CmdCode)
                {
                    command.Status = Result.Error;
                    command.ErrorDescription = "Код команды в ответе не соответствует коду в запросе";
                    OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
                    _ActiveCommands.Remove(command);
                }

                // Присваиваем новое значение параметру
                UInt32 gAdr = 0;
                gAdr = ((UInt32)answer.Data[0]) << 24;
                gAdr |= ((UInt32)answer.Data[1]) << 16;
                gAdr |= ((UInt32)answer.Data[2]) << 8;
                gAdr |= answer.Data[3];

                var parameter = GetParameter(ParameterIndexes.GADDR);
                parameter.Value = gAdr;
                command.Status = Result.OK;
                _ActiveCommands.Remove(command);
            }
            else
            {
                // Транзакция выполнена с ошибкам
                var command = _ActiveCommands.FirstOrDefault(
                    p => p.Transaction.Identifier == transaction.Identifier);
                command.Status = Result.Error;
                OnErrorOccurred(new ErrorOccuredEventArgs() { DescriptionError = command.ToString() });
                _ActiveCommands.Remove(command);
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
            var transaction = new Transaction(TransactionType.UnicastMode, request);
            transaction.TransactionWasEnded += _TransactionHandler;
            _ActiveCommands.Add(new DeviceCommand() { Transaction = transaction });
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
            var transaction = new Transaction(TransactionType.UnicastMode, request);
            transaction.TransactionWasEnded += _TransactionHandler;
            _ActiveCommands.Add(new DeviceCommand() { Transaction = transaction });
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
            var transaction = new Transaction(TransactionType.UnicastMode, request);
            transaction.TransactionWasEnded += _TransactionHandler;
            _ActiveCommands.Add(new DeviceCommand() { Transaction = transaction });
            return transaction;
        }
        #endregion
    }
}
