using System.Collections.Generic;

namespace Infrastructure.Common.Services.Layout
{
	public interface ILayoutDeclarationModule
	{
		IEnumerable<ILayoutPartDescription> GetLayoutPartDescriptions();
	}
}