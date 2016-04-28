using System.ComponentModel;
using Localization;

namespace Infrastructure.Common.Services.Layout
{
	public enum LayoutPartDescriptionGroup
	{
		Root,

		//[Description("Общие")]
        [LocalizedDescription(typeof(Resources.Language.Services.Layout.LayoutPartDescriptionGroup), "Common")]
		Common,

        //[Description("СКД")]
        [LocalizedDescription(typeof(Resources.Language.Services.Layout.LayoutPartDescriptionGroup), "SKD")]
		SKD,

        //[Description("Видео")]
        [LocalizedDescription(typeof(Resources.Language.Services.Layout.LayoutPartDescriptionGroup), "Video")]
		Video,

        //[Description("Элементы управления")]
        [LocalizedDescription(typeof(Resources.Language.Services.Layout.LayoutPartDescriptionGroup), "Control")]
		Control,
	}
}