using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using Common;
using Infrastructure.Common;
using PlansModule.Designer;
using Infrastructure.Client.Plans;
using Infrustructure.Plans.Designer;
using Infrastructure.Designer.ViewModels;
using PlansModule.InstrumentAdorners;

namespace PlansModule.ViewModels
{
	public class PlanDesignerViewModel : Infrastructure.Designer.ViewModels.PlanDesignerViewModel
	{
		public Plan Plan { get; private set; }
		public PlansViewModel PlansViewModel { get; private set; }

		public PlanDesignerViewModel(PlansViewModel plansViewModel)
		{
			PlansViewModel = plansViewModel;
			DesignerCanvas = new DesignerCanvas(this);
			DesignerCanvas.Toolbox.RegisterInstruments(new[]{
				new InstrumentViewModel()
				{
				    ImageSource="/Controls;component/Images/Subplan.png",
				    ToolTip="Подплан",
				    Index = 300,
				    Adorner = new SubPlanAdorner(DesignerCanvas),
				}});
		}

		public void Initialize(Plan plan)
		{
			Plan = plan;
			OnPropertyChanged(() => Plan);
			IsNotEmpty = Plan != null;
			OnPropertyChanged(() => IsNotEmpty);
			using (new TimeCounter("\tPlanDesignerViewModel.Initialize: {0}"))
			using (new WaitWrapper())
			{
				using (new TimeCounter("\t\tDesignerCanvas.Initialize: {0}"))
					((DesignerCanvas)DesignerCanvas).Initialize(plan);
				if (Plan != null)
				{
					using (new TimeCounter("\t\tDesignerItem.Create: {0}"))
					{
						foreach (var elementBase in PlanEnumerator.Enumerate(Plan))
							DesignerCanvas.Create(elementBase);
						foreach (var element in PlansViewModel.LoadPlan(Plan))
							DesignerCanvas.Create(element);
						DesignerCanvas.UpdateZIndex();
					}
					using (new TimeCounter("\t\tPlanDesignerViewModel.OnUpdated: {0}"))
						Update();
				}
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
