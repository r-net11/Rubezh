using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubezhResurs.OSI.DataLinkLayer
{
    /// <summary>
    /// Типы физических интерфейсов 
    /// </summary>
    public enum InterfaceType : int
    {
        Unknown = 0,
        ComPort = 1,
        CAN = 3
    }
}
