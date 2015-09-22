using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RubezhResurs.OSI.Messages;

namespace RubezhResurs.OSI.Messages
{
    /// <summary>
    /// Реализует служебное сообещение, для обмена между
    /// объектами ApplicationLayer и DataLinkLayer  (OSI модели)
    /// </summary>
    /// <remarks>
    /// Общая стуркура сервисного сообщения 
    /// </remarks>
    public abstract class ServiceErrorMessageBase : MessageBase, IServiceErrorMessage
    {
        #region Fields And Properties

        protected int _ErrorCode;
        /// <summary>
        /// Код ошибки
        /// </summary>
        public int ErrorCode
        {
            get { return _ErrorCode; }
            set { _ErrorCode = value; }
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
        public ServiceErrorMessageBase()
        {
            _MessageType = MessageType.ServiceErrorMessage;
            _Description = String.Empty;
            _ErrorCode = 0;
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
