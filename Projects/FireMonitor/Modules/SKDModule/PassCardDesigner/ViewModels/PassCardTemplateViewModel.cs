using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.ViewModels;

namespace SKDModule.PassCardDesigner.ViewModels
{
	public class PassCardTemplateViewModel : CartothequeTabItemElementBase<PassCardTemplateViewModel, ShortPassCardTemplate>
	{
		public PassCardTemplate PassCardTemplate { get; private set; }

		public PassCardTemplateViewModel()
		{
		}
		public PassCardTemplateViewModel(PassCardTemplate passCardTemplate)
		{
			PassCardTemplate = passCardTemplate;
		}

		public string Caption
		{
			get { return PassCardTemplate.Caption; }
		}
	}
}