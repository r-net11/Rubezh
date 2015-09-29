using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResursNetwork.OSI.Messages;

namespace ResursNetwork.OSI.Messages
{
    /// <summary>
    /// Реализует служебное сообещение, для обмена между
    /// объектами ApplicationLayer и DataLinkLayer  (OSI модели)
    /// </summary>
    /// <remarks>
    /// Общая стуркура сервисного сообщения 
    /// </remarks>
    public abstract class ServiceMessageBase : MessageBase, IServiceMessage
    {
        #region Fields And Properties

        protected int _Code;
        /// <summary>
        /// Код ошибки
        /// </summary>
        public int Code
        {
            get { return _Code; }
            set { _Code = value; }
        }
        private string _Description;
        /// <summary>
        /// Описание ошибки
        /// </summary>
        public string Description
        {
            get { return _Description; }
            set { _Description = value == null ? String.Empty : value ; }
        }
        #endregion

        #region Constructors
        public ServiceMessageBase()
        {
            _MessageType = MessageType.Undefined;
            _Description = String.Empty;
            _Code = 0;
        }
        #endregion

        #region Methods
        public override byte[] ToArray()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
