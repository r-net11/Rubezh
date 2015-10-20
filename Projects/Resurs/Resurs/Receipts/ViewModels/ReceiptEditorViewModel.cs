using DevExpress.XtraReports.UI;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Resurs.Receipts;
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
		bool isSaved;
		bool IsShowNotSavedMessage
		{
			get
			{
				if (SelectedReceipt == null || isSaved)
					return false;
				return isNewReceipt
					|| ReceiptEditorView.HasChangesFirstReceiptTemplate
					|| SelectedReceipt.Name != Name;
			}
		}
		public ReceiptEditorViewModel()
		{
			AddReceiptCommand = new RelayCommand(OnAddReceipt, CanAddReceipt);
			DeleteReceiptCommand = new RelayCommand(OnDeleteReceipt, CanDeleteReceipt);
			SaveReceiptCommand = new RelayCommand(OnSaveReceipt, CanSaveReceipt);

			Receipts = ReceiptHelper.GetEditableTemplates();
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
		List<ReceiptTemplate> _receipts;
		public List<ReceiptTemplate> Receipts 
		{
			get { return _receipts; }
			set
			{
				_receipts = value;
				OnPropertyChanged(() => Receipts);
			}
		}
		ReceiptTemplate _selectedReceipt;
		public ReceiptTemplate SelectedReceipt
		{
			get { return _selectedReceipt; }
			set
			{
				if (IsShowNotSavedMessage)
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
				isNewReceipt = false;
				isSaved = false;
				if (_selectedReceipt != null)
				{
					Name = SelectedReceipt.Name;
					ReceiptEditorView.CloseAllDocuments();
					ReceiptEditorView.OpenDocument(SelectedReceipt);
				}
				else
				{
					Name = "";
				}
			}
		}
		public RelayCommand AddReceiptCommand { get; private set; }
		void OnAddReceipt()
		{
			var newReceipt = new ReceiptTemplate
			{
				Name = "Новый шаблон",
				Uid = Guid.NewGuid()
			};
			SelectedReceipt = newReceipt;
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
				ReceiptHelper.DeleteReceipt(SelectedReceipt);
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
			var selectedReceipt = ReceiptEditorView.GetFirstReceiptTemplate();
			selectedReceipt.Name = Name;
			selectedReceipt.Uid = SelectedReceipt.Uid;
			ReceiptHelper.SaveReceipt(selectedReceipt);
			if (isNewReceipt)
			{
				Receipts.Add(selectedReceipt);
				Receipts = RewriteReceipts(Receipts);
				isNewReceipt = false;
			}
			isSaved = true;
			SelectedReceipt = selectedReceipt;
		}
		bool CanSaveReceipt()
		{
			return SelectedReceipt != null;
		}
		List<ReceiptTemplate> RewriteReceipts(List<ReceiptTemplate> receipts)
		{
			var newReceipts = new List<ReceiptTemplate>();
			newReceipts.AddRange(receipts);
			return newReceipts;
		}
	}
}