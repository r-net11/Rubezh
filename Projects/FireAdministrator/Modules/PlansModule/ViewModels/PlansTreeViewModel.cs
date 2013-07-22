using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services.DragDrop;
namespace PlansModule.ViewModels
{
	public class PlansTreeViewModel
	{
		public PlansTreeViewModel(PlansViewModel plansViewModel)
		{
			PlansViewModel = plansViewModel;
			TreeNodeDropCommand = new RelayCommand<TreeNodeDropObject>(OnDrop, CanDrop);
		}

		public PlansViewModel PlansViewModel { get; private set; }

		public RelayCommand<TreeNodeDropObject> TreeNodeDropCommand { get; private set; }
		private void OnDrop(TreeNodeDropObject data)
		{
			var source = data.DataObject.GetData(typeof(PlanViewModel)) as PlanViewModel;
			var target = data.Target as PlanViewModel;
			var parent = source.Parent;
			if (parent == null)
			{
				PlansViewModel.Plans.Remove(source);
				FiresecManager.PlansConfiguration.Plans.Remove(source.Plan);
			}
			else
			{
				parent.RemoveChild(source);
				parent.Plan.Children.Remove(source.Plan);
				parent.Update();
				parent.IsExpanded = true;
			}
			if (target == null)
			{
				PlansViewModel.Plans.Add(source);
				FiresecManager.PlansConfiguration.Plans.Add(source.Plan);
				source.Plan.Parent = null;
			}
			else
			{
				target.IsExpanded = true;
				target.AddChild(source);
				target.Plan.Children.Add(source.Plan);
				source.Plan.Parent = target.Plan;
			}
			FiresecManager.PlansConfiguration.Update();
			ServiceFactory.SaveService.PlansChanged = true;
			PlansViewModel.SelectedPlan = source;
		}
		private bool CanDrop(TreeNodeDropObject data)
		{
			var source = data.DataObject.GetData(typeof(PlanViewModel)) as PlanViewModel;
			if (source == null)
				return false;
			var target = data.Target as PlanViewModel;
			if (target == null && source.Parent != null)
				return true;
			if (target == source || target == source.Parent || target.GetAllParents().Contains(source))
				return false;
			return true;
		}
	}
}