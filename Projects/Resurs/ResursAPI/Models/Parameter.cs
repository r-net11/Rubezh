using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ResursAPI
{
	public class Parameter
	{
		public Parameter()
		{
			UID = Guid.NewGuid();
		}
		public void Initialize(DriverParameter driverParameter)
		{
			DriverParameter = driverParameter;
			Number = driverParameter.Number;
			switch (driverParameter.ParameterType)
			{
				case ParameterType.Enum:
					if (IntValue == null)
						IntValue = driverParameter.EnumDefaultItem;
					break;
				case ParameterType.String:
					if (StringValue == null)
						StringValue = driverParameter.StringDefaultValue;
					break;
				case ParameterType.Int:
					if (IntValue == null)
						IntValue = driverParameter.IntDefaultValue;
					break;
				case ParameterType.Double:
					if (DoubleValue == null)
						DoubleValue = driverParameter.DoubleDefaultValue;
					break;
				case ParameterType.Bool:
					BoolValue = driverParameter.BoolDefaultValue;
					break;
				case ParameterType.DateTime:
					if (DateTimeValue == null)
						DateTimeValue = driverParameter.DateTimeDefaultValue;
					break;
				default:
					break;
			}
		}
		public string Validate()
		{
			if(DriverParameter == null)
				return "Отсутствует тип параметра";
			switch (DriverParameter.ParameterType)
			{
				case ParameterType.Enum:
					if (IntValue == null)
						return "Значение параметра не задано";
					if(!DriverParameter.ParameterEnumItems.Select(x => x.Value).Any(x => x == IntValue.Value))
						return "Недопустимое значение параметра";
					break;
				case ParameterType.String:
					if (StringValue != null && DriverParameter.RegEx != null && !Regex.IsMatch(StringValue, DriverParameter.RegEx))
						return "Недопустимое значение параметра";
					break;
				case ParameterType.Int:
					if (IntValue == null)
						return "Значение параметра не задано";
					if (DriverParameter.IntMinValue != null && IntValue < DriverParameter.IntMinValue.Value)
						return "Значение параметра не должно быть меньше чем " + DriverParameter.IntMinValue.Value;
					if (DriverParameter.IntMaxValue != null && IntValue > DriverParameter.IntMaxValue.Value)
						return "Значение параметра не должно быть больше чем " + DriverParameter.IntMaxValue.Value;
					break;
				case ParameterType.Double:
					if (DoubleValue == null)
						return "Значение параметра не задано";
					if (DriverParameter.DoubleMinValue != null && DoubleValue < DriverParameter.DoubleMinValue.Value)
						return "Значение параметра не должно быть меньше чем " + DriverParameter.DoubleMinValue.Value;
					if (DriverParameter.DoubleMaxValue != null && DoubleValue > DriverParameter.DoubleMaxValue.Value)
						return "Значение параметра не должно быть больше чем " + DriverParameter.DoubleMaxValue.Value;
					break;
				case ParameterType.Bool:
					break;
				case ParameterType.DateTime:
					if (DateTimeValue == null)
						return "Значение параметра не задано";
					if (DriverParameter.DateTimeMinValue != null && DateTimeValue < DriverParameter.DateTimeMinValue.Value)
						return "Значение параметра не должно быть меньше чем " + DriverParameter.DateTimeMinValue.Value;
					if (DriverParameter.DateTimeMaxValue != null && DateTimeValue > DriverParameter.DateTimeMaxValue.Value)
						return "Значение параметра не должно быть больше чем " + DriverParameter.DateTimeMaxValue.Value;
					break;
				default:
					return "Отсутствует тип параметра";
			}
			return null;
		}
		[Key]
		public Guid UID { get; set; }
		public Device Device { get; set; }
		[NotMapped]
		public DriverParameter DriverParameter { get; set; }
		public int Number { get; set; }
		public int? IntValue { get; set; }
		public double? DoubleValue { get; set; }
		public bool BoolValue { get; set; }
		[MaxLength(4000)]
		public string StringValue { get; set; }
		public DateTime? DateTimeValue { get; set; }
		public string GetStringValue()
		{
			switch (DriverParameter.ParameterType)
			{
				case ParameterType.Enum:
					if(IntValue == null)
						return "";
					var enumItem = DriverParameter.ParameterEnumItems.FirstOrDefault(x => x.Value == IntValue);
					if(enumItem == null)
						return "";
					return enumItem.Name;	
				case ParameterType.String:
					return StringValue;
				case ParameterType.Int:
					if(IntValue == null)
						return "";
					return IntValue.Value.ToString();
				case ParameterType.Double:
					if (DoubleValue == null)
						return "";
					return DoubleValue.Value.ToString();
				case ParameterType.Bool:
					return BoolValue ? "Да" : "Нет";
				case ParameterType.DateTime:
					if (DateTimeValue == null)
						return "";
					return DateTimeValue.Value.ToString();
				default:
					return "";
			}
		}
		[NotMapped]
		public ValueType ValueType
		{
			get
			{
				switch (DriverParameter.ParameterType)
				{
					case ParameterType.Enum:
						if (IntValue != null)
							return IntValue.Value;
						break;
					case ParameterType.String:
						return new ParameterStringContainer 
						{ 
							Value = StringValue, 
							RegEx = DriverParameter.RegEx 
						};
					case ParameterType.Int:
						if (IntValue != null)
							return IntValue.Value;
						break;
					case ParameterType.Double:
						if (DoubleValue != null)
							return DoubleValue.Value;
						break;
					case ParameterType.Bool:
						return BoolValue;
					case ParameterType.DateTime:
						if (DateTimeValue != null)
							return DateTimeValue.Value;
						break;
				}
				throw new ArgumentNullException("Не найдено значение параметра");
			}
		}
	}
}
