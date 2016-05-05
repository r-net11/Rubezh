using System;
using StrazhAPI.Automation;

namespace StrazhAPI.Models.Automation
{
	public interface IVariable
	{
		Guid UID { get; set; }

		VariableValue VariableValue { get; set; }

		string Name { get; set; }

		bool IsReference { get; set; }
	}
}
