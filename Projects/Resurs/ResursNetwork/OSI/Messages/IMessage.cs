using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubezhResurs.OSI.Messages
{
    /// <summary>
    /// Сетевое сообщение 
    /// </summary>
    public interface IMessage
    {
        #region Fields And Properties
        /// <summary>
        /// Уникальный идентификатор сообщения
        /// </summary>
        Guid MessageId { get; }
        /// <summary>
        /// Время отравки или приёма сообщения.
        /// </summary>
        DateTime ExecutionTime { get; set; }
        /// <summary>
        /// Если true - сообщение было обработано и 
        /// свойство ExecutionTime действительно
        /// </summary>
        Boolean IsDone { get; }
        /// <summary>
        /// Тип сообщение
        /// </summary>
        MessageType MessageType { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Возвращает сообщение в виде массива байт
        /// </summary>
        /// <returns></returns>
        Byte[] ToArray();
        #endregion
    }
}
