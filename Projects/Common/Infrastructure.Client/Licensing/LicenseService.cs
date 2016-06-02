using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Enums;
using FiresecClient;

namespace Infrastructure.Client.Licensing
{
	//public sealed class LicenseService : ILicenseService
	//{
	//	public bool CheckLicenseExising()
	//	{
	//		var result = FiresecManager.FiresecService.CheckLicenseExising();

	//		if (result == null || result.HasError) return false;

	//		return result.Result;
	//	}

	//	public bool CanConnect()
	//	{
	//		var result = FiresecManager.FiresecService.CanConnect();

	//		if (result == null || result.HasError) return false;

	//		return result.Result;
	//	}

	//	public bool CanLoadModule(ModuleType type)
	//	{
	//		var result = FiresecManager.FiresecService.CanLoadModule(type);

	//		if (result == null || result.HasError) return false;

	//		return result.Result;
	//	}
	//}
}
