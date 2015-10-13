using RubezhAPI.Models;
using RubezhClient;
using Infrastructure;
using Infrastructure.Common.Services;

namespace FireMonitor
{
	public class SecurityService : ISecurityService
	{
		public bool Validate()
		{
			if (ClientManager.CheckPermission(PermissionType.Oper_MayNotConfirmCommands))
				return true;
			return ServiceFactory.LoginService.ExecuteValidate();
		}
	}
}