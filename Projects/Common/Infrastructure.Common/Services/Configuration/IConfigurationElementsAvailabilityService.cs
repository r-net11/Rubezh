using System.Collections.Generic;
using FiresecAPI.Automation;
using FiresecAPI.Models;

namespace Infrastructure.Common.Services.Configuration
{
	/// <summary>
	/// Интерфейс, описывающий доступность элементов в текущей конфигурации согласно лицензионным ограничениям
	/// </summary>
	public interface IConfigurationElementsAvailabilityService
	{
		/// <summary>
		/// Определяет доступность камер в конфигурации
		/// </summary>
		bool IsCamerasAvailable { get; }

		/// <summary>
		/// Определяет доступность элемента "Верификация" в макетах
		/// </summary>
		bool IsLayoutVerificationElementsAvailable { get; }

		/// <summary>
		/// Доступные процедуры в Автоматизации
		/// </summary>
		List<ProcedureStepType> AvailableProcedureSteps { get; }

		/// <summary>
		/// Инициализация сервиса на основе данных лицензии
		/// </summary>
		/// <param name="licenseData">Данные лицензии</param>
		void Initialize(ILicenseData licenseData);
	}
}