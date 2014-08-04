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
			if (organisations == null)
				return;
			var positions = PositionHelper.GetByCurrentUser();
			if (positions == null)
				return;
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
			get 
			{ 
				if(Organisations != null)
					return Organisations.ToArray();
				return null;
			}
		}

		public List<Guid> UIDs
		{
			get
			{
				var result = new List<Guid>();
				if (AllPositions != null)
				{
					foreach (var department in AllPositions)
					{
						if (department.IsChecked && !department.IsOrganisation)
						{
							result.Add(department.UID);
						}
					}
				}
				return result;
			}
		}
	}
}