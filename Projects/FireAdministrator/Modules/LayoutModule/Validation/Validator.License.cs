using FiresecClient;
using Infrastructure.Common.Validation;
using Infrastructure.Common;
using Infrastructure.Client.Layout;
using FiresecAPI.Models.Layouts;
using System.Text;
using System.Linq;

namespace LayoutModule.Validation
{
	public partial class Validator
	{
		void ValidateLicense()
		{
			foreach (var layout in FiresecManager.LayoutsConfiguration.Layouts)
			{
				string layoutLicenses = GetLayoutLicenses(layout);
				if (!string.IsNullOrEmpty(layoutLicenses))
					Errors.Add(new LayoutValidationError(layout, "Макет содержит элементы, требующие наличия лицензии модуля(ей): " + layoutLicenses, ValidationErrorLevel.CannotWrite));
			}
		}

		string GetLayoutLicenses(Layout layout)
		{
			string layoutLicenses = string.Empty;

			if (!LicenseHelper.Fire && layout.Parts.Any(x => 
				x.DescriptionUID == LayoutPartIdentities.PumpStations || 
				x.DescriptionUID == LayoutPartIdentities.MPTs))
				layoutLicenses += "\"GLOBAL Пожаротушение\", ";
			
			if (!LicenseHelper.Security && layout.Parts.Any(x => 
				x.DescriptionUID == LayoutPartIdentities.GuardZones))
				layoutLicenses += "\"GLOBAL Охрана\", ";

			if (!LicenseHelper.Access && layout.Parts.Any(x => 
				x.DescriptionUID == LayoutPartIdentities.Doors ||
				x.DescriptionUID == LayoutPartIdentities.GKSKDZones ||
				x.DescriptionUID == LayoutPartIdentities.SKDVerification ||
				x.DescriptionUID == LayoutPartIdentities.SKDDayIntervals ||
				x.DescriptionUID == LayoutPartIdentities.SKDHolidays ||
				x.DescriptionUID == LayoutPartIdentities.SKDHR ||
				x.DescriptionUID == LayoutPartIdentities.SKDSchedules ||
				x.DescriptionUID == LayoutPartIdentities.SKDScheduleSchemes ||
				x.DescriptionUID == LayoutPartIdentities.SKDTimeTracking))
				layoutLicenses += "\"GLOBAL Доступ\", ";

			if (!LicenseHelper.Video && layout.Parts.Any(x =>
				x.DescriptionUID == LayoutPartIdentities.CamerasList ||
				x.DescriptionUID == LayoutPartIdentities.CameraVideo ||
				x.DescriptionUID == LayoutPartIdentities.MultiCamera))
				layoutLicenses += "\"GLOBAL Видео\", ";

			return layoutLicenses.TrimEnd(',', ' ');
		}
	}
}