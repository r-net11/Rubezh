using System.Linq;
using System.Xml.Linq;
using Integration.Service.Entities;

namespace Integration.Service.Parcers
{
	internal sealed class ScriptParser : XMLParserBase<ScriptMessage>
	{
		public ScriptParser(string input) : base(input)
		{
		}

		protected override void Load()
		{
			XElements = XElement.Parse(InputString).Elements("scenaries").Elements("scenario");
		}

		protected override void Parse()
		{
			ResultCollection = XElements
				.Select(scriptXML => new ScriptMessage
				{
					Id = (string) scriptXML.Attribute("id"),
					Name = (string) scriptXML.Attribute("caption"),
					Description = (string) scriptXML.Attribute("desc"),
					IsEnabled = (string) scriptXML.Attribute("enabled")
				});
		}
	}
}
