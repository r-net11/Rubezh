using System;
using Common;
using FiresecAPI.Models;
using Infrastructure.Client.Plans;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Designer;
using PlansModule.Designer;

namespace PlansModule.ViewModels
{
	public partial class PlanDesignerViewModel : BaseViewModel, IPlanDesignerViewModel
	{
		public event EventHandler Updated;
		public event EventHandler IsCollapsedChanged;
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
			using (new TimeCounter("\tPlanDesignerViewModel.Initialize: {0}"))
			using (new WaitWrapper())
			{
				using (new TimeCounter("\t\tDesignerCanvas.Initialize: {0}"))
					DesignerCanvas.Initialize(plan);
				if (Plan != null)
				{
					using (new TimeCounter("\t\tDesignerItem.Create: {0}"))
					{
						foreach (var elementBase in PlanEnumerator.Enumerate(Plan))
							DesignerCanvas.Create(elementBase);
						foreach (var element in DesignerCanvas.Toolbox.PlansViewModel.LoadPlan(Plan))
							DesignerCanvas.Create(element);
						DesignerCanvas.UpdateZIndex();
					}
					using (new TimeCounter("\t\tPlanDesignerViewModel.OnUpdated: {0}"))
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

		public CommonDesignerCanvas Canvas
		{
			get { return DesignerCanvas; }
		}

		public bool HasPermissionsToScale
		{ get { return true; } }

		public bool AlwaysShowScroll
		{
			get { return true; }
		}

		public bool CanCollapse
		{
			get { return true; }
		}

		private bool _isCollapsed;
		public bool IsCollapsed
		{
			get { return _isCollapsed; }
			set
			{
				if (IsCollapsed != value)
				{
					_isCollapsed = value;
					OnPropertyChanged(() => IsCollapsed);
					if (IsCollapsedChanged != null)
						IsCollapsedChanged(this, EventArgs.Empty);
				}
			}
		}

		#endregion
	}
}