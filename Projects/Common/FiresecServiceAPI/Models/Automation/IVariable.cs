using System;
using FiresecAPI.Automation;

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
