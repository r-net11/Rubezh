using Infrastructure.Client.Plans;
using Infrustructure.Plans.Designer;
using PlansModule.Designer;
using PlansModule.InstrumentAdorners;
using StrazhAPI.Models;

namespace PlansModule.ViewModels
{
	public class PlanDesignerViewModel : Infrastructure.Designer.ViewModels.PlanDesignerViewModel
	{
		public Plan Plan { get; private set; }
		public PlansViewModel PlansViewModel { get; private set; }

		public PlanDesignerViewModel(PlansViewModel plansViewModel)
		{
			PlansViewModel = plansViewModel;
			DesignerCanvas = new DesignerCanvas(this) {Toolbox = {IsRightPanel = true}};
			DesignerCanvas.Toolbox.RegisterInstruments(new[]{
				new InstrumentViewModel
				{
					ImageSource="Subplan",
					ToolTip="Ссылка на план",
					Index = 1000,
					Adorner = new SubPlanAdorner(DesignerCanvas),
				}});
			AllowScalePoint = true;
			FullScreenSize = true;
		}

		public void Initialize(Plan plan)
		{
			Plan = plan;
			OnPropertyChanged(() => Plan);
			IsNotEmpty = Plan != null;
			((DesignerCanvas)DesignerCanvas).Initialize(plan);

			if (Plan != null)
			{
				foreach (var elementBase in PlanEnumerator.Enumerate(Plan))
					DesignerCanvas.Create(elementBase);

				foreach (var element in PlansViewModel.LoadPlan(Plan))
					DesignerCanvas.Create(element);

				DesignerCanvas.UpdateZIndex();
				Update();
			}

			ResetHistory();
		}

		public override void RegisterDesignerItem(DesignerItem designerItem)
		{
			base.RegisterDesignerItem(designerItem);
			PlansViewModel.RegisterDesignerItem(designerItem);
		}
	}
}
