using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class DocumentTypeDetailsViewModel : SaveCancelDialogViewModel
	{
		public DocumentTypeDetailsViewModel()
		{
			Title = "Создание документа";
		}
	}
}
