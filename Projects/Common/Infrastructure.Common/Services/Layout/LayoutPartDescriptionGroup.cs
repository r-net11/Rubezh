using System.ComponentModel;
using Localization.Common.InfrastructureCommon;
using Localization.Converters;

namespace Infrastructure.Common.Services.Layout
{
	public enum LayoutPartDescriptionGroup
	{
		Root,

		[LocalizedDescription(typeof(CommonResources),"Common")]
		//[Description("Общие")]
		Common,

		[LocalizedDescription(typeof(CommonResources),"SKD")]
		//[Description("СКД")]
		SKD,

		[LocalizedDescription(typeof(CommonResources),"Video")]
		//[Description("Видео")]
		Video,

		[LocalizedDescription(typeof(CommonResources),"ControlElements")]
		//[Description("Элементы управления")]
		Control,
	}
}