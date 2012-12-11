using System;
using Common;
using FiresecAPI.Models;
using Infrastructure.Client.Plans;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using PlansModule.Designer;

namespace PlansModule.ViewModels
{
	public partial class PlanDesignerViewModel : BaseViewModel, IPlanDesignerViewModel
	{
		public event EventHandler Updated;
		public DesignerCanvas DesignerCanvas { get; set; }
		public Plan Plan { get; private set; }

		public PlanDesignerViewModel()
		{
			InitializeZIndexCommands();
			InitializeAlignCommands();
		}

		public void Initialize(Plan plan)
		{
			using (new TimeCounter("\tPlanDesignerViewModel.Initialize: {0}"))
			{
				Plan = plan;
				OnPropertyChanged("Plan");
				if (Plan != null)
				{
					using (new WaitWrapper())
					{
						using (new TimeCounter("\t\tDesignerCanvas.Clear: {0}"))
							DesignerCanvas.Clear();
						ChangeZoom(1);
						DesignerCanvas.Plan = plan;
						DesignerCanvas.PlanDesignerViewModel = this;
						using (new TimeCounter("\t\tDesignerCanvas.Background: {0}"))
							DesignerCanvas.Update();

						using (new TimeCounter("\t\tDesignerItem.Create: {0}"))
						{

							// 1. Override standart Painters				+
							// 4. Change ResizeDecorator/ResizeAdorner		-

							foreach (var elementBase in PlanEnumerator.Enumerate(plan))
								DesignerCanvas.Create(elementBase);
							foreach (var element in DesignerCanvas.Toolbox.PlansViewModel.LoadPlan(plan))
								DesignerCanvas.Create(element);
						}
					}
					OnUpdated();
				}
			}
		}

		public void Save()
		{
			if (Plan == null)
				return;
			NormalizeZIndex();
		}

		public void Update()
		{
			OnUpdated();
		}

		private void OnUpdated()
		{
			if (Updated != null)
				Updated(this, EventArgs.Empty);
		}

		#region IPlanDesignerViewModel Members

		public object Toolbox
		{
			get { return DesignerCanvas.Toolbox; }
		}

		public object Canvas
		{
			get { return DesignerCanvas; }
		}

		public bool HasPermissionsToScale
		{ get { return true; } }

		#endregion
	}
}