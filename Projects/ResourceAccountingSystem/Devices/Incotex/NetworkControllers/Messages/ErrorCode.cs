using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.IO.Ports;

namespace RubezhResurs.Incotex.NetworkControllers.Messages
{
    /// <summary>
    /// Коды ошибок, для передачи в служебных сообщениях
    /// </summary>
    public enum ErrorCode: int
    {
        /// <summary>
        /// Источник ошибки не определён
        /// </summary>
        [Description("Неизвестная ошибка")]
        UnknownError = 0,
        /// <summary>
        /// Неправильная контрольная сумма сообщения
        /// </summary>
        IncorrectCRC = 1,
        /// <summary>
        /// Неправильная структура данных сообщения ()
        /// </summary>
        IncorrectMessageFormat = 2,
        /// <summary>
        /// Неправильная длина сообщения (коротокое или длинное)
        /// </summary>
        IncorrectMessageLength = 3,
        /// <summary>
        /// Ошибка определённая в System.IO.Ports.SerialPort SerialError.Frame
        /// Summary:
        ///     The hardware detected a framing error.
        /// </summary>
        ErrorSerialPortFrame = 4,
        /// <summary>
        /// Ошибка определённая в System.IO.Ports.SerialPort SerialError.Overrun
        /// Summary:
        ///     A character-buffer overrun has occurred. The next character is lost.
        /// </summary>
        ErrorSerialPortOverrun = 5, 
        /// <summary>
        /// Ошибка определённая в System.IO.Ports.SerialPort SerialError.RXOver
        /// Summary:
        ///     An input buffer overflow has occurred. There is either no room in the input
        ///     buffer, or a character was received after the end-of-file (EOF) character.
        /// </summary>
        ErrorSerialPortRXOver = 6, 
        /// <summary>
        /// Ошибка определённая в System.IO.Ports.SerialPort SerialError.RXParity 
        /// Summary:
        ///     The hardware detected a parity error.
        /// </summary>
        ErrorSerialPortRXParity = 7, 
        /// <summary>
        /// Ошибка определённая в System.IO.Ports.SerialPort SerialError.TXFull
        /// Summary:
        ///     The application tried to transmit a character, but the output buffer was
        ///     full.
        /// </summary>
        ErrorSerialPortTXFull =  8
    }
}
