using System;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Models.Layouts;

namespace Infrastructure.Common.Services.Layout
{
	public class LayoutPartPresenter : ILayoutPartPresenter
	{
		public Converter<ILayoutProperties, BaseViewModel> Factory { get; set; }

		#region ILayoutPartPresenter Members

		public Guid UID { get; set; }
		public string Name { get; set; }
		public string IconSource { get; set; }
		public BaseViewModel CreateContent(ILayoutProperties properties)
		{
			return Factory(properties);
		}

		#endregion
	}
}