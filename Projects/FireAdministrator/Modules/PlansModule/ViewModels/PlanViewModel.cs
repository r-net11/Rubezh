using RubezhAPI.Models;
using Infrastructure.Common.Windows.TreeList;

namespace PlansModule.ViewModels
{
	public class PlanViewModel : TreeNodeViewModel<PlanViewModel>
	{
		public Plan Plan { get; private set; }
		public PlanFolder PlanFolder { get; private set; }

		public PlanViewModel(Plan plan)
		{
			Plan = plan;
			PlanFolder = plan as PlanFolder;
			IsExpanded = true;
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
		public bool IsFolder
		{
			get { return PlanFolder != null; }
		}

		private bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged(() => IsChecked);
				if (IsChecked && Parent != null)
					Parent.IsChecked = true;
				else if (!IsChecked)
					foreach (var child in Children)
						child.IsChecked = false;
			}
		}
	}
}
