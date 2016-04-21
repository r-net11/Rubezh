using System;
using System.Reflection;
using System.Windows;

namespace Infrastructure.Common
{
	/// <summary>
	/// Представляет сервис по работе с ресурсами WPF приложения 
	/// </summary>
	public class ResourceService : IResourceService
	{
		/// <summary>
		/// Добавляет ресурс в коллекцию ресурсов приложения
		/// </summary>
		/// <param name="description"></param>
		//public void AddResource(Assembly callerAssembly, string name)
		public void AddResource(Assembly callerAssembly, string name)
		{
			try
			{
				var source = ComposeResourceUri(callerAssembly, name);
				if (Application.Current != null)
					Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = source });
			}
			catch { }
		}

		public static Uri ComposeResourceUri(Assembly callingAssembly, string resourcePath)
		{
			var initialiPath = string.Format("pack://application:,,,/{0};component{1}", callingAssembly.FullName, resourcePath.StartsWith("/") ? string.Empty : "/");
			string urlString = string.Format("{0}{1}", initialiPath, resourcePath);
			return new Uri(urlString, UriKind.Absolute);
		}
	}
}