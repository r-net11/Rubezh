using System.Runtime.Serialization;
using FiresecAPI.Models;
using Infrustructure.Plans.Elements;

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
			((ElementPassCardImageProperty)element).PropertyType = PropertyType;
		}

		#region IElementPassCardProperty Members

		[DataMember]
		public PassCardPropertyType PropertyType { get; set; }

		#endregion
	}
}