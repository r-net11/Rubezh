using System;
using System.Collections.Generic;
using System.Text;

namespace RubezhResurs.OSI.Messages.Transaction
{
    /// <summary>
    /// Состояния транзакции
    /// </summary>
    public enum TransactionStatus
    {
        /// <summary>
        /// Объкт транзакции был создан, и ожидает начала транзакции
        /// </summary>
        NotInitialized = 0,
        /// <summary>
        /// Транзакция находится в активном состоянии (в процессе выполнения)
        /// </summary>
        Running,
        /// <summary>
        /// Транзакция завершена
        /// </summary>
        Completed,
        /// <summary>
        /// Транзакция была отменена или прервана.
        /// </summary>
        Aborted
    }
}
