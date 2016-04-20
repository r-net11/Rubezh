using System.Collections.Generic;

namespace Infrastructure.Common.Windows.Services.Layout
{
	public interface ILayoutDeclarationModule
	{
		IEnumerable<ILayoutPartDescription> GetLayoutPartDescriptions();
	}
}