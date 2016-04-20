using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Resurs.Receipts;
using Resurs.Reports.Templates;
using Resurs.Views;
using ResursAPI;
using ResursDAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace Resurs.ViewModels
{
	public class ReceiptEditorViewModel : BaseViewModel
	{
		string SaveQuestion
		{
			get
			{
				var saveMessage = SelectedReceipt != null ? string.Format("Не сохранены изменения в шаблоне \"{0}\".\nСохранить изменения?", SelectedReceipt.Name) : string.Empty;
				return saveMessage;
			}
		}
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
		public static CancelEventHandler CancelEventHandler { get; private set; }

		public ReceiptEditorViewModel()
		{
			AddReceiptCommand = new RelayCommand(OnAddReceipt, CanAddReceipt);
			DeleteReceiptCommand = new RelayCommand(OnDeleteReceipt, CanDeleteReceipt);
			SaveReceiptCommand = new RelayCommand(OnSaveReceipt, CanSaveReceipt);

			CancelEventHandler = Save;
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
				string savedName = null;
				if (IsShowNotSavedMessage)
				{
					var messageBoxResult = MessageBoxService.ShowQuestionExtended(SaveQuestion);
					switch (messageBoxResult)
					{
						case MessageBoxResult.Yes:
							SaveReceiptCommand.Execute();
							if (isNewReceipt)
							{
								Receipts.Remove(_selectedReceipt);
								Receipts = RewriteReceipts(Receipts);
							}
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
							savedName = Name;
							break;
					}
				}
				_selectedReceipt = value;
				OnPropertyChanged(() => SelectedReceipt);
				isNewReceipt = false;
				isSaved = false;
				if (_selectedReceipt != null)
				{
					Name = savedName ?? _selectedReceipt.Name;
					ReceiptEditorView.CloseAllDocuments();
					ReceiptEditorView.OpenDocument(SelectedReceipt);
				}
				else
				{
					Name = string.Empty;
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
			Receipts.Add(newReceipt);
			Receipts = RewriteReceipts(Receipts);
			SelectedReceipt = Receipts.Find(x => x == newReceipt);
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
				isSaved =true;
				SelectedReceipt = null;
			}
		}
		public bool IsVisible
		{
			get { return DbCache.CheckPermission(PermissionType.EditReceipt); }
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
			var index = Receipts.IndexOf(SelectedReceipt);
			Receipts.Insert(index, selectedReceipt);
			Receipts.Remove(SelectedReceipt);
			Receipts = RewriteReceipts(Receipts);
			isSaved = true;
			isNewReceipt = false;
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
		void Save(object sender, CancelEventArgs e)
		{
			if (IsShowNotSavedMessage && MessageBoxService.ShowQuestion(SaveQuestion))
			{
				SaveReceiptCommand.Execute();
			}
		}
	}
}