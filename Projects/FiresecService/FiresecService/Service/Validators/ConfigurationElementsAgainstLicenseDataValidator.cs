using System;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common.Layouts;
using Infrastructure.Common.Services.Configuration;
using KeyGenerator;

namespace FiresecService.Service.Validators
{
	public sealed class ConfigurationElementsAgainstLicenseDataValidator
	{
		 private static readonly Lazy<ConfigurationElementsAgainstLicenseDataValidator> _instance = new Lazy<ConfigurationElementsAgainstLicenseDataValidator>(
			 () => new ConfigurationElementsAgainstLicenseDataValidator());

		private ConfigurationElementsAgainstLicenseDataValidator()
		{
			_configurationElementsAvailabilityService = new ConfigurationElementsAvailabilityService();
		}

		public static ConfigurationElementsAgainstLicenseDataValidator Instance
		{
			get { return _instance.Value; }
		}

		private IConfigurationElementsAvailabilityService _configurationElementsAvailabilityService;

		public ILicenseManager LicenseManager { private get; set; }

		public bool IsValidated { get; private set; }

		public void Validate()
		{
			if (LicenseManager == null)	return;

			_configurationElementsAvailabilityService.Initialize(
				LicenseManager.CurrentLicense != null
					? new LicenseData
					{
						IsEnabledAutomation = LicenseManager.CurrentLicense.IsEnabledAutomation,
						IsEnabledPhotoVerification = LicenseManager.CurrentLicense.IsEnabledPhotoVerification,
						IsEnabledRVI = LicenseManager.CurrentLicense.IsEnabledRVI,
						IsEnabledURV = LicenseManager.CurrentLicense.IsEnabledURV,
						IsUnlimitedUsers = LicenseManager.CurrentLicense.IsUnlimitedUsers
					}
					: new LicenseData());

			IsValidated = LicenseManager.IsValidExistingKey()
				&& ValidateCameras()
				&& ValidateProcedureSteps()
				&& ValidateLayouts();
		}

		private bool ValidateProcedureSteps()
		{
			var automationConfiguration = ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration;

			// Конфигурация для автоматизации отсутствует. Проверять нечего.
			if (automationConfiguration == null)
				return true;

			// Проверяем каждую процедуру автоматизации на присутствие запрещенных лицензией шагов процедуры
			foreach (var procedure in automationConfiguration.Procedures)
			{
				if (procedure.Steps.Any(procedureStep => _configurationElementsAvailabilityService.AvailableProcedureSteps.All(x => x != procedureStep.ProcedureStepType)))
					return false;
			}

			return true;
		}

		private bool ValidateCameras()
		{
			var systemConfiguration = ConfigurationCashHelper.SystemConfiguration;

			// Системная конфигурация отсутствует. Проверять нечего.
			if (systemConfiguration == null)
				return true;

			// Находим запрещенные лицензией камеры
			if (!_configurationElementsAvailabilityService.IsCamerasAvailable && systemConfiguration.Cameras.Any())
				return false;

			// Камеры разрешены лицензией
			return true;
		}

		private bool ValidateLayouts()
		{
			var layoutsConfiguration = ConfigurationCashHelper.GetLayoutsConfiguration();

			// Конфигурация макетов отсутствует. Проверять нечего.
			if (layoutsConfiguration == null)
				return true;

			// Проверяем элементы каждого макета
			foreach (var layout in layoutsConfiguration.Layouts)
			{
				// Находим запрещенные лицензией элементы "Верификация"
				if (!_configurationElementsAvailabilityService.IsLayoutVerificationElementsAvailable &&
				    layout.GetLayoutPartByType(LayoutPartIdentities.SKDVerification) != null)
					return false;
			}

			// Все элементы макетов валидны
			return true;
		}
	}
}