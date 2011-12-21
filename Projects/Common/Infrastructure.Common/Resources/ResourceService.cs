using System.Windows;
using Infrastructure.Common;

namespace Infrastructure.Common
{
    public class ResourceService
    {
        public void AddResource(ResourceDescription description)
        {
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = description.Source });
        }
    }
}