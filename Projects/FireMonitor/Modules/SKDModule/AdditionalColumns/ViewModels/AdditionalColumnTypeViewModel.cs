using StrazhAPI;
using StrazhAPI.SKD;

namespace SKDModule.ViewModels
{
	public class AdditionalColumnTypeViewModel : OrganisationElementViewModel<AdditionalColumnTypeViewModel, ShortAdditionalColumnType> 
	{
		public string DataType { get { return IsOrganisation ? "" : Model.DataType.ToDescription(); } }
	}
}