using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubezhResurs.OSI.Messages
{
    public abstract class MessageBase: IMessage
    {
        #region Fields And Properties

        protected Guid _MessageId;
        /// <summary>
        /// Id сообщения
        /// </summary>
        public Guid MessageId
        {
            get { return _MessageId; }
        }
        protected DateTime? _ExecutionTime;
        /// <summary>
        /// Время обработки сообщения
        /// </summary>
        public DateTime ExecutionTime
        {
            get { return _ExecutionTime.HasValue ? _ExecutionTime.Value : new DateTime(); }
            set { _ExecutionTime = value; }
        }
        /// <summary>
        /// Сообщение было обработано (отправлено или принято)
        /// </summary>
        public bool IsDone
        {
            get { return _ExecutionTime.HasValue ? true : false; }
        }

        protected MessageType _MessageType;
        /// <summary>
        /// Тип сообщения
        /// </summary>
        public MessageType MessageType
        {
            get { return _MessageType; }
            set { _MessageType = value; }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Возвращает сообщение в виде массива байт
        /// </summary>
        /// <returns></returns>
        public abstract byte[] ToArray();
        #endregion
    }
}
