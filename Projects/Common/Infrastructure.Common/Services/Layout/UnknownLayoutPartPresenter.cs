using System;

namespace Infrastructure.Common.Services.Layout
{
	public class UnknownLayoutPartPresenter : LayoutPartPresenter
	{
		public UnknownLayoutPartPresenter(Guid uid)
			: base(uid, Resources.Language.UnknownLayoutPartPresenter.Name, Resources.Language.UnknownLayoutPartPresenter.IconName, (p) => null)
		{
		}
	}
}