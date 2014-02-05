using System;
using FiresecAPI.Models.Layouts;
using Infrastructure.Common.Windows.ViewModels;

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