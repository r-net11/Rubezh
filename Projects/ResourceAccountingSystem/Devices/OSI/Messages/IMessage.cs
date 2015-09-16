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
        /// <summary>
        /// Уникальный идентификатор сообщения
        /// </summary>
        Guid MessageId { get; }
        /// <summary>
        /// Время отравки сообщения. Если сообщение не отпрвлено содержит null
        /// </summary>
        DateTime? SendingTime { get; set; }
        /// <summary>
        /// Возвращает сообщение в виде массива байт
        /// </summary>
        /// <returns></returns>
        Byte[] ToArray();
    }
}
