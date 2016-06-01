using System.Collections.Generic;

namespace StrazhAPI.SKD
{
	public static class CommonChinaControllerHelper
	{
		public static List<SKDDriverProperty> CreateProperties()
		{
			var properties = new List<SKDDriverProperty>();

			var addressProperty = new SKDDriverProperty
			{
                Name = Resources.Language.SKD.SKDManager.CommonChinaControllerHelper.AddressProperty_Name,
                Caption = Resources.Language.SKD.SKDManager.CommonChinaControllerHelper.AddressProperty_Caption,
				DriverPropertyType = SKDDriverType.StringType,
                StringDefault = Resources.Language.SKD.SKDManager.CommonChinaControllerHelper.AddressProperty_StringDefault
			};
			properties.Add(addressProperty);

			var portProperty = new SKDDriverProperty
			{
                Name = Resources.Language.SKD.SKDManager.CommonChinaControllerHelper.PortProperty_Name,
                Caption = Resources.Language.SKD.SKDManager.CommonChinaControllerHelper.PortProperty_Caption,
				DriverPropertyType = SKDDriverType.IntType,
				Default = 37777
			};
			properties.Add(portProperty);

			var loginProperty = new SKDDriverProperty
			{
                Name = Resources.Language.SKD.SKDManager.CommonChinaControllerHelper.LoginProperty_Name,
                Caption = Resources.Language.SKD.SKDManager.CommonChinaControllerHelper.LoginProperty_Caption,
				DriverPropertyType = SKDDriverType.StringType,
                StringDefault = Resources.Language.SKD.SKDManager.CommonChinaControllerHelper.LoginProperty_StringDefault
			};
			properties.Add(loginProperty);

			var passwordProperty = new SKDDriverProperty
			{
                Name = Resources.Language.SKD.SKDManager.CommonChinaControllerHelper.PasswordProperty_Name,
                Caption = Resources.Language.SKD.SKDManager.CommonChinaControllerHelper.PasswordProperty_Caption,
				DriverPropertyType = SKDDriverType.StringType,
                StringDefault = Resources.Language.SKD.SKDManager.CommonChinaControllerHelper.PasswordProperty_StringDefault
			};
			properties.Add(passwordProperty);

			var maskProperty = new SKDDriverProperty
			{
                Name = Resources.Language.SKD.SKDManager.CommonChinaControllerHelper.MaskProperty_Name,
                Caption = Resources.Language.SKD.SKDManager.CommonChinaControllerHelper.MaskProperty_Caption,
				DriverPropertyType = SKDDriverType.StringType,
                StringDefault = Resources.Language.SKD.SKDManager.CommonChinaControllerHelper.MaskProperty_StringDefault
			};
			properties.Add(maskProperty);

			var gatewayProperty = new SKDDriverProperty
			{
                Name = Resources.Language.SKD.SKDManager.CommonChinaControllerHelper.GatewayProperty_Name,
                Caption = Resources.Language.SKD.SKDManager.CommonChinaControllerHelper.GatewayProperty_Caption,
				DriverPropertyType = SKDDriverType.StringType,
                StringDefault = Resources.Language.SKD.SKDManager.CommonChinaControllerHelper.GatewayProperty_StringDefault
			};
			properties.Add(gatewayProperty);

			return properties;
		}
	}
}