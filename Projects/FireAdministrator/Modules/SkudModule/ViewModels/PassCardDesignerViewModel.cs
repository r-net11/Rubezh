using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.ViewModels;
using FiresecAPI.Models.Skud;
using Infrastructure.Designer;

namespace SkudModule.ViewModels
{
	public class PassCardDesignerViewModel : Infrastructure.Designer.ViewModels.PlanDesignerViewModel
	{
		public PassCardDesignerViewModel()
		{
			DesignerCanvas = new DesignerCanvas(this);
		}

		public void Initialize(PassCardTemplate PassCardTemplate)
		{
			IsNotEmpty = PassCardTemplate != null;
		}

	}
}