using System;
using System.Reflection;

namespace Infrastructure.Common
{
    public class ResourceHelper
    {
        static string InitialiPath(string resourcePath, string callingAssemblyName)
        {
            return string.Format("pack://application:,,,/{0};component{1}",
                                 callingAssemblyName,
                                 resourcePath.StartsWith("/") ? string.Empty : "/");
        }

        public static Uri ComposeResourceUri(Assembly callingAssembly, string resourcePath)
        {
            string urlString = string.Format("{0}{1}", InitialiPath(resourcePath, callingAssembly.FullName), resourcePath);

            return new Uri(urlString, UriKind.Absolute);
        }
    }
}