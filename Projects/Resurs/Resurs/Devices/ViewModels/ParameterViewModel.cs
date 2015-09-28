using Infrastructure.Common.Windows.ViewModels;
using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resurs.ViewModels
{
	public class ParameterViewModel
	{
		public Parameter Model { get; private set; }
		public ParameterViewModel(Parameter model)
		{
			Model = model;
			Name = Model.DriverParameter.Name;
			switch (Model.DriverParameter.ParameterType)
			{
				case ParameterType.Enum:
					var enumItem = Model.DriverParameter.ParameterEnumItems.FirstOrDefault(x => x.Value == Model.IntValue);
					if (enumItem != null)
						Value = enumItem.Name;
					break;
				case ParameterType.String:
					Value = Model.StringValue;
					break;
				case ParameterType.Int:
					Value = Model.IntValue != null ? Model.IntValue.Value.ToString() : "NULL";
					break;
				case ParameterType.Double:
					Value = Model.DoubleValue != null ? Model.DoubleValue.Value.ToString() : "NULL";
					break;
				case ParameterType.Bool:
					Value = Model.BoolValue.ToString();
					break;
				case ParameterType.DateTime:
					Value = Model.DateTimeValue != null ? Model.DateTimeValue.Value.ToString() : "NULL";
					break;
				default:
					break;
			}
		}

		public string Name { get; private set; }
		public string Value { get; private set; }
	}
}
