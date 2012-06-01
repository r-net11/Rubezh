using System;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecService.ViewModels;
using FiresecService.Database;
using FiresecService.Processor;
using FiresecService.DatabaseConverter;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		OperationResult<bool> Authenticate(string login, string password)
		{
			var operationResult = new OperationResult<bool>();

			if (CheckLogin(login, password) == false)
			{
				operationResult.HasError = true;
				operationResult.Error = "Неверный логин или пароль";
				return operationResult;
			}
			if (CheckRemoteAccessPermissions(login) == false)
			{
				operationResult.HasError = true;
				operationResult.Error = "У пользователя " + login + " нет прав на подкючение к удаленному серверу c хоста: " + _userIpAddress;
				return operationResult;
			}
			return operationResult;
		}

		bool CheckRemoteAccessPermissions(string login)
		{
			var endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
			_userIpAddress = endpoint.Address;

			if (CheckHostIps("localhost"))
				return true;

			var remoteAccessPermissions = ConfigurationCash.SecurityConfiguration.Users.FirstOrDefault(x => x.Login == login).RemoreAccess;
			if (remoteAccessPermissions == null)
				return false;

			switch (remoteAccessPermissions.RemoteAccessType)
			{
				case RemoteAccessType.RemoteAccessBanned:
					return false;

				case RemoteAccessType.RemoteAccessAllowed:
					return true;

				case RemoteAccessType.SelectivelyAllowed:
					foreach (var hostNameOrIpAddress in remoteAccessPermissions.HostNameOrAddressList)
					{
						if (CheckHostIps(hostNameOrIpAddress))
							return true;
					}
					break;
			}
			return false;
		}

		bool CheckHostIps(string hostNameOrIpAddress)
		{
			try
			{
				var addressList = Dns.GetHostEntry(hostNameOrIpAddress).AddressList;
				return addressList.Any(ip => ip.ToString() == _userIpAddress);
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове FiresecService.CheckHostIps");
				return false;
			}
		}

		bool CheckLogin(string login, string password)
		{
			var user = ConfigurationCash.SecurityConfiguration.Users.FirstOrDefault(x => x.Login == login);
			if (user == null || !HashHelper.CheckPass(password, user.PasswordHash))
				return false;

			_userLogin = login;
			SetUserFullName();

			return true;
		}

		void SetUserFullName()
		{
			var endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
			string userIp = endpoint.Address;

			var addressList = Dns.GetHostEntry("localhost").AddressList;
			if (addressList.Any(ip => ip.ToString() == userIp))
				userIp = "localhost";

			var user = ConfigurationCash.SecurityConfiguration.Users.FirstOrDefault(x => x.Login == _userLogin);
			_userName = user.Name + " (" + userIp + ")";
		}
	}
}