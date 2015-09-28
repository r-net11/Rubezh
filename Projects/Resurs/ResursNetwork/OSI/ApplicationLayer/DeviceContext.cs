using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ResursNetwork.Devices;
using ResursNetwork.OSI.Messages.Transaction;

namespace ResursNetwork.OSI.ApplicationLayer
{
    /// <summary>
    /// Контекст для сетевого устройтсва
    /// </summary>
    public class DeviceContext
    {
        #region Fields And Properties
        
        private DeviceBase _Device;
        /// <summary>
        /// Сетевое устройство  
        /// </summary>
        public DeviceBase Device
        {
            get { return _Device; }
            //set { _Device = value; }
        }
        private LinkedList<Parameter> _Parameters;
        /// <summary>
        /// Список параметров устройтсва
        /// </summary>
        private LinkedListNode<Parameter> _Cursor;
        /// <summary>
        /// Текущий параметр устройтсва
        /// </summary>
        public Parameter CurrentParameter
        {
            get { return _Cursor == null ? null : _Cursor.Value; }
        }
        /// <summary>
        /// Количество параметров
        /// </summary>
        public int Cout
        {
            get { return _Parameters.Count; }
        }

        private Transaction _CurrentTransaction;
        /// <summary>
        /// Текущая транзакция
        /// </summary>
        public Transaction CurrentTransaction
        {
            get { return _CurrentTransaction; }
            set { _CurrentTransaction = value; }
        }

        #endregion

        #region Constructors
        private DeviceContext() { throw new NotImplementedException(); }
        public DeviceContext(DeviceBase device)
        {
            if (device == null) 
            { 
                throw new ArgumentNullException("device", String.Empty); 
            }

            _Device = device;
            _Parameters = new LinkedList<Parameter>(_Device.Parameters);
            _Cursor = _Parameters.First;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Перемещает курсор на следующий параметр по порядку и возвращает его.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Возникает при попытке доступа к пустому списку параметров
        /// </exception>
        public Parameter Next()
        {
            string msg;

            if (_Parameters.Count == 0)
            {
                msg = "Невозможно переместить курсор т.к., список пуст"; 
                throw new InvalidOperationException(msg);
            }

            _Cursor = _Cursor.Next;

            if (_Cursor == null)
            {
                _Cursor = _Parameters.First; 
            }

            return _Cursor.Value;
        }
        #endregion
    }
}
