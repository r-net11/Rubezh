using System.Collections.ObjectModel;
using System.Linq;
using RubezhClient;
using Infrastructure.Common.Windows.ViewModels;

namespace SecurityModule.ViewModels
{
	public class RoleSelectationViewModel : SaveCancelDialogViewModel
	{
		public RoleSelectationViewModel()
		{
			Title = "Выбор шаблона прав";

			Roles = new ObservableCollection<RoleViewModel>();
			foreach (var role in ClientManager.SecurityConfiguration.UserRoles)
				Roles.Add(new RoleViewModel(role));
			SelectedRole = Roles.FirstOrDefault();
		}

		public ObservableCollection<RoleViewModel> Roles { get; private set; }

		RoleViewModel _selectedRole;
		public RoleViewModel SelectedRole
		{
			get { return _selectedRole; }
			set
			{
				_selectedRole = value;
				OnPropertyChanged(() => SelectedRole);
			}
		}

		protected override bool Save()
		{
			return SelectedRole != null;
		}
	}
}