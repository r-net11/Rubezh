using System.Linq;
using System.Text;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Mail;
using Infrastructure.Common.Windows.ViewModels;

namespace NotificationModule.ViewModels
{
	public class EmailViewModel : BaseViewModel
	{
		public EmailViewModel(Email email)
		{
			Email = email;
		}

		public EmailViewModel()
		{
			Email = new Email();
		}

		Email _email;
		public Email Email
		{
			get { return _email; }
			set
			{
				_email = value;
				OnPropertyChanged(() => Email);
				OnPropertyChanged(() => PresenrationStates);
				OnPropertyChanged(() => PresentationZones);
			}
		}

		public string PresenrationStates
		{
			get
			{
				return MailHelper.PresentStates(Email);
			}
		}

		public string PresentationZones
		{
			get
			{
				if (Email.Zones.IsNotNullOrEmpty())
				{
					var delimString = ", ";
					var result = new StringBuilder();

					foreach (var zoneUID in Email.Zones)
					{
						var zone = FiresecManager.Zones.FirstOrDefault(x => x.UID == zoneUID);
						if (zone != null)
						{
							result.Append(zone.No);
							result.Append(delimString);
						}
					}

					return result.ToString().Remove(result.Length - delimString.Length);
				}

				return null;
			}
		}

		public void Update()
		{
			OnPropertyChanged(() => Email);
			OnPropertyChanged(() => PresenrationStates);
			OnPropertyChanged(() => PresentationZones);
		}
	}
}