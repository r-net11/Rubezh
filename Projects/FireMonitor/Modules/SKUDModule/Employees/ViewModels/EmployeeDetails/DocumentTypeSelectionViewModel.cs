using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DocumentTypeSelectionViewModel : SaveCancelDialogViewModel
	{
		public DocumentTypeSelectionViewModel(EmployeeDocumentType employeeDocumentType)
		{
			Types = new List<SelectationDocumentTypeViewModel>();
			var documentTypes = Enum.GetValues(typeof(EmployeeDocumentType));
			foreach (EmployeeDocumentType documentType in documentTypes)
				Types.Add(new SelectationDocumentTypeViewModel(documentType));
			var selectedDocumentType = Types.FirstOrDefault(x => x.DocumentType == employeeDocumentType);
			selectedDocumentType.IsChecked = true;
		}

		public List<SelectationDocumentTypeViewModel> Types { get; private set; }
		public EmployeeDocumentType DocumentType;

		protected override bool Save()
		{
			DocumentType = Types.FirstOrDefault(x => x.IsChecked).DocumentType;
			return base.Save();
		}
	}

	public class SelectationDocumentTypeViewModel : BaseViewModel
	{
		public EmployeeDocumentType DocumentType { get; private set; }

		public SelectationDocumentTypeViewModel(EmployeeDocumentType documentType)
		{
			DocumentType = documentType;
		}

		public string Name { get { return DocumentType.ToDescription(); } }
		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged("IsChecked");
			}
		}
	}
}

