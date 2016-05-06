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

namespace StrazhModule.Zones
{
	/// <summary>
	/// Класс реализует общие команды управления зоной
	/// </summary>
	public static class ZoneCommander
	{
		/// <summary>
		/// Открыть зону
		/// </summary>
		/// <param name="zone">зона, которую необходимо открыть</param>
		public static void Open(SKDZone zone)
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDOpenZone(zone);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		/// <summary>
		/// Можно ли выполнить операцию открытия точки доступа
		/// </summary>
		/// <param name="device">зона, которую необходимо открыть</param>
		/// <returns>true - можно открыть, false - нельзя открыть</returns>
		public static bool CanOpen(SKDZone zone)
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Zones_Control)
				&& zone.State.StateClass != XStateClass.ConnectionLost;
		}

		/// <summary>
		/// Закрыть зону
		/// </summary>
		/// <param name="zone">зона, которую необходимо закрыть</param>
		public static void Close(SKDZone zone)
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDCloseZone(zone);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		/// <summary>
		/// Можно ли выполнить операцию закрытия точки доступа
		/// </summary>
		/// <param name="zone">зона, которую необходимо закрыть</param>
		/// <returns>true - можно закрыть, false - нельзя закрыть</returns>
		public static bool CanClose(SKDZone zone)
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Zones_Control)
				&& zone.State.StateClass != XStateClass.ConnectionLost;
		}

		/// <summary>
		/// Переводит зону в режим НОРМА
		/// </summary>
		/// <param name="zone">требуемая зона</param>
		public static void SetAccessStateToNormal(SKDZone zone)
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDZoneAccessStateNormal(zone);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		/// <summary>
		/// Проверяет можно ли перевести зону в режим НОРМА
		/// </summary>
		/// <param name="zone">требуемая зона</param>
		/// <returns>true - если можно перевести, false - в противном случае</returns>
		public static bool CanSetAccessStateToNormal(SKDZone zone)
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Zones_Control)
				&& zone.State.StateClass != XStateClass.ConnectionLost;
		}

		/// <summary>
		/// Переводит зону в режим ЗАКРЫТО
		/// </summary>
		/// <param name="zone">требуемая зона</param>
		public static void SetAccessStateToCloseAlways(SKDZone zone)
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDZoneAccessStateCloseAlways(zone);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		/// <summary>
		/// Проверяет можно ли перевести зону в режим ЗАКРЫТО
		/// </summary>
		/// <param name="zone">требуемая зона</param>
		/// <returns>true - если можно перевести, false - в противном случае</returns>
		public static bool CanSetAccessStateToCloseAlways(SKDZone zone)
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Zones_Control)
				&& zone.State.StateClass != XStateClass.ConnectionLost;
		}

		/// <summary>
		/// Переводит зону в режим ОТКРЫТО
		/// </summary>
		/// <param name="zone">требуемая зона</param>
		public static void SetAccessStateToOpenAlways(SKDZone zone)
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDZoneAccessStateOpenAlways(zone);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		/// <summary>
		/// Проверяет можно ли перевести зону в режим ОТКРЫТО
		/// </summary>
		/// <param name="zone">требуемая зона</param>
		/// <returns>true - если можно перевести, false - в противном случае</returns>
		public static bool CanSetAccessStateToOpenAlways(SKDZone zone)
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Zones_Control)
				&& zone.State.StateClass != XStateClass.ConnectionLost;
		}

		/// <summary>
		/// Сбрасывает статус ВЗЛОМ для зоны
		/// </summary>
		/// <param name="zone">требуемая зона</param>
		public static void ClearPromptWarning(SKDZone zone)
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDClearZonePromptWarning(zone);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		/// <summary>
		/// Проверяет можно ли сбросить статус ВЗЛОМ для зоны
		/// </summary>
		/// <param name="door">требуемая зона</param>
		/// <returns>true - если можно сбросить, false - в противном случае</returns>
		public static bool CanClearPromptWarning(SKDZone zone)
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Zones_Control)
				&& zone.State.StateClass != XStateClass.ConnectionLost;
		}
	}
}
