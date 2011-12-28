namespace Infrastructure.Common
{
    public interface IEditingViewModel
    {
        RelayCommand AddCommand { get; }
        RelayCommand DeleteCommand { get; }
        RelayCommand EditCommand { get; }
    }
}