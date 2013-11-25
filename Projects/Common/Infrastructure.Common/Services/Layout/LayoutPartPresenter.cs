using System;
using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Common.Services.Layout
{
	public class LayoutPartPresenter : ILayoutPartPresenter
	{
		#region ILayoutPartPresenter Members

		public Guid UID { get; set; }
		public string Name { get; set; }
		public string IconSource { get; set; }
		public BaseViewModel Content { get; set; }

		#endregion
	}
}