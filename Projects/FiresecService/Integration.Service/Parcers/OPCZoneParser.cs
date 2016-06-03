using Integration.Service.Entities;
using System.Linq;
using System.Xml.Linq;

namespace Integration.Service.Parcers
{
	internal sealed class OPCZoneParser : XMLParserBase<OPCZoneMessage>
	{
		public OPCZoneParser(string input) : base(input)
		{
		}

		protected override void Load()
		{
			XElements = XElement.Parse(InputString).Elements("zone");
		}

		protected override void Parse()
		{
			ResultCollection = XElements
				.Select(zoneXML => new OPCZoneMessage
				{
					Id = (string) zoneXML.Attribute("no"),
					Name = (string) zoneXML.Attribute("name"),
					Description = (string) zoneXML.Attribute("desc"),
					Autoset =
						(string)
							zoneXML.Descendants("param")
								.Where(x => x.Attribute("name") != null && x.Attribute("name").Value == "AutoSet")
								.Select(x => x.Attribute("value"))
								.FirstOrDefault(),
					Delay =
						(string)
							zoneXML.Descendants("param")
								.Where(x => x.Attribute("name") != null && x.Attribute("name").Value == "Delay")
								.Select(x => x.Attribute("value"))
								.FirstOrDefault(),
					GuardZoneType =
						(string)
							zoneXML.Descendants("param")
								.Where(x => x.Attribute("name") != null && x.Attribute("name").Value == "GuardZoneType")
								.Select(x => x.Attribute("value"))
								.FirstOrDefault(),
					IsSkippedTypeEnabled =
						(string)
							zoneXML.Descendants("param")
								.Where(x => x.Attribute("name") != null && x.Attribute("name").Value == "Skipped")
								.Select(x => x.Attribute("value"))
								.FirstOrDefault(),
					ZoneType =
						(string)
							zoneXML.Descendants("param")
								.Where(x => x.Attribute("name") != null && x.Attribute("name").Value == "ZoneType")
								.Select(x => x.Attribute("value"))
								.FirstOrDefault()
				});
		}
	}
}
