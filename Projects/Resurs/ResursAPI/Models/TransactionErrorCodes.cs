using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI.Models
{
    public enum TransactionErrorCodes
    {
        NoError = 0,
        /// <summary>
        /// Нет ответа на запрос за заданное время
        /// </summary>
        RequestTimeout,
        /// <summary>
        /// Порт для передачи данных закрыт
        /// </summary>
        DataLinkPortIsClosed,
        /// <summary>
        /// Отсутствует порт для передачи данных
        /// </summary>
        DataLinkPortNotInstalled,
        /// <summary>
        /// Выполение запросы было отменено или прервано
        /// </summary>
        RequestWasCancelled,
        /// <summary>
        /// Попытка выполнить сетевую транзакцию с недопустимым типом
        /// </summary>
        TransactionTypeIsWrong
    }
}
