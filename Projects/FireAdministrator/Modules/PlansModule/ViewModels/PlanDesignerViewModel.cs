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
				if (plan != null)
				{
					using (new TimeCounter("\t\tDesignerCanvas.RegisterPlan: {0}"))
						DesignerCanvas.RegisterPlan(plan);

					using (new TimeCounter("\t\tDesignerItem.Create: {0}"))
					{
						foreach (var elementBase in PlanEnumerator.Enumerate(plan))
							DesignerCanvas.Create(elementBase);
						foreach (var element in DesignerCanvas.Toolbox.PlansViewModel.LoadPlan(plan))
							DesignerCanvas.Create(element);
						using (new TimeCounter("\t\t\tDesignerCanvas.UpdateZIndex: {0}"))
							DesignerCanvas.UpdateZIndex();
					}
				}
		}
		public void SelectPlan(Plan plan)
		{
			using (new TimeCounter("\tPlanDesignerViewModel.SelectPlan: {0}"))
			{
				Plan = plan;
				OnPropertyChanged("Plan");
				DesignerCanvas.ShowPlan(plan);
				using (new WaitWrapper())
					if (Plan != null)
						using (new TimeCounter("\t\tPlanDesignerViewModel.OnUpdated: {0}"))
							OnUpdated();
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