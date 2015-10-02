using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResursNetwork.OSI.Messages
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
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Message: Id=");
            sb.Append(MessageId.ToString());
            sb.Append("; ");

            sb.Append("Type=");
            sb.Append(MessageType.ToString());
            sb.Append("; ");

            sb.Append("IsDone=");
            sb.Append(IsDone.ToString());
            sb.Append("; ");

            sb.Append("ExecutionTime=");
            sb.Append(ExecutionTime.ToLongDateString());
            sb.Append("; ");


            sb.Append("Array(Hex)=");
            
            var arr = ToArray();
            
            for (int i = 0; i < arr.Length; i++)
            {
                sb.Append(arr[i].ToString("X2"));
                
                if (i != arr.Length - 1)
                {
                    sb.Append(" ");
                }
            }
            sb.Append("; ");

            return sb.ToString();
            //return base.ToString();
        }
        #endregion
    }
}
