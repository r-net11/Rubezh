using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    /// до её успешного завершения или исчерпания кол-ва попыток
    /// </summary>
    public class NetworkRequest
    {
        #region Fields And Propeties
        private Guid _Id;
        private List<Transaction> _Transactions = new List<Transaction>();
        private int _TotalAttempts = 1;

        public Guid Id
        {
            get { return Id; }
        }

        public Transaction Request
        {
            get { return _Transactions.Count == 0 ? null : _Transactions[0]; }
        }

        public Transaction[] TransactionsStack
        {
            get { return _Transactions.ToArray(); }
        }

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
                    throw new ArgumentOutOfRangeException("TotalAttempts", 
                        "Не может быть меньше или равно нулю");
                }
            }
        }

        public Transaction CurrentTransaction
        {
            get { return _Transactions.Count == 0 ? null : 
                _Transactions[_Transactions.Count - 1]; }
        }

        public NetworkRequestStatus Status
        {
            get
            {
                if (_Transactions.Count == 0)
                {
                    return NetworkRequestStatus.NotInitialized;
                }
                else if (_Transactions.Count == 1)
                {
                    switch (_Transactions[0].Status)
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
                    switch (_Transactions[_Transactions.Count - 1].Status)
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

        #endregion

        #region Constructors

        private NetworkRequest()
        {
            throw new NotImplementedException();
        }

        public NetworkRequest(Transaction transaction)
        {
            _Transactions.Insert(0, transaction);
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

            if (_Transactions.Count < _TotalAttempts)
            {
                _Transactions.Add(new Transaction(_Transactions[0].Sender,
                    _Transactions[0].TransactionType, _Transactions[0].Request));
                transaction = _Transactions[_Transactions.Count - 1];
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