using FiresecAPI;
using FiresecAPI.SKD;

namespace SKDModule.ViewModels
{
	public class AdditionalColumnTypeViewModel : OrganisationElementViewModel<AdditionalColumnTypeViewModel, AdditionalColumnType> 
	{
		public string DataType { get { return IsOrganisation ? "" : Model.DataType.ToDescription(); } }
	}
}