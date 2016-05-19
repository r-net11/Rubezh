using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Reflection;

namespace RubezhService.Views.TypeConverters
{
    /// <summary>
    /// Конвертер для преобразования перечисления. Преобразование члена
    /// перечисления в стоку на остнове его атрибута [Description("...")]
    /// Класс создан на основе статьи:
    /// http://www.codeproject.com/Articles/6294/Description-Enum-TypeConverter
    /// </summary>
    public class EnumTypeConverter: EnumConverter
    {
        #region Fields And Properties

        protected System.Type myVal;
        
        #endregion
        
        #region Constructors

        public EnumTypeConverter(System.Type type)
            : base(type)
        {
            myVal = type;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Gets Enum Value's Description Attribute
        /// </summary>
        /// <param name="value">The value you want the description attribute for</param>
        /// <returns>The description, if any, else it's .ToString()</returns>
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            
            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
        }
        /// <summary>
        /// Gets the value of an Enum, based on it's Description Attribute or named value
        /// </summary>
        /// <param name="value">The Enum type</param>
        /// <param name="description">The description or name of the element</param>
        /// <returns>The value, or the passed in description, if it was not found</returns>
        public static object GetEnumValue(System.Type value, string description)
        {
            FieldInfo [] fis = value.GetFields();
            foreach(FieldInfo fi in fis) 
            {
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if(attributes.Length>0) 
                {
                    if(attributes[0].Description == description)
                    {
                        return fi.GetValue(fi.Name);
                    }
                }
                if(fi.Name == description)
                {
                    return fi.GetValue(fi.Name);
                }
            }
            return description;
        }

        public override object ConvertTo(ITypeDescriptorContext context,
            System.Globalization.CultureInfo culture,
            object value, Type destinationType)
        {
            if (value is Enum && destinationType == typeof(string))
            {
                return EnumTypeConverter.GetEnumDescription((Enum)value);
            }
            if (value is string && destinationType == typeof(string))
            {
                return EnumTypeConverter.GetEnumValue(myVal, (String)value);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, 
            System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
            {
                return EnumTypeConverter.GetEnumValue(myVal, (String)value);
            }
            if (value is Enum)
            {
                return EnumTypeConverter.GetEnumDescription((Enum)value);     
            }
            return base.ConvertFrom(context, culture, value);
        }    

        #endregion
    }
}