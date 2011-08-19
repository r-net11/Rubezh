using System.Windows;
using Infrastructure.Common;

namespace FireAdministrator
{
    public class ResourceService : IResourceService
    {
        public void AddResource(ResourceDescription description)
        {
            var resourceDictionary = new ResourceDictionary() { Source = description.Source };
            Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
        }
    }
}