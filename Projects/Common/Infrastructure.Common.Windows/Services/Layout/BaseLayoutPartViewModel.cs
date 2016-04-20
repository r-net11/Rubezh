using System.Collections.Generic;
using RubezhAPI.Models.Layouts;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace Infrastructure.Common.Windows.Services.Layout
{
	public abstract class BaseLayoutPartViewModel : BaseViewModel
	{
		public abstract ILayoutProperties Properties { get; }
		public abstract IEnumerable<LayoutPartPropertyPageViewModel> PropertyPages { get; }
	}
}