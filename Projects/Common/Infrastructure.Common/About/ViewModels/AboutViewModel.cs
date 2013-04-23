using System;
using System.Reflection;
using Common;
using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Common.About.ViewModels
{
	public class AboutViewModel : SaveCancelDialogViewModel
	{
		public AboutViewModel()
		{
			Title = "О программе";
			Sizable = false;
			AllowSave = false;
			CancelCaption = "Закрыть";
		}

		public string Product
		{
			get { return ((AssemblyProductAttribute)Attribute.GetCustomAttribute(Assembly.GetEntryAssembly(), typeof(AssemblyProductAttribute), false)).Product; }
		}
		public string Copyright
		{
			get { return ((AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(Assembly.GetEntryAssembly(), typeof(AssemblyCopyrightAttribute), false)).Copyright; }
		}
		public string Trademark
		{
			get { return ((AssemblyTrademarkAttribute)Attribute.GetCustomAttribute(Assembly.GetEntryAssembly(), typeof(AssemblyTrademarkAttribute), false)).Trademark; }
		}
		public string Company
		{
			get { return ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(Assembly.GetEntryAssembly(), typeof(AssemblyCompanyAttribute), false)).Company; }
		}
		public string Version
		{
			get { return Assembly.GetEntryAssembly().GetName().Version.ToString(); }
		}
		public DateTime BuildDate
		{
			get { return AssemblyHelper.GetAssemblyTimestamp(Assembly.GetEntryAssembly()); }
		}
	}
}