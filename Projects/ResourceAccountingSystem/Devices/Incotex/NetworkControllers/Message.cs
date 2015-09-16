using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RubezhResurs.OSI.Messages;

namespace RubezhResurs.Incotex.NetworkControllers
{
    public class Message: IMessage
    {
        #region Fields And Properties

        private Guid _MessageId;
        /// <summary>
        /// GUID - сообщения
        /// </summary>
        public Guid MessageId
        {
            get { throw new NotImplementedException(); }
        }
        private UInt32 _Address;
        /// <summary>
        /// Сетевой адрес устройства
        /// </summary>
        public UInt32 Address 
        { 
            get { return _Address; } 
            set { _Address = value; } 
        }
        private Byte _CmdCode;
        /// <summary>
        /// Код комманды
        /// </summary>
        public Byte CmdCode 
        {
            get { return _CmdCode; }
            set { _CmdCode = value; }
        }
        private List<Byte> _Data;
        /// <summary>
        /// Данные
        /// </summary>
        public List<Byte> Data 
        { 
            get { return _Data; } 
        }
        /// <summary>
        /// Контрольная сумма CRC16
        /// </summary>
        public UInt16 CRC16
        {
            get { throw new NotImplementedException(); }
        }
        private DateTime? _SendingTime;
        /// <summary>
        /// Время отправки сообщения
        /// </summary>
        public DateTime? SendingTime
        {
            get { return _SendingTime; }
            set { _SendingTime = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор
        /// </summary>
        public Message()
        {
            _MessageId = Guid.NewGuid();
            _Data = new List<byte>();
        }
        #endregion

        #region Methods
        public byte[] ToArray()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
