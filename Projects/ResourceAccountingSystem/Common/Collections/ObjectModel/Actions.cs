using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Collections.ObjectModel
{
    /// <summary>
    /// Действия производимые с коллекцией устройств
    /// </summary>
    public enum Action
    {
        /// <summary>
        /// Действие не определено
        /// </summary>
        Notdefined = 0,
        /// <summary>
        /// Добавление элемента в коллекцию
        /// </summary>
        Adding,
        /// <summary>
        /// Удаление элемента из коллекции
        /// </summary>
        Removing
    }
}
