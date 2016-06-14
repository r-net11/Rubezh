using Infrastructure.Common.Windows.ViewModels;
using SKDModule.PassCardDesigner.Model;

namespace SKDModule.PassCardDesigner.ViewModels
{
	public class TemplatePropertiesViewModel : SaveCancelDialogViewModel
	{
		public Template PassCardTemplate { get; private set; }

		public TemplatePropertiesViewModel(Template passCardTemplate)
		{
			Title = "Свойства элемента: Шаблон пропуска";
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