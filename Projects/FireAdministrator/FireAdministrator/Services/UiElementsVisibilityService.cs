using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Automation;
using FiresecAPI.Models;
using Infrastructure;

namespace FireAdministrator
{
	//public class UiElementsVisibilityService : IUiElementsVisibilityService
	//{
	//	private ILicenseData _licenseData;

	//	#region <Реализация интерфейса IUiElementsVisibilityService>

	//	/// <summary>
	//	/// Видимость элемента "Главное меню \ Видео"
	//	/// </summary>
	//	public bool IsMainMenuVideoElementVisible { get; private set; }

	//	/// <summary>
	//	/// Видимость элемента "Главное меню \ Автоматизация"
	//	/// </summary>
	//	public bool IsMainMenuAutomationElementVisible { get; private set; }

	//	/// <summary>
	//	/// Видимость процедур в Автоматизации
	//	/// </summary>
	//	public List<ProcedureStepType> VisibleProcedureSteps { get; private set; }

	//	/// <summary>
	//	/// Видимость элемента "Верификация" в конфигураторе макетов интерфейса
	//	/// </summary>
	//	public bool IsLayoutModuleVerificationElementVisible { get; private set; }

	//	#endregion </Реализация интерфейса IUiElementsVisibilityService>

	//	public UiElementsVisibilityService()
	//	{
	//		VisibleProcedureSteps = new List<ProcedureStepType>();
	//	}

	//	public void Initialize(ILicenseData licenseData)
	//	{
	//		_licenseData = licenseData;
	//		UpdateElementsVisibility();
	//	}

	//	private void UpdateElementsVisibility()
	//	{
	//		IsMainMenuVideoElementVisible = true;
	//		IsMainMenuAutomationElementVisible = true;
	//		IsLayoutModuleVerificationElementVisible = true;

	//		if (_licenseData.IsEnabledPhotoVerification &&
	//			_licenseData.IsEnabledRVI &&
	//			_licenseData.IsEnabledAutomation)
	//		{
	//			VisibleProcedureSteps = new List<ProcedureStepType>(GetAllProcedureSteps());
	//			return;
	//		}

	//		if (_licenseData.IsEnabledPhotoVerification &&
	//			_licenseData.IsEnabledRVI &&
	//			!_licenseData.IsEnabledAutomation)
	//		{
	//			VisibleProcedureSteps = new List<ProcedureStepType>
	//			{
	//				ProcedureStepType.StartRecord,
	//				ProcedureStepType.Ptz,
	//				ProcedureStepType.RviAlarm,
	//				ProcedureStepType.GenerateGuid,
	//				ProcedureStepType.Pause
	//			};
	//			return;
	//		}

	//		if (_licenseData.IsEnabledPhotoVerification &&
	//			!_licenseData.IsEnabledRVI &&
	//			_licenseData.IsEnabledAutomation)
	//		{
	//			IsMainMenuVideoElementVisible = false;
	//			VisibleProcedureSteps = new List<ProcedureStepType>(GetAllProcedureSteps()
	//				.Except(new List<ProcedureStepType>
	//				{
	//					ProcedureStepType.StartRecord,
	//					ProcedureStepType.Ptz,
	//					ProcedureStepType.RviAlarm
	//				}));

	//			return;
	//		}

	//		if (_licenseData.IsEnabledPhotoVerification &&
	//			!_licenseData.IsEnabledRVI &&
	//			!_licenseData.IsEnabledAutomation)
	//		{
	//			IsMainMenuVideoElementVisible = false;
	//			IsMainMenuAutomationElementVisible = false;
	//			return;
	//		}

	//		if (!_licenseData.IsEnabledPhotoVerification &&
	//			_licenseData.IsEnabledRVI &&
	//			_licenseData.IsEnabledAutomation)
	//		{
	//			IsLayoutModuleVerificationElementVisible = false;
	//			VisibleProcedureSteps = new List<ProcedureStepType>(GetAllProcedureSteps());
	//			return;
	//		}

	//		if (!_licenseData.IsEnabledPhotoVerification &&
	//			_licenseData.IsEnabledRVI &&
	//			!_licenseData.IsEnabledAutomation)
	//		{
	//			IsLayoutModuleVerificationElementVisible = false;
	//			VisibleProcedureSteps = new List<ProcedureStepType>
	//			{
	//				ProcedureStepType.StartRecord,
	//				ProcedureStepType.Ptz,
	//				ProcedureStepType.RviAlarm,
	//				ProcedureStepType.GenerateGuid,
	//				ProcedureStepType.Pause
	//			};
	//			return;
	//		}

	//		if (!_licenseData.IsEnabledPhotoVerification &&
	//			!_licenseData.IsEnabledRVI &&
	//			_licenseData.IsEnabledAutomation)
	//		{
	//			IsLayoutModuleVerificationElementVisible = false;
	//			IsMainMenuVideoElementVisible = false;
	//			VisibleProcedureSteps = new List<ProcedureStepType>(GetAllProcedureSteps()
	//				.Except(new List<ProcedureStepType>
	//				{
	//					ProcedureStepType.StartRecord,
	//					ProcedureStepType.Ptz,
	//					ProcedureStepType.RviAlarm
	//				}));
	//			return;
	//		}

	//		if (!_licenseData.IsEnabledPhotoVerification &&
	//			!_licenseData.IsEnabledRVI &&
	//			!_licenseData.IsEnabledAutomation)
	//		{
	//			IsLayoutModuleVerificationElementVisible = false;
	//			IsMainMenuVideoElementVisible = false;
	//			IsMainMenuAutomationElementVisible = false;
	//			return;
	//		}
	//	}

	//	private IEnumerable<ProcedureStepType> GetAllProcedureSteps()
	//	{
	//		return Enum.GetValues(typeof (ProcedureStepType)).Cast<ProcedureStepType>();
	//	}
	//}
}