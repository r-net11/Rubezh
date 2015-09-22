using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RubezhResurs.Devices;

namespace RubezhResurs.Incotex.Model
{
    /// <summary>
    /// Модель данных счётчика Меркурий M203
    /// </summary>
    public class Mercury203: Device
    {
        #region Fields And Properties
        public override DeviceType DeviceType
        {
            get { return Devices.DeviceType.Mercury203; }
        }
        #endregion

        #region Constructors
        public Mercury203(): base()
        {
        }
        #endregion

        #region Methods
        protected override void Initialization()
        {
            _Parameters.Add(new Parameter(0, "ParamName 1", "Это описание параметра",
                true, false, null, typeof(Int32), (Int32)0));
        }
        #endregion
    }
}
