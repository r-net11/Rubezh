﻿using System.Collections.ObjectModel;
using RubezhAPI.SKD;
using RubezhClient;

namespace SKDModule.ViewModels
{
	public class OrganisationDoorsViewModel : OrganisationItemsViewModel<OrganisationDoorViewModel>
	{
		public OrganisationDoorsViewModel(Organisation organisation):base(organisation)
		{
			Items = new ObservableCollection<OrganisationDoorViewModel>();
			foreach (var door in GKManager.DeviceConfiguration.Doors)  
			{
				Items.Add(new OrganisationDoorViewModel(Organisation, door));
			}
		}

		protected override RubezhAPI.Models.PermissionType Permission
		{
			get { return RubezhAPI.Models.PermissionType.Oper_SKD_Organisations_Doors; }
		}
	}
}