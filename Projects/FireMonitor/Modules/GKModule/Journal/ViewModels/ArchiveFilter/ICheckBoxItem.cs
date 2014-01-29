namespace GKModule.ViewModels
{
	public interface ICheckBoxItem
	{
		bool IsChecked { get; set; }
		CheckBoxItemList ItemsList { get; set; }
	}
}
