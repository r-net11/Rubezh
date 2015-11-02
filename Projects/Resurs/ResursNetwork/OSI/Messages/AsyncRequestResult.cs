using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ResursNetwork.OSI.Messages.Transactions;
using ResursNetwork.OSI.ApplicationLayer.Devices;
using ResursAPI.Models;

namespace ResursNetwork.OSI.Messages
{
    /// <summary>
    /// Результат выполения сетевого запроса
    /// </summary>
    public class AsyncRequestResult : IAsyncRequestResult
    {
        #region Fields And Properties

        bool _status = false;
        Transaction[] _stack;
		NetworkRequest _NetworkRequest;
        
        public Transaction[] Stack
        {
            get { return _stack; }
        }

		public NetworkRequest NetworkCommand
		{
			get { return _NetworkRequest; }
		}

        /// <summary>
        /// Возвращает статус сетевой операции
        /// </summary>
        public bool IsCompleted
        {
            get { return _status; }
        }

        /// <summary>
        /// Возвращает успешно или не успешно проведена
        /// сетевая операция
        /// </summary>
        public bool HasError
        {
            get 
            {
                if (!IsCompleted)
                {
                    return false;
                }
                else
                {
                    // Если операция завершена
                    switch(LastTransaction.Status)
                    {
                        case TransactionStatus.Aborted: { return true; }
                        case TransactionStatus.Completed:
                        case TransactionStatus.NotInitialized:
                        case TransactionStatus.Running: { return false; }
                        default: { throw new NotSupportedException(); }
                    }
                }
            }
        }

		/// <summary>
		/// Возвращает описание ошибки последней выолненной транзакции
		/// (если сетевая операция ещё активна, то всегда возвращается успешный результат)
		/// </summary>
		public TransactionError Error
		{
			get
			{
				if (!IsCompleted)
				{
					return new TransactionError
					{
						ErrorCode = TransactionErrorCodes.NoError,
						Description = String.Empty
					};
				}
				else
				{
 					return LastTransaction.Error;
				}
			}
		}

        /// <summary>
        /// Возвращает последнею выполненную сетевую транзакцию
        /// </summary>
        public Transaction LastTransaction
        {
            get { return _stack.Length == 0 ? null : _stack[Stack.Length - 1]; }
        }

        /// <summary>
        /// Устройство инициатор сетевого запроса
        /// </summary>
        public IDevice Sender 
        { 
            get { return _stack.Length == 0 ? null : _stack[0].Sender; } 
        }

        #endregion

        #region Constructors

		private AsyncRequestResult() { throw new NotImplementedException(); }

		public AsyncRequestResult(NetworkRequest networkCommand)
		{
			if (networkCommand == null)
			{
				throw new ArgumentNullException("networkCommand", String.Empty);
			}

			_NetworkRequest = networkCommand;
		}

        #endregion

        #region Methods

        /// <summary>
        /// Вызывается при завершении сетевой операции
        /// </summary>
        /// <param name="stack"></param>
        public void SetCompleted()
        {
            _status = true;
            _stack = _NetworkRequest.TransactionsStack;
        }

        #endregion
    }
}
