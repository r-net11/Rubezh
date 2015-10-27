using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ResursNetwork.OSI.Messages.Transactions;
using ResursAPI.Models;

namespace ResursNetwork.OSI.Messages
{
    /// <summary>
    /// Результат выполения сетевого запроса
    /// </summary>
    public interface IAsyncRequestResult
    {
        #region Fields And Properties

        /// <summary>
        /// Стек транзакций сетевых запросов при 
        /// выполнении сетевой операции
        /// </summary>
        Transaction[] Stack { get; }

        /// <summary>
        /// Состояние сетевой операции
        /// </summary>
        bool IsCompleted { get; }

		/// <summary>
		/// Успешность выполения операции
		/// </summary>
		bool HasError { get; }

		/// <summary>
		/// Описание ошибки в последней выполенной
		/// операции
		/// </summary>
		TransactionError Error { get; }
        
		#endregion
    }
}
