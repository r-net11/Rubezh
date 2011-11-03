using System.Windows;
using Infrastructure.Common;

namespace FireMonitor
{
    public class ResourceService : IResourceService
    {
        public void AddResource(ResourceDescription description)
        {
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = description.Source });
        }
    }
}