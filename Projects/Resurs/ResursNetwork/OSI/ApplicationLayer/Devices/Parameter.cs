using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResursNetwork.Devices.ValueConverters;

namespace ResursNetwork.Devices
{
    /// <summary>
    /// Содержит описание параметра профиля устройства
    /// </summary>
    public sealed class Parameter
    {
        #region Fields And Properties
        
        private String _Name;
        private String _Description;
        private IValueConverter _ValueConverter;
        private Boolean _ReadOnly;
        private Boolean _PollingEnabled;
        private ValueType _Value;

        /// <summary>
        /// Наименование параметра
        /// </summary>
        public String Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        /// <summary>
        /// Описание параметра
        /// </summary>
        public String Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        /// <summary>
        /// Конвертор значения параметра
        /// </summary>
        public IValueConverter ValueConverter
        {
            get { return _ValueConverter; }
            set { _ValueConverter = value; }
        }

        /// <summary>
        /// Параметр может только читаться из устройства.
        /// </summary>
        public Boolean ReadOnly
        {
            get { return _ReadOnly; }
            set { _ReadOnly = value; }
        }

        /// <summary>
        /// Разрешает/запрещает циклический опрос данного параметра
        /// из удалённого устройства
        /// </summary>
        public Boolean PollingEnabled
        {
            get { return _PollingEnabled; }
            set { _PollingEnabled = value; }
        }

        /// <summary>
        /// Тип данных значения параметра
        /// </summary>
        public readonly Type ValueType;

        public ValueType Value
        {
            get { return _Value; }
            set 
            {
                if (value.GetType() == this.ValueType)
                {
                    _Value = value;
                }
                else
                {
                    throw new InvalidCastException("Попытка установить значение недопустимого типа");
                }
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор
        /// </summary>
        private Parameter()
        {
            throw new NotSupportedException();
        }
        /// <summary>
        /// Тип значения параметра
        /// </summary>
        /// <param name="type"></param>
        public Parameter(Type type)
        {
            if (type.BaseType != typeof(ValueType))
            {
                throw new ArgumentException("Тип данных параметра должен быть значимым");
            }
            ValueType = type;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="pollingEnabled"></param>
        /// <param name="readOnly"></param>
        /// <param name="valueConverter"></param>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public Parameter(Int32 index, string name, string description, 
            bool pollingEnabled, bool readOnly, IValueConverter valueConverter, 
            Type type, ValueType value)
        {
            //_Index = index;
            _Name = name == null ? String.Empty : name;
            _Description = description == null ? String.Empty : description;
            _PollingEnabled = pollingEnabled;
            _ReadOnly = readOnly;
            _ValueConverter = valueConverter;

            if (type.BaseType != typeof(ValueType))
            {
                throw new ArgumentException("Тип данных параметра должен быть значимым");
            }
            ValueType = type;

            Value = value;
        }
        #endregion
    }
}
