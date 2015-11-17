using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.SKD;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GKSDK
{
	public class SKDViewModel : BaseViewModel
	{
		public SKDViewModel()
		{
			AddCommand = new RelayCommand(OnAdd);
		}


		string _lastName;
		public string LastName
		{
			get { return _lastName; }
			set
			{
				_lastName = value;
				OnPropertyChanged(() => LastName);
			}
		}

		string _firstName;
		public string FirstName
		{
			get { return _firstName; }
			set
			{
				_firstName = value;
				OnPropertyChanged(() => FirstName);
			}
		}

		string _secondName;
		public string SecondName
		{
			get { return _secondName; }
			set
			{
				_secondName = value;
				OnPropertyChanged(() => SecondName);
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var employee = new Employee();
			employee.LastName = LastName;
			employee.FirstName = FirstName;
			employee.SecondName = SecondName;
			var organisationsResult = ClientManager.FiresecService.GetOrganisations(new OrganisationFilter());
			if(organisationsResult.HasError || organisationsResult.Result.Count == 0)
				return;
			var organisation = organisationsResult.Result.FirstOrDefault();
			employee.OrganisationUID = organisation.UID;
			employee.Type = PersonType.Guest;
			ClientManager.FiresecService.SaveEmployee(employee, true);
		}
	}
}