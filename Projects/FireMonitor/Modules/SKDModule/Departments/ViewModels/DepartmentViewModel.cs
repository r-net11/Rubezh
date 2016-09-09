using StrazhAPI.SKD;

namespace SKDModule.ViewModels
{
	public class DepartmentViewModel : OrganisationElementViewModel<DepartmentViewModel, ShortDepartment>
	{
		public string Phone
		{
			get
			{
				return IsOrganisation ? Organisation.Phone : Model.Phone;
			}
		}

		public override void Update()
		{
			base.Update();
			OnPropertyChanged(() => Phone);
		}
	}
}