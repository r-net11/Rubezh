using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace ResursNetwork.Management
{
    /// <summary>
    /// Интервейс для управления состоянием объектов. Позволяемт запустить, остановить
    /// или приостановить работу объекта.
    /// </summary>
    [Description("Интерфейс для реализации управления выполением задач")]
    public interface IManageable
    {
        /// <summary>
        /// Запускает работу
        /// </summary>
        [Description("Запуск на выполение")]
        void Start();
        /// <summary>
        /// Останавливает работу
        /// </summary>
        [Description("Остановить выполение")] 
        void Stop();
        /// <summary>
        /// Приостанавливает работу
        /// </summary>
        [Description("Текущее состояние")]
        Status Status
        { get; set; }
        /// <summary>
        /// Событие происходит при изменении состояния объекта
        /// </summary>
        [Description("Событие при изменении состояния упавляемого объекта")]
        event EventHandler StatusChanged;
    }
}
