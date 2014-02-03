namespace GKModule.ViewModels
{
	public interface ICheckBoxItem
	{
		bool IsChecked { get; set; }
		ICheckBoxItemList ItemsList { get; set; }
	}
}
