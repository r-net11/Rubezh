using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RubezhResurs.OSI.Messages;
using RubezhResurs.Modbus;

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
            get { return _MessageId; }
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
        public CRC16 CRC16
        {
            get 
            {
                List<Byte> list = new List<byte>();
                // Адрес устройства
                list.Add((Byte)(Address >> 24));
                list.Add((Byte)(Address >> 16));
                list.Add((Byte)(Address >> 8));
                list.Add((Byte)(Address));
                // Код команды
                list.Add((byte)(CmdCode));
                // Данные
                list.AddRange(Data);
                return CRC16.GetCRC16(list.ToArray());
            }
        }
        
        private DateTime? _ExecutionTime;
        /// <summary>
        /// Время отправки сообщения
        /// </summary>
        public DateTime ExecutionTime
        {
            get { return _ExecutionTime.HasValue ? _ExecutionTime.Value : new DateTime(); }
            set { _ExecutionTime = value; }
        }
        public bool IsDone
        {
            get { return _ExecutionTime.HasValue ? true : false; }
        }
        private MessageType _MessageType;
        public MessageType MessageType
        {
            get { return _MessageType; }
            set { MessageType = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор
        /// </summary>
        public Message()
        {
            _MessageType = MessageType.Undefined;
            _MessageId = Guid.NewGuid();
            _ExecutionTime = null;
            _Data = new List<byte>();
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="data"></param>
        public Message(Byte[] data)
        {
            _MessageType = MessageType.Undefined;
            _MessageId = Guid.NewGuid();
            _ExecutionTime = null;
            _Data = new List<byte>(data);
        }
        #endregion

        #region Methods
        public byte[] ToArray()
        {
            List<Byte> list = new List<byte>();
            // Адрес устройства
            list.Add((Byte)(Address >> 24));
            list.Add((Byte)(Address >> 16));
            list.Add((Byte)(Address >> 8));
            list.Add((Byte)(Address));
            // Код команды
            list.Add((byte)(CmdCode));
            // Данные
            list.AddRange(Data);
            // Контрольная сумма
            list.AddRange(CRC16.ToArray());

            return list.ToArray();
        }
        #endregion
    }
}
