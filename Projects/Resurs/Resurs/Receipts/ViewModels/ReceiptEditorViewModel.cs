using DevExpress.XtraReports.UI;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Resurs.Consumers;
using Resurs.Reports.Templates;
using Resurs.Views;
using ResursAPI;
using ResursDAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Runtime.Serialization;
using System.Text;

namespace Resurs.ViewModels
{
	public class ReceiptEditorViewModel : BaseViewModel
	{
		bool isNewReceipt;
		string directoryPath;
		bool IsNotSavedReceipt
		{
			get
			{
				if (SelectedReceiptTemplate == null)
					return false;
				return isNewReceipt 
					|| ReceiptEditorView.HasChangesFirstReceiptTemplate
					|| SelectedReceiptTemplate.Name != Name;
			}
		}
		string FilePath 
		{
			get
			{
				if (SelectedReceipt != null)
				{
					var fileName = string.Format("{0}", SelectedReceipt.UID);
					return Path.Combine(directoryPath ,fileName);
				}
					
				return string.Empty;
			}
		}
		public ReceiptEditorViewModel()
		{
			AddReceiptCommand = new RelayCommand(OnAddReceipt, CanAddReceipt);
			DeleteReceiptCommand = new RelayCommand(OnDeleteReceipt, CanDeleteReceipt);
			SaveReceiptCommand = new RelayCommand(OnSaveReceipt, CanSaveReceipt);

			var folderName = FolderHelper.FolderName;
			directoryPath = Path.Combine(folderName, "Шаблоны квитанций");
			if (!Directory.Exists(directoryPath))
				Directory.CreateDirectory(directoryPath);
	
			Receipts = DBCash.GetAllReceipts();
		}
		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}
		List<Receipt> _receipts;
		public List<Receipt> Receipts 
		{
			get { return _receipts; }
			set
			{
				_receipts = value;
				OnPropertyChanged(() => Receipts);
			}
		}
		Receipt _selectedReceipt;
		public Receipt SelectedReceipt
		{
			get { return _selectedReceipt; }
			set
			{
				if (IsNotSavedReceipt)
				{
					var messageBoxResult = MessageBoxService.ShowQuestionExtended(string.Format("Не сохранены изменения в шаблоне \"{0}\".\nСохранить изменения?", SelectedReceipt.Name));
					switch (messageBoxResult)
					{
						case MessageBoxResult.Yes:
							if (isNewReceipt)
							{
								Receipts.Remove(_selectedReceipt);
								Receipts = RewriteReceipts(Receipts);
							}
							SaveReceiptCommand.Execute();
							break;
						case MessageBoxResult.No:
							if (isNewReceipt)
							{
								Receipts.Remove(_selectedReceipt);
								Receipts = RewriteReceipts(Receipts);
							}
							break;
						case MessageBoxResult.Cancel:
							value = _selectedReceipt; 
							break;
					}
				}
				_selectedReceipt = value;
				OnPropertyChanged(() => SelectedReceipt);
				if (_selectedReceipt != null)
				{
					Name = _selectedReceipt.Name;
					if (_selectedReceipt.Template == null)
						SelectedReceiptTemplate = new ReceiptTemplate { Name = "Новый шаблон" };
					else
					{
						if (!File.Exists(FilePath))
						{
							File.WriteAllBytes(FilePath, _selectedReceipt.Template);
						}
						SelectedReceiptTemplate = (ReceiptTemplate)XtraReport.FromFile(FilePath, true);
						SelectedReceiptTemplate.Name = value.Name;
					}
					ReceiptEditorView.CloseAllDocuments();
					ReceiptEditorView.OpenDocument(SelectedReceiptTemplate);
					isNewReceipt = false;
				}
			}
		}
		public ReceiptTemplate SelectedReceiptTemplate { get; private set; }
		public RelayCommand AddReceiptCommand { get; private set; }
		void OnAddReceipt()
		{
			SelectedReceipt = new Receipt();
			Receipts.Add(SelectedReceipt);
			Receipts = RewriteReceipts(Receipts);
			isNewReceipt = true;
		}
		bool CanAddReceipt()
		{
			return true;
		}
		public RelayCommand DeleteReceiptCommand { get; private set; }
		void OnDeleteReceipt()
		{
			if (MessageBoxService.ShowQuestion(string.Format("Вы уверены что хотите удалить шаблон \"{0}\"?", SelectedReceipt.Name)))
			{
				DBCash.DeleteReceipt(SelectedReceipt);
				if (File.Exists(FilePath))
					File.Delete(FilePath);
				Receipts.Remove(SelectedReceipt);
				Receipts = RewriteReceipts(Receipts);
				ReceiptEditorView.CloseAllDocuments();
				isNewReceipt = false;
				SelectedReceipt = null;
			}
		}
		bool CanDeleteReceipt()
		{
			return SelectedReceipt != null;
		}
		public RelayCommand SaveReceiptCommand { get; private set; }
		void OnSaveReceipt()
		{
			if (isNewReceipt)
			{
				Receipts.Add(SelectedReceipt);
				Receipts = RewriteReceipts(Receipts);
				isNewReceipt = false;
			}
			SelectedReceiptTemplate = ReceiptEditorView.GetFirstReceiptTemplate();
			SelectedReceiptTemplate.SaveLayout(FilePath);
			var template = File.ReadAllBytes(FilePath);
			SelectedReceipt.Template = template;
			SelectedReceipt.Name = Name;
			DBCash.SaveReceipt(SelectedReceipt);
		}
		bool CanSaveReceipt()
		{
			return SelectedReceiptTemplate != null;
		}
		List<Receipt> RewriteReceipts(List<Receipt> receipts)
		{
			var newReceipts = new List<Receipt>();
			newReceipts.AddRange(receipts);
			return newReceipts;
		}
	}
}