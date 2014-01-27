using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using FiresecClient;
using FiresecAPI.Models;

namespace FireMonitor
{
	public class SecurityService : ISecurityService
	{
		public bool Validate()
		{
			if (FiresecManager.CheckPermission(PermissionType.Oper_MayNotConfirmCommands))
				return true;
			return ServiceFactory.LoginService.ExecuteValidate();
		}
	}
}