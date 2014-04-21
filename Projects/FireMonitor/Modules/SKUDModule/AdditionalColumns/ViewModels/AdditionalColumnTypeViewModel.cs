using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;
using Infrastructure.Common.TreeList;

namespace SKDModule.ViewModels
{
	public class AdditionalColumnTypeViewModel : TreeNodeViewModel<AdditionalColumnTypeViewModel>
	{
		public Organisation Organization { get; private set; }
		public AdditionalColumnType AdditionalColumnType { get; private set; }

		public AdditionalColumnTypeViewModel(Organisation organization)
		{
			Organization = organization;
			IsOrganisation = true;
			Name = organization.Name;
		}

		public AdditionalColumnTypeViewModel(AdditionalColumnType additionalColumnType)
		{
			AdditionalColumnType = additionalColumnType;
			IsOrganisation = false;
			Name = additionalColumnType.Name;
			Description = additionalColumnType.Description;
		}

		public void Update(AdditionalColumnType additionalColumnType)
		{
			Name = additionalColumnType.Name;
			Description = additionalColumnType.Description;
			OnPropertyChanged("Name");
			OnPropertyChanged("Description");
		}

		public bool IsOrganisation { get; private set; }

		public string Name { get; private set; }
		public string Description { get; private set; }
	}
}