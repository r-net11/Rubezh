using System;
using System.Reflection;

namespace Infrastructure.Common.Windows.ViewModels
{
	public class LoadingViewModel : ProgressViewModel
	{
		public LoadingViewModel()
			: base()
		{
			TopMost = true;
		}

		public string Product
		{
			get { return ((AssemblyProductAttribute)Attribute.GetCustomAttribute(Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly(), typeof(AssemblyProductAttribute), false)).Product; }
		}
		public string Copyright
		{
			get { return ((AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly(), typeof(AssemblyCopyrightAttribute), false)).Copyright; }
		}
		public string Trademark
		{
			get { return ((AssemblyTrademarkAttribute)Attribute.GetCustomAttribute(Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly(), typeof(AssemblyTrademarkAttribute), false)).Trademark; }
		}
		public string Company
		{
			get { return ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly(), typeof(AssemblyCompanyAttribute), false)).Company; }
		}
		public string Version
		{
			get { return (Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly()).GetName().Version.ToString(); }
		}
	}
}