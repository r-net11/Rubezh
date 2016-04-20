using RubezhClient;
using Infrastructure.Common.Windows.Validation;
using Infrastructure.Client.Layout;
using RubezhAPI.Models.Layouts;
using System.Linq;
using System.Collections.Generic;
using System;
using RubezhAPI.License;

namespace LayoutModule.Validation
{
	public partial class Validator
	{
		void ValidateLicense()
		{
			foreach (var layout in ClientManager.LayoutsConfiguration.Layouts)
			{
				var layoutLicenses = GetLayoutLicenses(layout);
				if (layoutLicenses.Any())
					Errors.Add(new LayoutValidationError(layout, "Макет содержит элементы, требующие наличия лицензии модуля(ей): " + String.Join(", ", layoutLicenses), ValidationErrorLevel.Warning));
			}
		}

		IEnumerable<string> GetLayoutLicenses(Layout layout)
		{
			if (!LicenseManager.CurrentLicenseInfo.HasFirefighting && layout.Parts.Any(x => 
				x.DescriptionUID == LayoutPartIdentities.PumpStations || 
				x.DescriptionUID == LayoutPartIdentities.MPTs))
				yield return "\"GLOBAL Пожаротушение\"";

			if (!LicenseManager.CurrentLicenseInfo.HasGuard && layout.Parts.Any(x => 
				x.DescriptionUID == LayoutPartIdentities.GuardZones))
				yield return "\"GLOBAL Охрана\"";

			if (!LicenseManager.CurrentLicenseInfo.HasSKD && layout.Parts.Any(x => 
				x.DescriptionUID == LayoutPartIdentities.Doors ||
				x.DescriptionUID == LayoutPartIdentities.GKSKDZones ||
				x.DescriptionUID == LayoutPartIdentities.SKDVerification ||
				x.DescriptionUID == LayoutPartIdentities.SKDHR ||
				x.DescriptionUID == LayoutPartIdentities.SKDTimeTracking))
				yield return "\"GLOBAL Доступ\"";

			if (!LicenseManager.CurrentLicenseInfo.HasVideo && layout.Parts.Any(x =>
				x.DescriptionUID == LayoutPartIdentities.CamerasList ||
				x.DescriptionUID == LayoutPartIdentities.CameraVideo ||
				x.DescriptionUID == LayoutPartIdentities.MultiCamera))
				yield return "\"GLOBAL Видео\"";
		}
	}
}