using Infrastructure.Common.Windows.ViewModels;
using Localization.SKD.ViewModels;
using SKDModule.PassCardDesigner.Model;

namespace SKDModule.PassCardDesigner.ViewModels
{
	public class TemplatePropertiesViewModel : SaveCancelDialogViewModel
	{
		public Template PassCardTemplate { get; private set; }

		public TemplatePropertiesViewModel(Template passCardTemplate)
		{
			Title = CommonViewModels.PasscardTemplProperties;
			PassCardTemplate = passCardTemplate;
		}
		
		protected override bool CanSave()
		{
			return PassCardTemplate != null && !string.IsNullOrEmpty(PassCardTemplate.Caption);
		}

		protected override bool Save()
		{
			PassCardTemplate.Save();
			return base.Save();
		}
	}
}