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

		public string Version
		{
			get { return (Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly()).GetName().Version.ToString(); }
		}
	}
}