namespace Infrastructure.Common.Windows.ViewModels
{
	public interface IEditingViewModel : IEditingBaseViewModel
	{
		RelayCommand AddCommand { get; }
		RelayCommand DeleteCommand { get; }
	}
}