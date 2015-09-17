using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubezhResurs.OSI.Messages
{
    public enum MessageType
    {
        /// <summary>
        /// Тип сообщения не определён
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// Входящие сообщение
        /// </summary>
        IncomingMessage,
        /// <summary>
        /// Исходящие сообщение
        /// </summary>
        OutcomingMessage,
        /// <summary>
        /// Служебное сообщение
        /// </summary>
        ServiceMessage
    }
}
