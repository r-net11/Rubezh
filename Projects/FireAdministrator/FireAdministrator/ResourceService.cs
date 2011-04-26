using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using System.Windows;

namespace FireAdministrator
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
