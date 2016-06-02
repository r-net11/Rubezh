using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Automation;
using FiresecAPI.Models;

namespace Infrastructure.Common.Services.Configuration
{
	/// <summary>
	/// Класс, описывающий доступность элементов в текущей конфигурации согласно лицензионным ограничениям
	/// </summary>
	//public class ConfigurationElementsAvailabilityService : IConfigurationElementsAvailabilityService
	//{
	//	private ILicenseData _licenseData;

	//	#region <Реализация интерфейса IConfigurationElementsAvailabilityService>

	//	/// <summary>
	//	/// Определяет доступность камер в конфигурации
	//	/// </summary>
	//	public bool IsCamerasAvailable { get; private set; }

	//	/// <summary>
	//	/// Определяет доступность элемента "Верификация" в макетах
	//	/// </summary>
	//	public bool IsLayoutVerificationElementsAvailable { get; private set; }

	//	/// <summary>
	//	/// Доступные процедуры в Автоматизации
	//	/// </summary>
	//	public List<ProcedureStepType> AvailableProcedureSteps { get; private set; }

	//	/// <summary>
	//	/// Инициализация сервиса на основе данных лицензии
	//	/// </summary>
	//	/// <param name="licenseData">Данные лицензии</param>
	//	public void Initialize(ILicenseData licenseData)
	//	{
	//		_licenseData = licenseData;
	//		UpdateElementsAvailability();
	//	}

	//	/// <summary>
	//	/// Расчитывает доступность элементов в конфигарации на основе данных лицензии
	//	/// </summary>
	//	private void UpdateElementsAvailability()
	//	{
	//		// По умолчанию элементы разрешены
	//		IsCamerasAvailable = true;
	//		IsLayoutVerificationElementsAvailable = true;

	//		// Фотоверификация      - да
	//		// Интеграция видео RVi - да
	//		// Автоматизация        - да
	//		if (_licenseData.IsEnabledPhotoVerification &&
	//			_licenseData.IsEnabledRVI &&
	//			_licenseData.IsEnabledAutomation)
	//		{
	//			// Все разрешено
	//			AvailableProcedureSteps = Enum.GetValues(typeof (ProcedureStepType)).Cast<ProcedureStepType>().ToList();
	//			return;
	//		}

	//		// Фотоверификация      - да
	//		// Интеграция видео RVi - да
	//		// Автоматизация        - нет
	//		if (_licenseData.IsEnabledPhotoVerification &&
	//			_licenseData.IsEnabledRVI &&
	//			!_licenseData.IsEnabledAutomation)
	//		{
	//			// Разрешены только функции интеграции с видеонаблюдением, генерация идентификатора и пауза
	//			AvailableProcedureSteps = new List<ProcedureStepType>
	//			{
	//				ProcedureStepType.StartRecord,
	//				ProcedureStepType.Ptz,
	//				ProcedureStepType.RviAlarm,
	//				ProcedureStepType.GenerateGuid,
	//				ProcedureStepType.Pause
	//			};
	//			return;
	//		}

	//		// Фотоверификация      - да
	//		// Интеграция видео RVi - нет
	//		// Автоматизация        - да
	//		if (_licenseData.IsEnabledPhotoVerification &&
	//			!_licenseData.IsEnabledRVI &&
	//			_licenseData.IsEnabledAutomation)
	//		{
	//			// Видеокамеры запрещены
	//			IsCamerasAvailable = false;
	//			// Запрещены функции интеграции с видеонаблюдением
	//			AvailableProcedureSteps = Enum.GetValues(typeof (ProcedureStepType)).Cast<ProcedureStepType>().Where(x =>
	//				x != ProcedureStepType.StartRecord &&
	//				x != ProcedureStepType.Ptz &&
	//				x != ProcedureStepType.RviAlarm).ToList();
	//			return;
	//		}

	//		// Фотоверификация      - да
	//		// Интеграция видео RVi - нет
	//		// Автоматизация        - нет
	//		if (_licenseData.IsEnabledPhotoVerification &&
	//			!_licenseData.IsEnabledRVI &&
	//			!_licenseData.IsEnabledAutomation)
	//		{
	//			// Видеокамеры запрещены
	//			IsCamerasAvailable = false;
	//			// Все функции автоматизации запрещены
	//			AvailableProcedureSteps.Clear();
	//			return;
	//		}

	//		// Фотоверификация      - нет
	//		// Интеграция видео RVi - да
	//		// Автоматизация        - да
	//		if (!_licenseData.IsEnabledPhotoVerification &&
	//			_licenseData.IsEnabledRVI &&
	//			_licenseData.IsEnabledAutomation)
	//		{
	//			// Элемент "Верификация" в макетах запрещен
	//			IsLayoutVerificationElementsAvailable = false;
	//			// Разрешены все функции автоматизации
	//			AvailableProcedureSteps = Enum.GetValues(typeof (ProcedureStepType)).Cast<ProcedureStepType>().ToList();
	//			return;
	//		}

	//		// Фотоверификация      - нет
	//		// Интеграция видео RVi - да
	//		// Автоматизация        - нет
	//		if (!_licenseData.IsEnabledPhotoVerification &&
	//			_licenseData.IsEnabledRVI &&
	//			!_licenseData.IsEnabledAutomation)
	//		{
	//			// Элемент "Верификация" в макетах запрещен
	//			IsLayoutVerificationElementsAvailable = false;
	//			// Разрешены только функции интеграции с видеонаблюдением
	//			AvailableProcedureSteps =  new List<ProcedureStepType>
	//			{
	//				ProcedureStepType.StartRecord,
	//				ProcedureStepType.Ptz,
	//				ProcedureStepType.RviAlarm,
	//				ProcedureStepType.GenerateGuid,
	//				ProcedureStepType.Pause
	//			};
	//			return;
	//		}

	//		// Фотоверификация      - нет
	//		// Интеграция видео RVi - нет
	//		// Автоматизация        - да
	//		if (!_licenseData.IsEnabledPhotoVerification &&
	//			!_licenseData.IsEnabledRVI &&
	//			_licenseData.IsEnabledAutomation)
	//		{
	//			// Элемент "Верификация" в макетах запрещен
	//			IsLayoutVerificationElementsAvailable = false;
	//			// Камеры запрещены
	//			IsCamerasAvailable = false;
	//			// Запрещены функции интеграции с видеонаблюдением
	//			AvailableProcedureSteps = Enum.GetValues(typeof (ProcedureStepType)).Cast<ProcedureStepType>().Where(x =>
	//				x != ProcedureStepType.StartRecord &&
	//				x != ProcedureStepType.Ptz &&
	//				x != ProcedureStepType.RviAlarm).ToList();
	//			return;
	//		}

	//		// Фотоверификация      - нет
	//		// Интеграция видео RVi - нет
	//		// Автоматизация        - нет
	//		if (!_licenseData.IsEnabledPhotoVerification &&
	//			!_licenseData.IsEnabledRVI &&
	//			!_licenseData.IsEnabledAutomation)
	//		{
	//			// Элемент "Верификация" в макетах запрещен
	//			IsLayoutVerificationElementsAvailable = false;
	//			// Камеры запрещены
	//			IsCamerasAvailable = false;
	//			// Все функции автоматизации запрещены
	//			AvailableProcedureSteps.Clear();
	//			return;
	//		}
	//	}

	//	#endregion </Реализация интерфейса IConfigurationElementsAvailabilityService>

	//	public ConfigurationElementsAvailabilityService()
	//	{
	//		AvailableProcedureSteps = new List<ProcedureStepType>();
	//	}
	//}
}