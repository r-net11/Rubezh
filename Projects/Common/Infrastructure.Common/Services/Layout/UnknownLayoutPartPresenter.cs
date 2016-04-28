using System;

namespace Infrastructure.Common.Services.Layout
{
	public class UnknownLayoutPartPresenter : LayoutPartPresenter
	{
		public UnknownLayoutPartPresenter(Guid uid)
			: base(uid, Resources.Language.Services.Layout.UnknownLayoutPartPresenter.Name, Resources.Language.Services.Layout.UnknownLayoutPartPresenter.IconName, (p) => null)
		{
		}
	}
}