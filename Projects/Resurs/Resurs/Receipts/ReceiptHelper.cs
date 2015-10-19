using DevExpress.XtraReports.UI;
using Resurs.Reports.Templates;
using ResursAPI;
using ResursDAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Resurs.Receipts
{
	public static class ReceiptHelper
	{
		static string DirectoryPath
		{
			get
			{
				var directoryPath = Path.Combine(FolderHelper.FolderName, "Шаблоны квитанций");
				if (!Directory.Exists(directoryPath))
					Directory.CreateDirectory(directoryPath);
				return directoryPath;
			}	
		}
		static DataContractSerializer _serializer = new DataContractSerializer(typeof(ReceiptTemplate));
		public static List<ReceiptTemplate> GetAllTemplate()
		{
			var templates = new List<ReceiptTemplate>();
			templates.Add(GetDefaultTemplate());
			templates.AddRange(GetEditableTemplates());
			return templates;
		}
		public static ReceiptTemplate GetDefaultTemplate()
		{
			var template = new ReceiptTemplate() { Name = "По умолчанию" };
			return template;
		}
		public static void SaveReceipt(ReceiptTemplate receipt)
		{
			var filePath = Path.Combine(DirectoryPath, receipt.Uid.ToString());
			receipt.SaveLayout(filePath);
			DBCash.SaveReceipt(new Receipt
			{
				Name = receipt.Name,
				UID = receipt.Uid,
				Description = receipt.Description,
				Template = File.ReadAllBytes(filePath)
			});

		}
		public static List<ReceiptTemplate> GetEditableTemplates()
		{
			var templates = new List<ReceiptTemplate>();
			var receipts = DBCash.GetAllReceipts();
			foreach (var receipt in receipts)
			{
				var filePath = Path.Combine(DirectoryPath, receipt.UID.ToString());
				if (!File.Exists(filePath))
					File.WriteAllBytes(filePath, receipt.Template);
				var template = (ReceiptTemplate)XtraReport.FromFile(filePath, true);
				template.Name = receipt.Name;
				templates.Add(template);
			}
			return templates;
		}
		public static void DeleteReceipt(ReceiptTemplate receipt)
		{
			var filePath = Path.Combine(DirectoryPath, receipt.Uid.ToString());
			if (File.Exists(filePath))
				File.Delete(filePath);
			DBCash.DeleteReceiptByUid(receipt.Uid);
		}
	}
}