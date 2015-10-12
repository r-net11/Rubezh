using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ResursNetwork.OSI.Messages.Transactions;

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

        public bool IsCompleted
        {
            get { return _Status; }
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
        public void SetCompleted(Transaction[] stack = null)
        {
            _Status = true;
            _Stack = stack == null ? new Transaction[0] : stack;
        }

        #endregion
    }
}
