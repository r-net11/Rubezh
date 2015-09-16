using System;

namespace RubezhResurs.OSI.Messages.Transaction
{
    /// <summary>
    /// Определяет тип транзакции "запрос-ответ"
    /// </summary>
    public enum TransactionType
    {
        /// <summary>
        /// Транзакция "запрос - ответ" отсутствует
        /// </summary>
        //NoTransaction,
        /// <summary>
        /// Транзакция запроса по уникальному адресу
        /// </summary>
        UnicastMode,
        /// <summary>
        /// Транзакция широковещаетльного запроса 
        /// </summary>
        BroadcastMode,
        /// <summary>
        /// Тип транзации не определён
        /// </summary>
        Undefined
    }
}