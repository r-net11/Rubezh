using Common;
using Infrastructure.Common.Windows.ViewModels;
using System;
using System.Reflection;
using Localization.Common.InfrastructureCommon;

namespace Infrastructure.Common.About.ViewModels
{
	public class AboutViewModel : SaveCancelDialogViewModel
	{
		public AboutViewModel()
		{
			Title = CommonResources.About;
			Sizable = false;
			AllowSave = false;
			CancelCaption = CommonResources.Close;
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