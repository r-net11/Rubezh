using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using DiagnosticsModule.Models_CC;

namespace DiagnosticsModule.Models
{
	public static class SerializerHelper
	{
		static string FileName = "D:/Model.xml";

		public static void Load()
		{
			if (File.Exists(FileName))
			{
				using (var fileStream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				{
					//var overrides = new XmlAttributeOverrides();
					//var ignore = new XmlAttributes { XmlIgnore = true };
					var xmlSerializer = new XmlSerializer(typeof(ModelB));
					var modelB = (ModelB)xmlSerializer.Deserialize(fileStream);

					var name = modelB.Name;
					var description = modelB.Description;
					var count = modelB.Count;
				}
			}
		}

		public static void Save()
		{
			var modelB = new ModelB();
			modelB.Name = "Hello 1";
			modelB.Description = "Hello 2";
			modelB.Count = 123;
			modelB.PublicField = "Public Field";
			modelB.ElementProperty = "Element Property";
			modelB.AttributeProperty = "Attribute Property";
			modelB.ModelC1 = new ModelC();
			modelB.ModelC1.Name = "ModelC1_Name";
			modelB.ModelC1.Description = "ModelC1_Description";
			modelB.ModelC1.NonAutoProperty = "Non Auto Property";
			modelB.ModelItems.Add(new ModelItem { Name = "Item 1" });
			modelB.ModelItems.Add(new ModelItem { Name = "Item 2" });
			modelB.ModelItems.Add(new ModelItem { Name = "Item 3" });
			modelB.VirtualPrioerty = "Virtual Prioerty";

			//var overrides = new XmlAttributeOverrides();
			//var ignore = new XmlAttributes { XmlIgnore = true };
			var xmlSerializer = new XmlSerializer(typeof(ModelB));
			using (var fileStream = new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
			{
				xmlSerializer.Serialize(fileStream, modelB);
			}
		}
	}
}