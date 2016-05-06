using StrazhAPI.Models.Layouts;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;

namespace Infrastructure.Common.Services.Layout
{
	public abstract class BaseLayoutPartViewModel : BaseViewModel
	{
		public abstract ILayoutProperties Properties { get; }

		public abstract IEnumerable<LayoutPartPropertyPageViewModel> PropertyPages { get; }
	}
}