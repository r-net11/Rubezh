using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResursNetwork.OSI.ApplicationLayer;
using ResursNetwork.OSI.Messages;

namespace ResursNetwork.OSI.DataLinkLayer
{
    /// <summary>
    /// Базовый класс для создания портов физических интерфейсов 
    /// </summary>
    public abstract class DataLinkPortBase : IDataLinkPort
    {
        #region Fields And Properties
        
        protected Guid _Id;
		/// <summary>
		/// Буфер входящих сообщений
		/// </summary>
		protected Queue<IMessage> _InputBuffer = new Queue<IMessage>();
		protected INetwrokController _NetworkController;
        
		public Guid Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        public abstract string PortName { get; set; }
        
		public abstract bool IsOpen { get; }
        
        public int MessagesToRead
        {
            get { return _InputBuffer.Count; }
        }

		public abstract InterfaceType InterfaceType { get; }
        
        public INetwrokController NetworkController
        {
            get { return _NetworkController; }
        }

        #endregion

        #region Constructors

        protected DataLinkPortBase()
        {
            _Id = Guid.NewGuid();
        }

        #endregion

        #region Methods
        /// <summary>
        /// Создаёт объект порта физического подключения на основе
        /// стороки сериализованного представления порта
        /// </summary>
        /// <param name="serialized">
        /// Строка сериализованного представления порта</param>
        /// <returns>Порт</returns>
        public static IDataLinkPort Create(string serialized)
        {
            throw new NotImplementedException();
        }
        protected void SetNetworkController(INetwrokController controller)
        {
            _NetworkController = controller;
        }
        public abstract void Open();
        public abstract void Close();
        public abstract void Write(Messages.IMessage message);
        public IMessage Read()
        {
            if (_InputBuffer.Count > 0)
            {
                return _InputBuffer.Dequeue();
            }
            else
            {
                return null;
            }
        }
        public override string ToString()
        {
            return String.Format(
                "Type={0}; PortName={1}; ControllerId={2};",
                this.GetType().ToString(), PortName, 
                _NetworkController == null ? String.Empty : _NetworkController.Id.ToString());
            //return base.ToString();
        }
        /// <summary>
        /// Генерирует событие приёма сообщения из сети
        /// </summary>
        protected virtual void OnMessageReceived()
        {
            if (MessageReceived != null)
            {
                MessageReceived(this, new EventArgs());
            }
        }

        #endregion

        #region Event
        public event EventHandler MessageReceived;
        #endregion
    }
}
