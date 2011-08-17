using System.Windows;
using Infrastructure.Common;

namespace FireMonitor
{
    public class ResourceService : IResourceService
    {
        public void AddResource(ResourceDescription description)
        {
            var resourceDescription = new ResourceDictionary() { Source = description.Source };
            Application.Current.Resources.MergedDictionaries.Add(resourceDescription);
        }
    }
}