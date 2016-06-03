using System.Collections.Generic;
using System.Xml.Linq;

namespace Integration.Service.Parcers
{
	internal abstract class XMLParserBase<T>
	{
		protected string InputString;
		protected IEnumerable<XElement> XElements;
		protected IEnumerable<T> ResultCollection;

		protected XMLParserBase(string input)
		{
			if (string.IsNullOrEmpty(input)) return;

			InputString = input;
		}

		protected abstract void Load();
		protected abstract void Parse();

		public IEnumerable<T> GetResult()
		{
			Load();

			if (XElements == null)
				return null;

			Parse();

			return ResultCollection;
		}
	}
}
