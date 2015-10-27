using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ResursNetwork.OSI.Messages.Transactions;
using ResursNetwork.OSI.ApplicationLayer.Devices;

namespace ResursNetwork.OSI.Messages
{
    /// <summary>
    /// Результат выполения сетевого запроса
    /// </summary>
    public class AsyncRequestResult : IAsyncRequestResult
    {
        #region Fields And Properties

        private bool _Status = false;
        private Transaction[] _Stack; 
        
        public Transaction[] Stack
        {
            get { return _Stack; }
        }

        /// <summary>
        /// Возвращает статус сетевой операции
        /// </summary>
        public bool IsCompleted
        {
            get { return _Status; }
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
            get { return _Stack.Length == 0 ? null : _Stack[Stack.Length - 1]; }
        }

        /// <summary>
        /// Устройство инициатор сетевого запроса
        /// </summary>
        public IDevice Sender 
        { 
            get { return _Stack.Length == 0 ? null : _Stack[0].Sender; } 
        }

        #endregion

        #region Constructors

        public AsyncRequestResult() { }
        
        #endregion

        #region Methods

        /// <summary>
        /// Вызывается при завершении сетевой операции
        /// </summary>
        /// <param name="stack"></param>
        public void SetCompleted(Transaction[] stack)
        {
            if (stack == null)
            {
                throw new ArgumentNullException("stack", 
                    "В стеке транзакций сетевой операции должно быть хотябы одна транзакция");
            }

            _Status = true;
            _Stack = stack;
        }

        #endregion
    }
}
