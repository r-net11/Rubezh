using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResursNetwork.Networks
{
    /// <summary>
    /// Объет упрвления сетями системы учёта русурсов
    /// </summary>
    public class NetworkManager
    {
        #region Fields And Properties

        private static NetworkManager _Instance;
        /// <summary>
        /// Возвращает менеджер сетей
        /// </summary>
        public static NetworkManager Instance
        {
            get 
            {
                if (_Instance == null)
                {
                    lock(_SyncRoot)
                    {
                        if (_Instance == null)
                        {
                            _Instance = new NetworkManager();
                        }
                    }
                }
                return _Instance;
            }
        }
        private static Object _SyncRoot = new Object();
        #endregion

        #region Constructors
        #endregion

        #region Methods
        #endregion
    }
}
