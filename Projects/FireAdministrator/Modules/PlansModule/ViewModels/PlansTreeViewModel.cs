using Infrastructure.Common;
using System.Windows;
namespace PlansModule.ViewModels
{
	public class PlansTreeViewModel
	{
		public PlansTreeViewModel(PlansViewModel plansViewModel)
		{
			PlansViewModel = plansViewModel;
			DropCommand = new RelayCommand<IDataObject>(OnDrop, CanDrop);
		}

		public PlansViewModel PlansViewModel { get; private set; }

		public RelayCommand<IDataObject> DropCommand { get; private set; }
		private void OnDrop(IDataObject data)
		{
		}
		private bool CanDrop(IDataObject data)
		{
			return data.GetDataPresent(typeof(PlanViewModel));
		}
	}
}