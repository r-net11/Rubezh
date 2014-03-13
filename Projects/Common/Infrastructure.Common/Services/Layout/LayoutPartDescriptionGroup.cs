using System;
using FiresecAPI.Models.Layouts;
using System.ComponentModel;

namespace Infrastructure.Common.Services.Layout
{
	public enum LayoutPartDescriptionGroup
	{
		Root,
		[Description("Общие")]
		Common,
		[Description("Планы")]
		Plans,
		[Description("Видео")]
		Video,
		[Description("Монитор")]
		Monitor,
		[Description("ГК")]
		GK,
		[Description("СКД")]
		SKD,
	}
}
