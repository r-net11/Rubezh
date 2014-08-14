using System;
using System.Runtime.Serialization;
using Infrustructure.Plans.Elements;
using Infrustructure.Plans.Interfaces;

namespace FiresecAPI.Models
{
	[DataContract]
	public class ElementProcedure : ElementRectangle, IElementReference, IPrimitive
	{
		public ElementProcedure()
		{
			ProcedureUID = Guid.Empty;
		}

		[DataMember]
		public Guid ProcedureUID { get; set; }

		public override ElementBase Clone()
		{
			ElementProcedure elementBase = new ElementProcedure();
			Copy(elementBase);
			return elementBase;
		}
		public override void Copy(ElementBase element)
		{
			base.Copy(element);
			((ElementProcedure)element).ProcedureUID = ProcedureUID;
		}
		public override void UpdateZLayer()
		{
			ZLayer = 70;
		}

		#region IElementReference Members

		Guid IElementReference.ItemUID
		{
			get { return ProcedureUID; }
			set { ProcedureUID = value; }
		}

		#endregion

		#region IPrimitive Members

		public override Primitive Primitive
		{
			get { return base.Primitive; }
		}

		#endregion
	}
}