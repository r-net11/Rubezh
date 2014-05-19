using System.Collections.Generic;
using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class SKDPassCardLibraryConfiguration : VersionedConfiguration
	{
		public SKDPassCardLibraryConfiguration()
		{
			Templates = new List<PassCardTemplate>();
		}

		[DataMember]
		public List<PassCardTemplate> Templates { get; set; }

		public override bool ValidateVersion()
		{
			var result = true;
			if (Templates == null)
			{
				Templates = new List<PassCardTemplate>();
				result = false;
			}
			foreach (var template in Templates)
				result &= ValidateVersion(template);
			return result;
		}
		private bool ValidateVersion(PassCardTemplate template)
		{
			bool result = true;
			if (template.ElementExtensions == null)
			{
				template.ElementExtensions = new List<ElementBase>();
				result = false;
			}
			if (template.ElementImageProperties == null)
			{
				template.ElementImageProperties = new List<ElementPassCardImageProperty>();
				result = false;
			}
			if (template.ElementTextProperties == null)
			{
				template.ElementTextProperties = new List<ElementPassCardTextProperty>();
				result = false;
			}
			return result;
		}
	}
}