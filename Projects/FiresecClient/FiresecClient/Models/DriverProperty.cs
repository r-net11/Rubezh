using System;
using System.Collections.Generic;
using Firesec.Metadata;
using System.Runtime.Serialization;

namespace FiresecClient.Models
{
    [DataContract]
    public class DriverProperty
    {
        public DriverProperty(configDrvPropInfo internalProperty)
        {
            Name = internalProperty.name;
            Caption = internalProperty.caption;
            Default = internalProperty.@default;
            Visible = ((internalProperty.hidden == "0") && (internalProperty.showOnlyInState == "0"));
            IsHidden = (internalProperty.hidden == "1");

            Parameters = new List<DriverPropertyParameter>();
            if (internalProperty.param != null)
            {
                foreach (var firesecParameter in internalProperty.param)
                {
                    DriverPropertyParameter driverPropertyParameter = new DriverPropertyParameter(firesecParameter);
                    Parameters.Add(driverPropertyParameter);
                }
            }

            if (internalProperty.param != null)
            {
                DriverPropertyType = DriverPropertyTypeEnum.EnumType;
            }
            else
            {
                switch (internalProperty.type)
                {
                    case "String":
                        DriverPropertyType = DriverPropertyTypeEnum.StringType;
                        break;

                    case "Int":
                    case "Double":
                        DriverPropertyType = DriverPropertyTypeEnum.IntType;
                        break;

                    case "Byte":
                        DriverPropertyType = DriverPropertyTypeEnum.ByteType;
                        break;

                    case "Bool":
                        DriverPropertyType = DriverPropertyTypeEnum.BoolType;
                        break;

                    default:
                        throw new Exception("Неизвестный тип свойства");
                }
            }
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Caption { get; set; }

        [DataMember]
        public string Default { get; set; }

        [DataMember]
        public bool Visible { get; set; }

        [DataMember]
        public bool IsHidden { get; set; }

        [DataMember]
        public List<DriverPropertyParameter> Parameters { get; set; }

        [DataMember]
        public DriverPropertyTypeEnum DriverPropertyType { get; set; }
    }

    [DataContract]
    public class DriverPropertyParameter
    {
        public DriverPropertyParameter(param parameter)
        {
            Name = parameter.name;
            Value = parameter.value;
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Value { get; set; }
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
