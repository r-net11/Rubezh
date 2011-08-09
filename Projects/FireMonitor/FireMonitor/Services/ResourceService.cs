using System.Windows;
using Infrastructure.Common;

namespace FireMonitor
{
    public class ResourceService : IResourceService
    {
        public void AddResource(ResourceDescription description)
        {
            ResourceDictionary rd = new ResourceDictionary() { Source = description.Source };
            Application.Current.Resources.MergedDictionaries.Add(rd);
        }
    }
}