using Common;
using RubezhAPI.Models;
using Infrastructure.Client.Plans;
using Infrastructure.Common.Windows;
using Infrustructure.Plans.Designer;
using PlansModule.Designer;
using PlansModule.InstrumentAdorners;
using Infrastructure;
using System.Linq;
using Infrastructure.Designer.Events;
using System;
using RubezhClient;

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
			DesignerCanvas.Toolbox.IsRightPanel = true;
			DesignerCanvas.Toolbox.RegisterInstruments(new[]
			{
				new InstrumentViewModel()
				{
					ImageSource="Subplan",
					ToolTip="Ссылка на план",
					Index = 1000,
					Adorner = new SubPlanRectangleAdorner(DesignerCanvas),
					GroupIndex = 1000,
				},
				new InstrumentViewModel()
				{
					ImageSource="Subplan",
					ToolTip="Ссылка на план",
					Index = 1001,
					Adorner = new SubPlanPolygonAdorner(DesignerCanvas),
					GroupIndex = 1000,
				},
			});
			AllowScalePoint = true;
			FullScreenSize = true;
		}

		public void Initialize(Plan plan)
		{
			Plan = plan;
			OnPropertyChanged(() => Plan);
			IsNotEmpty = Plan != null;
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
