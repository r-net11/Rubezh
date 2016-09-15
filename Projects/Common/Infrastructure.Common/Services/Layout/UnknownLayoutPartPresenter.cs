using System;
using Localization.Common.InfrastructureCommon;

namespace Infrastructure.Common.Services.Layout
{
	public class UnknownLayoutPartPresenter : LayoutPartPresenter
	{
		public UnknownLayoutPartPresenter(Guid uid)
			: base(uid, CommonResources.UnknownElement, "Close.png", (p) => null)
		{
		}
	}
}