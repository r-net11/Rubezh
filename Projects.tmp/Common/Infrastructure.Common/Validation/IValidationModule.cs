using System.Collections.Generic;

namespace Infrastructure.Common.Validation
{
	public interface IValidationModule
	{
		IEnumerable<IValidationError> Validate();
	}
}