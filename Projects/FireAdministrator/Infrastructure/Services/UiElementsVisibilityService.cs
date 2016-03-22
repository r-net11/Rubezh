using FiresecAPI.Models;

namespace Infrastructure
{
	public class UiElementsVisibilityService : IUiElementsVisibilityService
	{
		private ILicenseData _licenseData;

		public bool IsMainMenuVideoElementVisible { get; private set; }

		public bool IsMainMenuAutomationElementVisible { get; private set; }

		public void Initialize(ILicenseData licenseData)
		{
			_licenseData = licenseData;
			UpdateElementsVisibility();
		}

		private void UpdateElementsVisibility()
		{
			//throw new System.NotImplementedException();
		}
	}
}