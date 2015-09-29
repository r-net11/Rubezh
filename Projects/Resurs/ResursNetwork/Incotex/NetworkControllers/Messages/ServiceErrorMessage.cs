using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResursNetwork.OSI.Messages;

namespace ResursNetwork.Incotex.NetworkControllers.Messages
{
    public class ServiceErrorMessage: ServiceMessageBase
    {
        #region Fields And Properties
        /// <summary>
        /// Возвращает значение кода ошибки, как перечисление
        /// </summary>
        public ErrorCode SpecificErrorCode
        {
            get { return (ErrorCode)_Code; }
            set { _Code = (int)value; }
        }
        #endregion

        public ServiceErrorMessage()
        {
            _MessageType = MessageType.ServiceErrorMessage;
        }
    }
}
