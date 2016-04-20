using System.Collections.Generic;

namespace Infrastructure.Common.Windows.Validation
{
	public interface IValidationModule
	{
		IEnumerable<IValidationError> Validate();
	}
}