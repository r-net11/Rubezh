using FiresecAPI.SKD;

namespace SKDModule.ViewModels
{
	public class PositionViewModel : CartothequeTabItemElementBase<PositionViewModel, ShortPosition>
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