using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResursNetwork.OSI.Messages;
using ResursNetwork.OSI.ApplicationLayer;

namespace ResursNetwork.OSI.DataLinkLayer
{
    /// <summary>
    /// Определяет объект для передачи и приёма сообщений в сети 
    /// </summary>
    public interface IDataLinkPort
    {
        #region Fields And Properties
        /// <summary>
        /// Уникальный идентификатор порта
        /// </summary>
        Int32 PortId { get; set; }
        String PortName { get; set; }
        /// <summary>
        /// Возвращает состояние порта
        /// </summary>
        Boolean IsOpen { get; }
        /// <summary>
        /// Кол-во собщений для прочтения в приёмном буфере
        /// </summary>
        int MessagesToRead { get; }
        /// <summary>
        /// Тип физического (аппаратного интерфейса)
        /// </summary>
        InterfaceType InterfaceType { get; }
        /// <summary>
        /// Сетевой контроллер, которому принадлежит данный порт
        /// </summary>
        INetwrokController NetworkController { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Открывает порт
        /// </summary>
        void Open();
        /// <summary>
        /// Закрывает порт
        /// </summary>
        void Close();
        /// <summary>
        /// Отправляет сообщение в сеть
        /// </summary>
        /// <param name="message">Сообщение для отсылки в сеть</param>
        void Write(IMessage message);
        /// <summary>
        /// Читает одно сообщение из приёмного буфера
        /// </summary>
        /// <param name="message"></param>
        /// <returns>Прочитанное сообщение, null - если буфер пуст</returns>
        IMessage Read();
        /// <summary>
        /// Возвращает тип порта
        /// </summary>
        /// <returns></returns>
        Type GetPortType(); 
        #endregion

        #region Events
        /// <summary>
        /// Событие происходит при приёме сообщения из сети
        /// </summary>
        event EventHandler MessageReceived;
        #endregion
    }
}
