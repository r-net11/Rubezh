using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ResursAPI
{
	public struct ParameterStringContainer
	{
		public string Value;
		public string RegEx;

        /// <summary>
        /// Возвращает соответствует ли строка регулярному выражению
        /// </summary>
        public bool IsValid
        {
            get 
            {
                try
                {
                    return String.IsNullOrEmpty(RegEx) ? true 
                        : Regex.IsMatch(Value, RegEx);
                }
                catch
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// Проверяет на соответствие регулярному выражения
        /// и возвращает значение строки при успешной проверки,
        /// иначе возникает исключение
        /// </summary>
        /// <returns></returns>
        public string GetValue()
        {
            if (Value == null)
            {
                Value = String.Empty;
            }

            if (String.IsNullOrEmpty(RegEx))
            {
                return Value;
            }
            else
            {
                if (Regex.IsMatch(Value, RegEx))
                {
                    return Value;
                }
                else
                {
                    throw new InvalidOperationException(
                        "Невозможно вернуть значение строки которое не соответствует маске регулярного выражения");
                }
            }
        }
	}
}
