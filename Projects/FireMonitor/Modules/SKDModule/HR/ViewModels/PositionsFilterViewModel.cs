using System;
using System.Collections.Generic;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class PositionsFilterViewModel : BaseViewModel
	{
		public PositionsFilterViewModel(EmployeeFilter filter)
		{
			var organisations = OrganisationHelper.GetByCurrentUser();
			var positions = PositionHelper.GetByCurrentUser();
			AllPositions = new List<PositionFilterItemViewModel>();
			Organisations = new List<PositionFilterItemViewModel>();
			foreach (var organisation in organisations)
			{
				var organisationViewModel = new PositionFilterItemViewModel(organisation);
				Organisations.Add(organisationViewModel);
				AllPositions.Add(organisationViewModel);
				foreach (var position in positions)
				{
					if (position.OrganisationUID == organisation.UID)
					{
						var positionViewModel = new PositionFilterItemViewModel(position.Name, position.Description, position.UID);
						positionViewModel.IsChecked = filter.PositionUIDs.Contains(position.UID);
						organisationViewModel.AddChild(positionViewModel);
						AllPositions.Add(positionViewModel);
					}
				}
			}
		}

		public List<PositionFilterItemViewModel> AllPositions;
		public List<PositionFilterItemViewModel> Organisations { get; private set; }

		public PositionFilterItemViewModel[] RootPositions
		{
			get { return Organisations.ToArray(); }
		}

		public List<Guid> UIDs
		{
			get
			{
				var result = new List<Guid>();
				foreach (var department in AllPositions)
				{
					if (department.IsChecked && !department.IsOrganisation)
					{
						result.Add(department.UID);
					}
				}
				return result;
			}
		}
	}
}