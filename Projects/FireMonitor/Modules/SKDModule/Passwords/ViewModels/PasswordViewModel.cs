using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.TreeList;

namespace SKDModule.ViewModels
{
	public class PasswordViewModel : TreeNodeViewModel<PasswordViewModel>
	{
		public Organisation Organisation { get; private set; }
		public bool IsOrganisation { get; private set; }
		public string Name { get; private set; }
		public string Description { get; private set; }
		public ShortPassword Password { get; private set; }

		public PasswordViewModel(Organisation organisation)
		{
			Organisation = organisation;
			IsOrganisation = true;
			Name = organisation.Name;
			IsExpanded = true;
		}

		public PasswordViewModel(Organisation organisation, ShortPassword password)
		{
			Organisation = organisation;
			Password = password;
			IsOrganisation = false;
			Name = password.Name;
			Description = password.Description;
		}

		public void Update(ShortPassword password)
		{
			Name = password.Name;
			Description = password.Description;
			OnPropertyChanged("Name");
			OnPropertyChanged("Description");
		}
	}
}