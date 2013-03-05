using Infrastructure;
using Infrastructure.Common;

namespace FireMonitor
{
	public class SecurityService : ISecurityService
	{
		public bool Validate()
		{
			if (GlobalSettingsHelper.GlobalSettings.DoNotShowConfirmatinoOnIgnore)
				return true;
			return ServiceFactory.LoginService.ExecuteValidate();
		}
	}
}