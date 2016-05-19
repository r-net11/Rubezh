using RubezhService.Service;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;
using RubezhAPI.Models;
using System;

namespace RubezhService.Models
{
	public class ClientViewModel : BaseViewModel
	{
		public ClientCredentials ClientCredentials { get; private set; }
		public string ClientType { get; private set; }
		public Guid UID { get; private set; }
		public string IpAddress { get; private set; }

		public ClientViewModel(ClientCredentials clientCredentials)
		{
			ClientCredentials = clientCredentials;
			ClientType = clientCredentials.ClientType.ToDescription();
			UID = ClientCredentials.ClientUID;
			FriendlyUserName = clientCredentials.FriendlyUserName;
			IpAddress = clientCredentials.ClientIpAddress;
			if (IpAddress.StartsWith("127.0.0.1"))
				IpAddress = "localhost";
			RemoveCommand = new RelayCommand(OnRemove);
		}

		string _friendlyUserName;
		public string FriendlyUserName
		{
			get { return _friendlyUserName; }
			set
			{
				_friendlyUserName = value;
				OnPropertyChanged("FriendlyUserName");
			}
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			if (MessageBoxService.ShowQuestion(string.Format("Вы действительно хотите отключить клиента <{0} / {1} / {2}> от сервера?", ClientType, IpAddress, FriendlyUserName)))
				ClientsManager.Remove(UID);
		}
	}
}