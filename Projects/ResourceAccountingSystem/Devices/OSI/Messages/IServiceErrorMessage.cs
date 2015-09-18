using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubezhResurs.OSI.Messages
{
    public interface IServiceErrorMessage: IMessage
    {
        /// <summary>
        /// Код ошибки
        /// </summary>
        int ErrorCode { get; set; }
        /// <summary>
        /// Описание ошибки
        /// </summary>
        string Description { get; set; }
    }
}
