using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Firesec.Metadata;

namespace FiresecClient.Models
{
    public class DriverProperty
    {
        configDrvPropInfo _property;

        public DriverProperty(configDrvPropInfo property)
        {
            _property = property;
        }

        public string Name
        {
            get { return _property.name; }
        }

        public string Caption
        {
            get { return _property.caption; }
        }

        public string Default
        {
            get { return _property.@default; }
        }

        public List<DriverPropertyParameter> Parameters
        {
            get
            {
                List<DriverPropertyParameter> parameters = new List<DriverPropertyParameter>();
                foreach (var firesecParameter in _property.param)
                {
                    DriverPropertyParameter driverPropertyParameter = new DriverPropertyParameter(firesecParameter);
                    parameters.Add(driverPropertyParameter);
                }
                return parameters;
            }
        }

        public DriverPropertyTypeEnum DriverPropertyType
        {
            get
            {
                if (_property.param != null)
                {
                    return DriverPropertyTypeEnum.EnumType;
                }
                else
                {
                    switch (_property.type)
                    {
                        case "String":
                            return DriverPropertyTypeEnum.StringType;

                        case "Int":
                            return DriverPropertyTypeEnum.IntType;

                        case "Byte":
                            return DriverPropertyTypeEnum.ByteType;

                        case "Bool":
                            return DriverPropertyTypeEnum.BoolType;

                        default:
                            throw new Exception("Неизвестный тип свойства");
                    }
                }
            }
        }

        public class DriverPropertyParameter
        {
            public DriverPropertyParameter(param parameter)
            {
                _parameter = parameter;
            }

            param _parameter;

            public string Name
            {
                get { return _parameter.name; }
            }

            public string Value
            {
                get { return _parameter.value; }
            }
        }

        public enum DriverPropertyTypeEnum
        {
            EnumType,
            StringType,
            IntType,
            ByteType,
            BoolType
        }
    }
}
