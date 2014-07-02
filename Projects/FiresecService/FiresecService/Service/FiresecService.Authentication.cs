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
		OperationResult<bool> Authenticate(ClientCredentials clientCredentials)
		{
			var operationResult = new OperationResult<bool>();

			if (CheckLogin(clientCredentials) == false)
			{
				operationResult.HasError = true;
				operationResult.Error = "Неверный логин или пароль";
				return operationResult;
			}
			if (CheckRemoteAccessPermissions(clientCredentials) == false)
			{
				operationResult.HasError = true;
				operationResult.Error = "У пользователя " + clientCredentials.UserName + " нет прав на подкючение к удаленному серверу c хоста: " + clientCredentials.ClientIpAddressAndPort;
				return operationResult;
			}
			return operationResult;
		}

		bool CheckRemoteAccessPermissions(ClientCredentials clientCredentials)
		{
			if (CheckHostIps(clientCredentials, "localhost"))
				return true;
			if (CheckHostIps(clientCredentials, "127.0.0.1"))
				return true;

			var remoteAccessPermissions = ConfigurationCashHelper.SecurityConfiguration.Users.FirstOrDefault(x => x.Login == clientCredentials.UserName).RemoreAccess;
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
						if (CheckHostIps(clientCredentials, hostNameOrIpAddress))
							return true;
					}
					break;
			}
			return false;
		}

		bool CheckHostIps(ClientCredentials clientCredentials, string hostNameOrIpAddress)
		{
			try
			{
				var addressList = Dns.GetHostEntry(hostNameOrIpAddress).AddressList;
				return addressList.Any(ip => ip.ToString() == clientCredentials.ClientIpAddress);
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове FiresecService.CheckHostIps");
				return false;
			}
		}

		bool CheckLogin(ClientCredentials clientCredentials)
		{
			var user = ConfigurationCashHelper.SecurityConfiguration.Users.FirstOrDefault(x => x.Login == clientCredentials.UserName);
			{
				if (user == null)
				{
					return false;
				}
				if (!HashHelper.CheckPass(clientCredentials.Password, user.PasswordHash))
				{
					return false;
				}
			}

			SetUserFullName(clientCredentials);
			return true;
		}

		void SetUserFullName(ClientCredentials clientCredentials)
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

			var user = ConfigurationCashHelper.SecurityConfiguration.Users.FirstOrDefault(x => x.Login == clientCredentials.UserName);
			clientCredentials.FriendlyUserName = user.Name;// +" (" + userIp + ")";
		}
	}
}