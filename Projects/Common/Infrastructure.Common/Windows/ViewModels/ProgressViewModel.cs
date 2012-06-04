using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Infrastructure.Common.Windows.ViewModels
{
	public class ProgressViewModel : WindowBaseViewModel
	{
		public ProgressViewModel(bool restrictClose = true)
		{
			Sizable = false;
			TopMost = true;
			RestrictClose = restrictClose;
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

		public bool RestrictClose { get; private set; }

		public override bool OnClosing(bool isCanceled)
		{
			return RestrictClose;
		}
		public void ForceClose()
		{
			RestrictClose = false;
			Close();
		}
	}
}
