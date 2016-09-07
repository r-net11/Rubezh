using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.XtraReports.Design;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;

namespace ReportSystem.UI.Services
{
	/// <summary>
	/// Сервис, позволяющий кастомизировать меню, которое вызывается
	/// с помощью пракого клика мыши в контроле дизайнера пропусков.
	/// </summary>
	public class CustomMenuCreationService : IMenuCreationService
	{
		private readonly XRDesignPanel _controller;

		private XtraReport ActiveReport
		{
			get { return _controller != null ? _controller.Report : null; }
		}

		public CustomMenuCreationService(XRDesignPanel controller)
		{
			_controller = controller;
		}

		public void ProcessMenuItems(MenuKind menuKind, MenuItemDescriptionCollection items)
		{
			if (menuKind != MenuKind.Selection) return;

			MenuItemDescription toHide = null;

			foreach (var item in items.Where(item => item.Text == "С&войства"))
			{
				toHide = item;
			}

			if (toHide != null)
			{
				items.Remove(toHide);
			}
		}

		public MenuCommandDescription[] GetCustomMenuCommands()
		{
			return new MenuCommandDescription[0];
		}
	}
}
