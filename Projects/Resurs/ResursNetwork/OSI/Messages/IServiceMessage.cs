using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResursNetwork.OSI.Messages
{
    public interface IServiceMessage: IMessage
    {
        /// <summary>
        /// Код ошибки
        /// </summary>
        int Code { get; set; }
        /// <summary>
        /// Описание ошибки
        /// </summary>
        string Description { get; set; }
    }
}
