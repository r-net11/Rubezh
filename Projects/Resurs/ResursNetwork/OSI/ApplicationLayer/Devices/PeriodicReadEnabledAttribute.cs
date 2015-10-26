using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursNetwork.OSI.ApplicationLayer.Devices
{
    /// <summary>
    /// Атрибут помечает метод как метод для периодического чтения 
    /// параметра удалённого устройства
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false, Inherited=false)]
    public class PeriodicReadEnabledAttribute : Attribute
    {
        #region Constructors
        public PeriodicReadEnabledAttribute() { }
        #endregion
    }
}
