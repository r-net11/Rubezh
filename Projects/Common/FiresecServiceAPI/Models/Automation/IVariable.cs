using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecAPI.Models.Automation
{
	public interface IVariable
	{
		Guid UID { get; set; }

		VariableValue VariableValue { get; set; }

		string Name { get; set; }

		bool IsReference { get; set; }
	}
}
