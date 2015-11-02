using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ResursNetwork.OSI.ApplicationLayer;

namespace ResursNetwork.OSI.Messages.Transactions
{
    public enum NetworkRequestStatus
    {
        // Запрос создан, но не запущен в исполнение
        NotInitialized,
        // Запрос в процессе выполнения
        Running,
        // Запрос выполен успешно
        Completed,
        // Запрос не выполнен из-за ошибок
        Failed
    }

    /// <summary>
    /// Сетевой запрос. Предназначен для последовательного повторения выполения транзакции
    /// до её успешного завершения или исчерпания количества установленных попыток
    /// </summary>
    public class NetworkRequest
    {
        #region Fields And Propeties
        private Guid _id = Guid.NewGuid();
        private List<Transaction> _transactions = new List<Transaction>();
        private int _totalAttempts = 1;
        private AsyncRequestResult _asyncRequestResult;

        public Guid Id
        {
            get { return _id; }
        }

        public Transaction Request
        {
            get { return _transactions.Count == 0 ? null : _transactions[0]; }
        }

        public Transaction[] TransactionsStack
        {
            get { return _transactions.ToArray(); }
        }

        public int TotalAttempts
        {
            get { return _totalAttempts; }
            set 
            {
                if (value > 0)
                {
                    _totalAttempts = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("TotalAttempts", 
                        "Не может быть меньше или равно нулю");
                }
            }
        }

        public Transaction CurrentTransaction
        {
            get { return _transactions.Count == 0 ? null : 
                _transactions[_transactions.Count - 1]; }
        }

        public NetworkRequestStatus Status
        {
            get
            {
                if (_transactions.Count == 0)
                {
                    return NetworkRequestStatus.NotInitialized;
                }
                else if (_transactions.Count == 1)
                {
                    switch (_transactions[0].Status)
                    {
                        case TransactionStatus.NotInitialized:
                            { return NetworkRequestStatus.NotInitialized; }
                        case TransactionStatus.Running:
                            { return NetworkRequestStatus.Running; }
                        case TransactionStatus.Aborted:
                            { return NetworkRequestStatus.Failed; }
                        case TransactionStatus.Completed:
                            { return NetworkRequestStatus.Completed; }
                        default:
                            { throw new NotImplementedException(); }
                    }
                }
                else
                {
                    switch (_transactions[_transactions.Count - 1].Status)
                    {
                        case TransactionStatus.NotInitialized:
                        case TransactionStatus.Running:
                            { return NetworkRequestStatus.Running; }
                        case TransactionStatus.Aborted:
                            { return NetworkRequestStatus.Failed; }
                        case TransactionStatus.Completed:
                            { return NetworkRequestStatus.Completed; }
                        default:
                            { throw new NotImplementedException(); }
                    }
                }
            }
        }

        /// <summary>
        /// Возвращает объект слежения за выполнением операции
        /// </summary>
        public AsyncRequestResult AsyncRequestResult
        {
            get { return _asyncRequestResult; }
        }

        #endregion

        #region Constructors

        private NetworkRequest()
        {
            throw new NotImplementedException();
        }

        public NetworkRequest(Transaction transaction)
        {
			_asyncRequestResult = new AsyncRequestResult(this);
            _transactions.Insert(0, transaction);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Возвращает объект транзакции для провторного выполения запроса
        /// </summary>
        /// <returns></returns>
        public bool NextAttempt(out Transaction transaction)
        {
            if (Status != NetworkRequestStatus.Failed)
            {
                throw new InvalidOperationException(
                    "Невозможно выполнить повтор запроса, данная операция возможно только при неудачном предыдущем запросе");
            }

            if (_transactions.Count < _totalAttempts)
            {
                _transactions.Add(new Transaction(_transactions[0].Sender,
                    _transactions[0].TransactionType, _transactions[0].Request));
                transaction = _transactions[_transactions.Count - 1];
                return true;
            }
            else
            {
                // Кол-во попыток исчерпано
                transaction = null;
                return false;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Событие возникат при удачном или неудачном завершении запроса
        /// </summary>
        //public event EventHandler NetwokrRequestWasCompleted;

        #endregion
    }
}