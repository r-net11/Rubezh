using System.Collections.Generic;
using System.Text;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace NotificationModule.ViewModels
{
	public class EmailViewModel : BaseViewModel
	{
		public EmailViewModel(Email email)
		{
			Email = email;
		}

		Email _email;

		public Email Email
		{
			get { return _email; }
			set
			{
				_email = value;
				OnPropertyChanged("Email");
				OnPropertyChanged("PresenrationStates");
			}
		}

		public string PresenrationStates
		{
			get
			{
				var presenrationStates = new StringBuilder();
				if (Email.SendingStates == null)
					Email.SendingStates = new List<StateType>();
				for (int i = 0; i < Email.SendingStates.Count; i++)
				{
					if (i > 0)
						presenrationStates.Append(", ");
					presenrationStates.Append(Email.SendingStates[i].ToDescription());
				}
				return presenrationStates.ToString();
			}
		}
	}
}