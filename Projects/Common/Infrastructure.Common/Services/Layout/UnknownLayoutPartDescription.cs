using System;
using Localization.Common.InfrastructureCommon;

namespace Infrastructure.Common.Services.Layout
{
	public class UnknownLayoutPartDescription : LayoutPartDescription
	{
		public UnknownLayoutPartDescription(Guid uid)
			: base(LayoutPartDescriptionGroup.Root, uid, 1, CommonResources.UnknownElement, null, "BClose.png")
		{
		}
	}
}