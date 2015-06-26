using System;

namespace Defender
{
    public class LicenseParameter
    {
        public string Name { get; set; }
        
        public LicenseParameterType Type
        { 
            get
            {
                return GetLicenseParameterType(_value);
            }
        }

        object _value;
        public object Value 
        { 
            get { return _value; }
            set
            {
                if (value == null)
                    value = String.Empty;

                switch (GetLicenseParameterType(value))
                {
                    case LicenseParameterType.Boolean:
                    case LicenseParameterType.DateTime:
                        _value = value;
                        break;

                    case LicenseParameterType.Float:
                        _value = Convert.ToDouble(value);
                        break;

                    case LicenseParameterType.Integer:
                        _value = Convert.ToInt32(value);
                        break;
                    
                    case LicenseParameterType.String:
                        _value = value.ToString();
                        break;
                }
            }
        }

        public LicenseParameter()
        {

        }

        public LicenseParameter(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }

        LicenseParameterType GetLicenseParameterType(object value)
        {
            if (value.GetType() == System.Type.GetType("System.Boolean"))
                    return LicenseParameterType.Boolean;

            if (value.GetType() == System.Type.GetType("System.DateTime"))
                return LicenseParameterType.DateTime;

            if (value.GetType() == System.Type.GetType("System.Int16") ||
                value.GetType() == System.Type.GetType("System.Int32") ||
                value.GetType() == System.Type.GetType("System.UInt16") ||
                value.GetType() == System.Type.GetType("System.UInt32"))
                return LicenseParameterType.Integer;

            if (value.GetType() == System.Type.GetType("System.Single") ||
                 value.GetType() == System.Type.GetType("System.Double") ||
                 value.GetType() == System.Type.GetType("System.Decimal"))
                return LicenseParameterType.Float;

            return LicenseParameterType.String;
        }
    }
}
