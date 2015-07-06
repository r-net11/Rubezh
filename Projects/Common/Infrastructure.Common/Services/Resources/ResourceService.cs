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
		public void AddResource(ResourceDescription description)
		{
			try
			{
				if (Application.Current != null)
					Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = description.Source });
			}
			catch { }
		}
	}
}