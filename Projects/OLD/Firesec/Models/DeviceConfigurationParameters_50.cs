﻿//------------------------------------------------------------------------------
// <auto-generated>
//	 Этот код создан программой.
//	 Исполняемая версия:2.0.50727.3053
//
//	 Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//	 повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=2.0.50727.3038.
// 
namespace Firesec.Models.DeviceCustomFunctions_50
{


	/// <remarks/>
	[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
	[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
	public class Requests
	{
		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute("Request", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
		public requestsRequest[] Request;
	}
	
	/// <remarks/>
	[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
	public partial class requestsRequest
	{
		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute("param", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
		public requestsParam[] param;

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public int ID;
		
		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string State;

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string Code;

		///// <remarks/>
		//[System.Xml.Serialization.XmlAttributeAttribute()]
		//public int paramCount;

		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string RequestType;

		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string StartTime;

		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string BeginTime;

		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string EndTime;

		[System.Xml.Serialization.XmlAttributeAttribute()]
		public int coNum;

		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string coCount;

		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string coProgress;

		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string coCaption;

		[System.Xml.Serialization.XmlElementAttribute("ResultString", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
		public string resultString;
	}

	/// <remarks/>
	[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public partial class requestsParam
	{
		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string name;

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public int value;
	}

	/// <remarks/>
	[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
	[System.SerializableAttribute()]
	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.ComponentModel.DesignerCategoryAttribute("code")]
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public partial class resultString
	{
	}
}