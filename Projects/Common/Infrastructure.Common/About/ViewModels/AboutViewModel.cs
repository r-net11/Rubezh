using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using System.Windows;
using System.Reflection;
using Infrastructure.Common.Windows.ViewModels;

namespace Infrastructure.Common.About.ViewModels
{
	public class AboutViewModel : SaveCancelDialogViewModel
	{
		public AboutViewModel()
		{
			Title = "О программе";
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
	}
}
