using System;
using System.Linq;
using Infrastructure.Common.Layouts;
using Infrastructure.Common.Services.Configuration;

namespace FiresecService.Service.Validators
{
	public sealed class ConfigurationElementsAgainstLicenseDataValidator
	{
		 private static readonly Lazy<ConfigurationElementsAgainstLicenseDataValidator> _instance = new Lazy<ConfigurationElementsAgainstLicenseDataValidator>(
			 () => new ConfigurationElementsAgainstLicenseDataValidator());

		private ConfigurationElementsAgainstLicenseDataValidator()
		{
		}

		public static ConfigurationElementsAgainstLicenseDataValidator Instance
		{
			get { return _instance.Value; }
		}

		public IConfigurationElementsAvailabilityService ConfigurationElementsAvailabilityService { private get; set; }

		public bool IsValidated { get; private set; }

		public void Validate()
		{
			IsValidated = 
				ValidateCameras() &&
				ValidateProcedureSteps() &&
				ValidateLayouts();
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
				if (procedure.Steps.Any(procedureStep => ConfigurationElementsAvailabilityService.AvailableProcedureSteps.All(x => x != procedureStep.ProcedureStepType)))
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
			if (!ConfigurationElementsAvailabilityService.IsCamerasAvailable && systemConfiguration.Cameras.Any())
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
				if (!ConfigurationElementsAvailabilityService.IsLayoutVerificationElementsAvailable &&
				    layout.GetLayoutPartByType(LayoutPartIdentities.SKDVerification) != null)
					return false;
			}

			// Все элементы макетов валидны
			return true;
		}
	}
}