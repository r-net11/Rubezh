using StrazhAPI.Models.Layouts;
using Infrastructure.Common.Windows.ViewModels;
using System;

namespace Infrastructure.Common.Services.Layout
{
	public class LayoutPartPresenter : ILayoutPartPresenter
	{
		private const string IconPath = "/Controls;component/Images/";

		public Converter<ILayoutProperties, BaseViewModel> Factory { get; set; }

		public LayoutPartPresenter(Guid uid, string name, string iconName, Converter<ILayoutProperties, BaseViewModel> factory)
		{
			UID = uid;
			Name = name;
			if (!string.IsNullOrEmpty(iconName))
				IconSource = IconPath + iconName;
			Factory = factory;
		}

		#region ILayoutPartPresenter Members

		public Guid UID { get; set; }

		public string Name { get; set; }

		public string IconSource { get; set; }

		public BaseViewModel CreateContent(ILayoutProperties properties)
		{
			return Factory(properties);
		}

		#endregion ILayoutPartPresenter Members
	}
}