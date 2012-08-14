using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Common.Validation
{
	public interface IValidationModule
	{
		IEnumerable<IValidationError> Validate();
	}
}
