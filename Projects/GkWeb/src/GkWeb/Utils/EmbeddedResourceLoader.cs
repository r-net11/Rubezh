using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace GkWeb.Utils
{
	public static class EmbeddedResourceLoader
	{
		public static string LoadResource(string resourceName) {
			if (string.IsNullOrWhiteSpace(resourceName)) {
				throw new ArgumentNullException("resourceName");
			}
			var assembly = Assembly.GetExecutingAssembly();
			using (var stream = assembly.GetManifestResourceStream(resourceName)) {
				if (stream == null) {
					throw new Exception(string.Format("Resource{0} not found.", resourceName));
				}
				using (var reader = new StreamReader(stream)) {
					return reader.ReadToEnd();
				}
			}
		}
	}
}
