namespace PlansModule.ViewModels
{
	public class PlansTreeViewModel
	{
		public PlansTreeViewModel(PlansViewModel plansViewModel)
		{
			PlansViewModel = plansViewModel;
		}

		public PlansViewModel PlansViewModel { get; private set; }
	}
}