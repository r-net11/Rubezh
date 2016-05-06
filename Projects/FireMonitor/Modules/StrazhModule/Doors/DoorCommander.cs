using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StrazhAPI.GK;
using StrazhAPI.Models;
using StrazhAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;

namespace StrazhModule.Doors
{
	/// <summary>
	/// Класс реализует общие команды управления точкой доступа
	/// </summary>
	public static class DoorCommander
	{
		/// <summary>
		/// Открыть точку доступа
		/// </summary>
		/// <param name="door">точка доступа, которую необходимо открыть</param>
		public static void Open(SKDDoor door)
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDOpenDoor(door);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		/// <summary>
		/// Можно ли выполнить операцию открытия точки доступа
		/// </summary>
		/// <param name="device">точка доступа, которую необходимо открыть</param>
		/// <returns>true - можно открыть, false - нельзя открыть</returns>
		public static bool CanOpen(SKDDoor door)
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Doors_Control)
				&& door.State.StateClass != XStateClass.On
				&& door.State.StateClass != XStateClass.ConnectionLost
				&& door.State.StateClass != XStateClass.Attention
				&& door.State.AccessState == AccessState.Normal;
		}

		/// <summary>
		/// Закрыть точку доступа
		/// </summary>
		/// <param name="door">точка доступа, которую необходимо закрыть</param>
		public static void Close(SKDDoor door)
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDCloseDoor(door);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		/// <summary>
		/// Можно ли выполнить операцию закрытия точки доступа
		/// </summary>
		/// <param name="door">точка доступа, которую необходимо закрыть</param>
		/// <returns>true - можно закрыть, false - нельзя закрыть</returns>
		public static bool CanClose(SKDDoor door)
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Doors_Control)
				&& door.State.StateClass != XStateClass.Off
				&& door.State.StateClass != XStateClass.ConnectionLost
				&& door.State.StateClass != XStateClass.Attention
				&& door.State.AccessState == AccessState.Normal;
		}

		/// <summary>
		/// Переводит точку доступа в режим НОРМА
		/// </summary>
		/// <param name="door">требуемая точка доступа</param>
		public static void SetAccessStateToNormal(SKDDoor door)
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDDoorAccessStateNormal(door);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		/// <summary>
		/// Проверяет можно ли перевести точку доступа в режим НОРМА
		/// </summary>
		/// <param name="door">требуемая точка доступа</param>
		/// <returns>true - если можно перевести, false - в противном случае</returns>
		public static bool CanSetAccessStateToNormal(SKDDoor door)
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Doors_Control)
				&& door.State.StateClass != XStateClass.ConnectionLost
				&& door.State.StateClass != XStateClass.Attention
				&& door.State.AccessState != AccessState.Normal;
		}

		/// <summary>
		/// Переводит точку доступа в режим ЗАКРЫТО
		/// </summary>
		/// <param name="door">требуемая точка доступа</param>
		public static void SetAccessStateToCloseAlways(SKDDoor door)
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDDoorAccessStateCloseAlways(door);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		/// <summary>
		/// Проверяет можно ли перевести точку доступа в режим ЗАКРЫТО
		/// </summary>
		/// <param name="door">требуемая точка доступа</param>
		/// <returns>true - если можно перевести, false - в противном случае</returns>
		public static bool CanSetAccessStateToCloseAlways(SKDDoor door)
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Doors_Control)
				&& door.State.StateClass != XStateClass.ConnectionLost
				&& door.State.StateClass != XStateClass.Attention
				&& door.State.AccessState != AccessState.CloseAlways;
		}

		/// <summary>
		/// Переводит точку доступа в режим ОТКРЫТО
		/// </summary>
		/// <param name="door">требуемая точка доступа</param>
		public static void SetAccessStateToOpenAlways(SKDDoor door)
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDDoorAccessStateOpenAlways(door);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		/// <summary>
		/// Проверяет можно ли перевести точку доступа в режим ОТКРЫТО
		/// </summary>
		/// <param name="door">требуемая точка</param>
		/// <returns>true - если можно перевести, false - в противном случае</returns>
		public static bool CanSetAccessStateToOpenAlways(SKDDoor door)
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Doors_Control)
				&& door.State.StateClass != XStateClass.ConnectionLost
				&& door.State.StateClass != XStateClass.Attention
				&& door.State.AccessState != AccessState.OpenAlways;
		}

		/// <summary>
		/// Сбрасывает статус ВЗЛОМ для точки доступа
		/// </summary>
		/// <param name="door">требуемая точка доступа</param>
		public static void ClearPromptWarning(SKDDoor door)
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDClearDoorPromptWarning(door);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		/// <summary>
		/// Проверяет можно ли сбросить статус ВЗЛОМ для точки доступа
		/// </summary>
		/// <param name="door">требуемая точка доступа</param>
		/// <returns>true - если можно сбросить, false - в противном случае</returns>
		public static bool CanClearPromptWarning(SKDDoor door)
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Doors_Control)
				&& door.State.StateClass != XStateClass.ConnectionLost;
		}
	}
}
