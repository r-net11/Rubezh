using FiresecAPI.SKD;

namespace SKDModule.ViewModels
{
	public class PositionViewModel : OrganisationElementViewModel<PositionViewModel, ShortPosition> 
	{
		public PositionEmployeeListViewModel EmployeeListViewModel { get; private set; }

		public override void InitializeModel(Organisation organisation, ShortPosition model, Infrastructure.Common.Windows.ViewModels.ViewPartViewModel parentViewModel)
		{
			base.InitializeModel(organisation, model, parentViewModel);
			EmployeeListViewModel = new PositionEmployeeListViewModel(this);
		}
	}
}