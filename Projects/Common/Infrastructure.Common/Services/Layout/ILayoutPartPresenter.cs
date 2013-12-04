using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Models.Layouts;

namespace Infrastructure.Common.Services.Layout
{
	public interface ILayoutPartPresenter
	{
		Guid UID { get; }
		string Name { get; }
		string IconSource { get; }
		BaseViewModel CreateContent(ILayoutProperties properties);
	}
}