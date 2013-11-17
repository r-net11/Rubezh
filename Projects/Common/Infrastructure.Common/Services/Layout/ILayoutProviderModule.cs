using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Common.Services.Layout
{
	public interface ILayoutProviderModule
	{
		IEnumerable<ILayoutPartPresenter> GetLayoutParts();
	}
}
