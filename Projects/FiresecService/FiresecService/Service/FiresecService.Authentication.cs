using System;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Common;
using FiresecAPI;
using FiresecAPI.Models;

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
				operationResult.Error = "У пользователя " + login + " нет прав на подкючение к удаленному серверу c хоста: " + ClientIpAddressAndPort;
				return operationResult;
			}
			return operationResult;
		}

		bool CheckRemoteAccessPermissions(string login)
		{
			if (CheckHostIps("localhost"))
				return true;
			if (CheckHostIps("127.0.0.1"))
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
				return addressList.Any(ip => ip.ToString() == ClientIpAddress);
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
			{
				if (user == null)
				{
					Logger.Error("FiresecService.CheckLogin user = null");
					return false;
				}
				if (!HashHelper.CheckPass(password, user.PasswordHash))
				{
					Logger.Error("FiresecService.CheckLogin HashHelper.CheckPass Password=" + password + " User.PasswordHash=" + user.PasswordHash);
					return false;
				}
			}

			SetUserFullName(login);
			return true;
		}

		void SetUserFullName(string login)
		{
			string userIp = "127.0.0.1";
			try
			{
				if (OperationContext.Current.IncomingMessageProperties.Keys.Contains(RemoteEndpointMessageProperty.Name))
				{
					var endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
					userIp = endpoint.Address;
				}
			}
			catch { }

			var addressList = Dns.GetHostEntry("localhost").AddressList;
			if (addressList.Any(ip => ip.ToString() == userIp))
				userIp = "localhost";

			var user = ConfigurationCash.SecurityConfiguration.Users.FirstOrDefault(x => x.Login == login);
			ClientCredentials.UserName = user.Name + " (" + userIp + ")";
		}
	}
}