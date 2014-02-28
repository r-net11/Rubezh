using System.Runtime.Serialization;
using System.Windows.Media;
using Infrustructure.Plans.Elements;
using FiresecAPI.Models;

namespace FiresecAPI.SKD.PassCardLibrary
{
	[DataContract]
	public class ElementPassCardImageProperty : ElementRectangle, IElementPassCardProperty
	{
		public ElementPassCardImageProperty()
		{
		}

		public override ElementBase Clone()
		{
			ElementPassCardImageProperty elementBase = new ElementPassCardImageProperty();
			Copy(elementBase);
			return elementBase;
		}
		public override void Copy(ElementBase element)
		{
			base.Copy(element);
		}

		#region IElementPassCardProperty Members

		public string Property { get; set; }

		#endregion
	}
}