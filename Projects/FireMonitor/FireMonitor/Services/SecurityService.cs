using StrazhAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Services;

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