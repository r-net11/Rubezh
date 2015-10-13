using System;
using System.Diagnostics;
using System.Text;
using ResursNetwork.OSI.ApplicationLayer.Devices;

namespace ResursNetwork.OSI.Messages.Transactions
{
    /// <summary>
    /// Класс для хранения данных транзакции "Запрос-ответ"
    /// </summary>
    public class Transaction
    {
        #region Fields And Properties
        
        /// <summary>
        /// Тип modbus-транзацкии "запрос-ответ"
        /// </summary>
        private TransactionType _Type;

        /// <summary>
        /// Устройтво отправитель транзакции
        /// </summary>
        private IDevice _Sender;
        
        /// <summary>
        /// Состояние транзакции
        /// </summary>
        private TransactionStatus _Status;
        
        /// <summary>
        /// Время начала транзакции, мсек
        /// </summary>
        /// <remarks>Количество мсек от старта системы</remarks>
        private DateTime _StartTime;
        
        /// <summary>
        /// Время окончания транзакции, мсек
        /// </summary>
        /// <remarks>Количество мсек от старта системы</remarks>
        private DateTime _EndTime;
        
        /// <summary>
        /// Запрос от мастера сети
        /// </summary>
        private IMessage _Request;
        
        /// <summary>
        /// Ответ от ведомого устройства
        /// </summary>
        private IMessage _Answer;

        private TransactionError _Error = new TransactionError 
            { 
                Description = String.Empty, 
                ErrorCode = TransactionErrorCodes.NoError 
            };

        /// <summary>
        /// Возвращает тип текущей транзакции
        /// </summary>
        public TransactionType TransactionType
        {
            get { return this._Type; }
        }

        /// <summary>
        /// Устройтво отправитель транзакции
        /// </summary>
        public IDevice Sender
        {
            get { return _Sender; }
            set { _Sender = value; }
        }

        /// <summary>
        /// Состояние транзакции
        /// </summary>
        public TransactionStatus Status
        {
            get { return _Status; }
        }

        /// <summary>
        /// Возвращает состояние транзакции
        /// </summary>
        public Boolean IsRunning
        {
            get
            {
                Boolean result;
                switch (this._Status)
                {
                    case TransactionStatus.Aborted:
                        {
                            result = false; break;
                        }
                    case TransactionStatus.Completed:
                        {
                            result = false; break;
                        }
                    case TransactionStatus.NotInitialized:
                        {
                            result = false; break;
                        }
                    case TransactionStatus.Running:
                        {
                            result = true; break;
                        }
                    default:
                        {
                            throw new NotImplementedException(
                                "Данное состояние транзакции не поддерживается в данной версии ПО");
                        }
                }
                return result;
            }
        }
                
        /// <summary>
        /// Возвращает время начала транзакции, мсек
        /// </summary>
        /// <remarks>Количество мсек от старта системы</remarks>
        public DateTime StartTime
        {
            get { return _StartTime; }
        }
                
        /// <summary>
        /// Возвращает время окончания транзакции, мсек
        /// </summary>
        /// <remarks>Количество мсек от старта системы</remarks>
        public DateTime EndTime
        {
            get { return _EndTime; }
        }
        
        /// <summary>
        /// Длительность транзакции, мсек
        /// </summary>
        public TimeSpan TotalTime
        {
            get 
            {
                if (IsRunning)
                {
                    return DateTime.Now - _StartTime;
                }
                else
                {
                    return _EndTime - _StartTime;
                }
            }
        }
                
        /// <summary>
        /// Запрос от мастера сети
        /// </summary>
        public IMessage Request
        {
            get { return _Request; }
        }
                
        /// <summary>
        /// Ответ от ведомого устройства
        /// </summary>
        public IMessage Answer
        {
            get { return _Answer; }
            set { _Answer = value; }
        }
        
        /// <summary>
        /// Идентификатор транзакции
        /// </summary>
        public Guid Identifier
        {
            get
            {
                return _Request == null ? Guid.Empty : _Request.MessageId;
            }
        }
                
        /// <summary>
        /// Возвращает описание причины звершения транзакции при вызове метода Abort()
        /// </summary>
        public TransactionError Error
        {
            get { return _Error; }
        }        
        

        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор
        /// </summary>
        public Transaction() 
        {
            _Answer = null;
            _Sender = null;
            _Request = null;
            _Status = TransactionStatus.NotInitialized;
            _EndTime = DateTime.Now;
            _StartTime = DateTime.Now;
            _Type = TransactionType.Undefined;
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="sender">Отпрвитель запроса</param>
        /// <param name="type">Тип modbus транзации</param>
        /// <param name="request">Запрос от мастера сети</param>
        public Transaction(IDevice sender, TransactionType type, 
            IMessage request)
        {            
            _Type = type;
            _Sender = sender;
            _Answer = null;
            _Request = request;
            _Status = TransactionStatus.NotInitialized;
            _EndTime = new DateTime();
            _StartTime = new DateTime();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Начинает новую транзакцию
        /// </summary>
        public void Start()
        {
            if (this.IsRunning)
            {
                throw new InvalidOperationException(String.Format(
                    "Transaction ID: {0} - Попытка запустить уже запущенную транзакцию",
                    Identifier));
            }
            else
            {
                _Status = TransactionStatus.Running;
                _StartTime = DateTime.Now;
                OnTransactionWasStarted();

                //Debug.WriteLine(String.Format(
                //    "Transaction ID: {0} - Начало транзакции: {1} мсек",
                //    this.Identifier.ToString(), this._TimeOfStart));
            }
            return;
        }
        /// <summary>
        /// Заканчивает текущую транзакцию
        /// </summary>
        /// <param name="answer">Ответ slave-устройства</param>
        public void Stop(IMessage answer)
        {
            if (!this.IsRunning)
            {
                throw new InvalidOperationException(
                    String.Format("Transaction ID: {0}; Попытка завершить не начатую транзакцию", 
                    this.Identifier));
            }
            else
            {

                switch (TransactionType)
                {
                    case TransactionType.UnicastMode:
                        {
                            if (answer != null)
                            {
                                this._Answer = answer;
                            }
                            else
                            {
                                throw new NullReferenceException(
                                    "Попытка установить в null ответное сообщение для завершения " +
                                    "транзакции адресованного запроса");
                            }
                            break;
                        }
                    case TransactionType.BroadcastMode:
                        {
                            if (answer != null)
                            {
                                throw new InvalidOperationException(
                                    "Попытка установить ответное сообщение для завершения транзакции " +
                                    "широковещательного запроса");
                            }
                            break;
                        }
                    case TransactionType.Undefined:
                        {
                            this._Answer = answer;
                            break;
                        }
                }

                _EndTime = DateTime.Now;
                _Status = TransactionStatus.Completed;
                
                // Генерируем событие окончания транзакции.
                OnTransactionWasEnded();

                //Debug.WriteLine(String.Format(
                //    "Transaction ID: {0} - Конец транзакции: {1}; Время транзакции: {2}",
                //    this.Identifier, this._TimeOfEnd, this.TimeOfTransaction));
            }
            return;
        }
        /// <summary>
        /// Прерывает текущую транзакцию
        /// </summary>
        /// <param name="error">Описывает ситуацию отмены текущей транзакции</param>
        public void Abort(TransactionError error)
        {
            if (!IsRunning)
            {
                throw new InvalidOperationException(
                    String.Format("Transaction ID: {0}; Попытка отменить не начатую транзакцию",
                    this.Identifier));
            }
            else
            {
                _EndTime = DateTime.Now;
                _Error = error;
                _Status = TransactionStatus.Aborted;
                // Генерируем событие
                OnTransactionWasEnded();
            }
            return;
        }
        
        /// <summary>
        /// Метод гененрирует событие завершения транзакции
        /// </summary>
        private void OnTransactionWasEnded()
        {
            EventHandler handler = this.TransactionWasEnded;
            EventArgs args = new EventArgs();

            if (handler != null)
            {
                foreach (EventHandler singleCast in handler.GetInvocationList())
                {
                    System.ComponentModel.ISynchronizeInvoke syncInvoke =
                        singleCast.Target as System.ComponentModel.ISynchronizeInvoke;
                    if (syncInvoke != null)
                    {
                        if (syncInvoke.InvokeRequired)
                        {
                            syncInvoke.Invoke(singleCast, new Object[] { this, args });
                        }
                        else
                        {
                            singleCast(this, args);
                        }
                    }
                    else
                    {
                        singleCast(this, args);
                    }
                }
            }
            return;
        }
        
        /// <summary>
        /// Генерирует событие запуска транзакции
        /// </summary>
        public void OnTransactionWasStarted()
        {
            if (TransactionWasStarted != null)
            {
                TransactionWasStarted(this, new EventArgs());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format(
                "Transaction: Id={0}; Type={1}; Status={2}; Start={3}; Stop={4}; Error={5}; Request={6}; Answer={7}", 
                Identifier, TransactionType, Status, StartTime, EndTime, Error, 
                Request == null ? String.Empty : Request.ToString(), 
                Answer == null ? String.Empty : Answer.ToString());
            //return base.ToString();
        }
        
        #endregion

        #region Events

        /// <summary>
        /// Событие возникает после запуска транзакции
        /// </summary>
        public event EventHandler TransactionWasStarted;
        
        /// <summary>
        /// Событие возникает после завершения транзакции;
        /// </summary>
        public event EventHandler TransactionWasEnded;
        
        #endregion
    }
}
