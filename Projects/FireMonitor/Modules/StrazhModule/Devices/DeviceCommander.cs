using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using StrazhAPI.GK;
using StrazhAPI.Models;
using StrazhAPI.SKD;

namespace StrazhModule.Devices
{
	/// <summary>
	/// Класс реализует общие команды управления устройством для таких классов
	/// как DeviceCommandsViewModel и DeviceViewModel
	/// </summary>
	public static class DeviceCommander
	{
		/// <summary>
		/// Открыть замок
		/// </summary>
		/// <param name="device">замок, который необходимо открыть</param>
		public static void Open(SKDDevice device)
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDOpenDevice(device);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		/// <summary>
		/// Можно ли выполнить операцию открытия замка
		/// </summary>
		/// <param name="device">замок, который необходимо открыть</param>
		/// <returns>true - можно открыть, false - нельзя открыть</returns>
		public static bool CanOpen(SKDDevice device)
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Devices_Control)
				&& device.State.CanControl
				&& device.State.StateClass != XStateClass.On
				&& device.State.AccessState == AccessState.Normal;
		}

		/// <summary>
		/// Закрыть замок
		/// </summary>
		/// <param name="device">замок, который необходимо закрыть</param>
		public static void Close(SKDDevice device)
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDCloseDevice(device);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		/// <summary>
		/// Можно ли выполнить операцию закрытия замка
		/// </summary>
		/// <param name="device">замок, который необходимо закрыть</param>
		/// <returns>true - можно закрыть, false - нельзя закрыть</returns>
		public static bool CanClose(SKDDevice device)
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Devices_Control)
				&& device.State.CanControl
				&& device.State.StateClass != XStateClass.Off
				&& device.State.AccessState == AccessState.Normal;
		}

		/// <summary>
		/// Переводит замок в режим НОРМА
		/// </summary>
		/// <param name="device">требуемый замок</param>
		public static void SetAccessStateToNormal(SKDDevice device)
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDDeviceAccessStateNormal(device);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		/// <summary>
		/// Проверяет можно ли перевести замок в режим НОРМА
		/// </summary>
		/// <param name="device">требуемый замок</param>
		/// <returns>true - если можно перевести, false - в противном случае</returns>
		public static bool CanSetAccessStateToNormal(SKDDevice device)
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Devices_Control)
				&& device.State.CanControl
				&& device.State.AccessState != AccessState.Normal;
		}

		/// <summary>
		/// Переводит замок в режим ЗАКРЫТО
		/// </summary>
		/// <param name="device">требуемый замок</param>
		public static void SetAccessStateToCloseAlways(SKDDevice device)
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDDeviceAccessStateCloseAlways(device);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		/// <summary>
		/// Проверяет можно ли перевести замок в режим ЗАКРЫТО
		/// </summary>
		/// <param name="device">требуемый замок</param>
		/// <returns>true - если можно перевести, false - в противном случае</returns>
		public static bool CanSetAccessStateToCloseAlways(SKDDevice device)
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Devices_Control)
				&& device.State.CanControl
				&& device.State.AccessState != AccessState.CloseAlways;
		}

		/// <summary>
		/// Переводит замок в режим ОТКРЫТО
		/// </summary>
		/// <param name="device">требуемый замок</param>
		public static void SetAccessStateToOpenAlways(SKDDevice device)
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDDeviceAccessStateOpenAlways(device);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		/// <summary>
		/// Проверяет можно ли перевести замок в режим ОТКРЫТО
		/// </summary>
		/// <param name="device">требуемый замок</param>
		/// <returns>true - если можно перевести, false - в противном случае</returns>
		public static bool CanSetAccessStateToOpenAlways(SKDDevice device)
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Devices_Control)
				&& device.State.CanControl
				&& device.State.AccessState != AccessState.OpenAlways;
		}

		/// <summary>
		/// Сбрасывает статус ВЗЛОМ для замака
		/// </summary>
		/// <param name="device">требуемый замок</param>
		public static void ClearPromptWarning(SKDDevice device)
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDClearDevicePromptWarning(device);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		/// <summary>
		/// Проверяет можно ли сбросить статус ВЗЛОМ для замка
		/// </summary>
		/// <param name="door">требуемый замок</param>
		/// <returns>true - если можно сбросить, false - в противном случае</returns>
		public static bool CanClearPromptWarning(SKDDevice device)
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Devices_Control)
				&& device.State.StateClass != XStateClass.ConnectionLost;
		}
	}
}