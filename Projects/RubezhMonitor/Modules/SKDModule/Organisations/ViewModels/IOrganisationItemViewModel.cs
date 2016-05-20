namespace SKDModule.ViewModels
{
	public interface IOrganisationItemViewModel
	{
		bool IsChecked { get; set; }

		bool CanChange { get; }
	}
}