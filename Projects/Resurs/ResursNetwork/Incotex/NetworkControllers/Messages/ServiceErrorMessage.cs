using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RubezhResurs.OSI.Messages;

namespace RubezhResurs.Incotex.NetworkControllers.Messages
{
    public class ServiceErrorMessage: ServiceErrorMessageBase
    {
        #region Fields And Properties
        /// <summary>
        /// Возвращает значение кода ошибки, как перечисление
        /// </summary>
        public ErrorCode SpecificErrorCode
        {
            get { return (ErrorCode)_ErrorCode; }
            set { _ErrorCode = (int)value; }
        }
        #endregion
    }
}
