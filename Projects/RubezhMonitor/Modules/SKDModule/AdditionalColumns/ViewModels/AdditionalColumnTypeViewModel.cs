using RubezhAPI;
using RubezhAPI.SKD;

namespace SKDModule.ViewModels
{
	public class AdditionalColumnTypeViewModel : OrganisationElementViewModel<AdditionalColumnTypeViewModel, AdditionalColumnType> 
	{
		public string DataType { get { return IsOrganisation ? "" : Model.DataType.ToDescription(); } }
	}
}