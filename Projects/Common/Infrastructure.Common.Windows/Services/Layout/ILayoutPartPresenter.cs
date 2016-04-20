using System;
using RubezhAPI.Models.Layouts;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace Infrastructure.Common.Windows.Services.Layout
{
	public interface ILayoutPartPresenter
	{
		Guid UID { get; }
		string Name { get; }
		string IconSource { get; }
		BaseViewModel CreateContent(ILayoutProperties properties);
	}
}