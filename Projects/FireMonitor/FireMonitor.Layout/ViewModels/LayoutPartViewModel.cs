using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Models.Layouts;
using Infrastructure.Common.Services.Layout;

namespace FireMonitor.Layout.ViewModels
{
	public class LayoutPartViewModel : BaseViewModel
	{
		public ILayoutPartPresenter LayoutPartPresenter { get; private set; }
		public LayoutPart LayoutPart { get; private set; }

		public LayoutPartViewModel(LayoutPart layoutPart, ILayoutPartPresenter layoutPartPresenter)
		{
			LayoutPart = layoutPart;
			LayoutPartPresenter = layoutPartPresenter;
			Content = LayoutPartPresenter.CreateContent(LayoutPart.Properties);
		}

		public Guid UID
		{
			get { return LayoutPart.UID; }
		}
		public string Title
		{
			get { return LayoutPartPresenter.Name; }
		}
		public string IconSource
		{
			get { return LayoutPartPresenter.IconSource; }
		}
		public object Content { get; private set; }
	}
}