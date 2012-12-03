using System;
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
			Plan = plan;
			OnPropertyChanged("Plan");
			if (Plan != null)
			{
				using (new WaitWrapper())
				{
					ChangeZoom(1);
					DesignerCanvas.Plan = plan;
					DesignerCanvas.PlanDesignerViewModel = this;
					DesignerCanvas.Update();
					DesignerCanvas.Children.Clear();
					DesignerCanvas.Width = plan.Width;
					DesignerCanvas.Height = plan.Height;
					OnUpdated();

					foreach (var elementBase in PlanEnumerator.Enumerate(plan))
						DesignerCanvas.Create(elementBase);
					foreach (var element in DesignerCanvas.Toolbox.PlansViewModel.LoadPlan(plan))
						DesignerCanvas.Create(element);
					DesignerCanvas.DeselectAll();
				}
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

		#endregion
	}
}