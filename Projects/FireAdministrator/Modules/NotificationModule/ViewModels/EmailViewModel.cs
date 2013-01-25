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
				return Infrastructure.Common.Mail.MailHelper.PresentStates(Email);
			}
		}
	}
}