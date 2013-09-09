namespace Infrastructure.Common.Windows.ViewModels
{
	public interface IEditingViewModel
	{
		RelayCommand AddCommand { get; }
		RelayCommand DeleteCommand { get; }
		RelayCommand EditCommand { get; }
	}
}