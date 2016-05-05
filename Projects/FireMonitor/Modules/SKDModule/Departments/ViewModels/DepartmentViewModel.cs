using StrazhAPI.SKD;

namespace SKDModule.ViewModels
{
	public class DepartmentViewModel : OrganisationElementViewModel<DepartmentViewModel, ShortDepartment>
	{
		public override void InitializeModel(Organisation organisation, ShortDepartment model, Infrastructure.Common.Windows.ViewModels.ViewPartViewModel parentViewModel)
		{
			base.InitializeModel(organisation, model, parentViewModel);
		}

		public string Phone
		{
			get
			{
				if (IsOrganisation)
					return Organisation.Phone;
				else
					return Model.Phone;
			}
		}

		public override void Update()
		{
			base.Update();
			OnPropertyChanged(() => Phone);
		}
	}
}