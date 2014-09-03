using FiresecAPI.SKD;

namespace SKDModule.ViewModels
{
	public class DepartmentViewModel : CartothequeTabItemElementBase<DepartmentViewModel, ShortDepartment>
	{
		public override string Description
		{
			get { return IsOrganisation ? Organisation.Description : Model.Description; }
			protected set
			{
				if (IsOrganisation)
					Organisation.Description = value;
				else
					Model.Description = value;
			}
		}
	}
}