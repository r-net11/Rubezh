using FiresecAPI;

namespace SKDModule.ViewModels
{
	public class DocumentFilterViewModel : OrganizationFilterBaseViewModel<DocumentFilter>
	{
		public DocumentFilterViewModel(DocumentFilter filter) : base(filter) { }
	}
}