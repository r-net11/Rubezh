using FiresecAPI;
using Infrastructure.Common.TreeList;

namespace SKDModule.ViewModels
{
	public class AdditionalColumnTypeViewModel : TreeNodeViewModel<AdditionalColumnTypeViewModel>
	{
		public Organisation Organisation { get; private set; }
		public bool IsOrganisation { get; private set; }
		public string Name { get; private set; }
		public string Description { get; private set; }
		public ShortAdditionalColumnType AdditionalColumnType { get; private set; }

		public AdditionalColumnTypeViewModel(Organisation organisation)
		{
			Organisation = organisation;
			IsOrganisation = true;
			Name = organisation.Name;
			IsExpanded = true;
		}

		public AdditionalColumnTypeViewModel(ShortAdditionalColumnType additionalColumnType)
		{
			AdditionalColumnType = additionalColumnType;
			IsOrganisation = false;
			Name = additionalColumnType.Name;
			Description = additionalColumnType.Description;
		}

		public void Update(ShortAdditionalColumnType additionalColumnType)
		{
			Name = additionalColumnType.Name;
			Description = additionalColumnType.Description;
			OnPropertyChanged("Name");
			OnPropertyChanged("Description");
		}
	}
}