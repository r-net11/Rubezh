using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSIModel.Messages;

namespace OSIModel.DataLinkLayer
{
    /// <summary>
    /// Определяет объект для передачи и приёма сообщений в сети 
    /// </summary>
    public interface IDataLinkPort
    {
        #region Fields And Properties
        /// <summary>
        /// Возвращает состояние порта
        /// </summary>
        Boolean IsOpen { get; }
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
        #endregion
    }
}
