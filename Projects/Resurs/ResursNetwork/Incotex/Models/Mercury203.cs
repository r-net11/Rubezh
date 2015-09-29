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

namespace ResursNetwork.Incotex.Model
{
    /// <summary>
    /// Модель данных счётчика Меркурий M203
    /// </summary>
    public class Mercury203: DeviceBase
    {
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
        protected override void Initialization()
        {
            _Parameters.Add(new Parameter(0, "ParamName 1", "Это описание параметра",
                true, false, null, typeof(Int32), (Int32)0));
        }
        #endregion

        #region Network API

        /// <summary>
        /// Установка нового сетевого адреса счетчика 
        /// </summary>
        /// <param name="addr">Текущий сетевой адрес счётчика</param>
        /// <param name="newaddr">Новый сетевой адрес счётчика</param>
        /// <returns></returns>
        private void SetNewAddress(UInt32 addr, UInt32 newaddr)
        {
            DataMessage request = new DataMessage()
            {
                Address = addr,
                CmdCode = (Byte)Mercury203CmdCode.SetNetworkAddress
            };

            var transaction = new Transaction(TransactionType.UnicastMode, request);
            transaction.TransactionWasEnded += _TransactionHandler;
            var command = new DeviceCommand() { Transaction = transaction };
            _ActiveCommands.Add(command);
            ((IncotexNetworkController)_NetworkController).Write(command.Transaction);
        }
        /// <summary>
        /// Разбирает ответ от удалённого устройтва 
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

            switch((Mercury203CmdCode)request.CmdCode)
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
    }
}
