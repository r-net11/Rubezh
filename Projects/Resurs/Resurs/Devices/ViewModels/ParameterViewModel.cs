using Infrastructure.Common.Windows.Windows.ViewModels;
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
			Name = Model.DriverParameter.Description;
			Value = Model.GetStringValue();
		}

		public string Name { get; private set; }
		public string Value { get; private set; }
	}
}
