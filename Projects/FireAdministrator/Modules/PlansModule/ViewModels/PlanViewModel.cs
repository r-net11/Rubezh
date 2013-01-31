using FiresecAPI.Models;
using Infrastructure.Common;

namespace PlansModule.ViewModels
{
	public class PlanViewModel : TreeItemViewModel<PlanViewModel>
	{
		public Plan Plan { get; private set; }
		public PlanFolder PlanFolder { get; private set; }

		public PlanViewModel(Plan plan)
		{
			Plan = plan;
		}
		public PlanViewModel(PlanFolder planFolder)
		{
			PlanFolder = planFolder;
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
			get { return Plan == null ? PlanFolder.Caption : Plan.Caption; }
		}
		public string Description
		{
			get { return Plan == null ? PlanFolder.Description : Plan.Description; }
		}
	}
}