using System.Collections.Generic;

namespace Infrastructure.Common.Windows.Services.Layout
{
	public interface ILayoutProviderModule
	{
		IEnumerable<ILayoutPartPresenter> GetLayoutParts();
	}
}