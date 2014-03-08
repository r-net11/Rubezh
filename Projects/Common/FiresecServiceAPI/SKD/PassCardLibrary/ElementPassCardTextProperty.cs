using System.Runtime.Serialization;
using FiresecAPI.Models;
using Infrustructure.Plans.Elements;

namespace FiresecAPI.SKD.PassCardLibrary
{
	[DataContract]
	public class ElementPassCardTextProperty : ElementTextBlock, IElementPassCardProperty
	{
		public ElementPassCardTextProperty()
		{
		}

		public override ElementBase Clone()
		{
			ElementPassCardTextProperty elementBase = new ElementPassCardTextProperty();
			Copy(elementBase);
			return elementBase;
		}
		public override void Copy(ElementBase element)
		{
			base.Copy(element);
			((ElementPassCardTextProperty)element).PropertyType = PropertyType;
		}

		#region IElementPassCardProperty Members

		[DataMember]
		public PassCardPropertyType PropertyType { get; set; }

		#endregion
	}
}