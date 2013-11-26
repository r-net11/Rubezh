using System.Collections.Generic;

namespace Infrastructure.Common.Services.Layout
{
	public interface ILayoutProviderModule
	{
		IEnumerable<ILayoutPartPresenter> GetLayoutParts();
	}
}