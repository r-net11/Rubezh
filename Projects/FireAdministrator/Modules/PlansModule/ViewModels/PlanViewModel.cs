using FiresecAPI.Models;
using Infrastructure.Common.TreeList;

namespace PlansModule.ViewModels
{
	public class PlanViewModel : TreeItemViewModel<PlanViewModel>
	{
		public Plan Plan { get; private set; }
		public PlanFolder PlanFolder { get; private set; }

		public PlanViewModel(Plan plan)
		{
			Plan = plan;
			PlanFolder = plan as PlanFolder;
		}

		public void Update()
		{
			IsExpanded = false;
			IsExpanded = true;
			OnPropertyChanged(() => HasChildren);
			OnPropertyChanged(() => Caption);
			OnPropertyChanged(() => Description);
		}

		public string Caption
		{
			get { return Plan.Caption; }
		}
		public string Description
		{
			get { return Plan.Description; }
		}
	}
}