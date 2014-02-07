using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;

namespace SKDModule.ViewModels
{
	public class DocumentViewModel : BaseViewModel
	{
		public Document Document { get; private set; }

		public DocumentViewModel(Document document)
		{
			Document = document;
		}

		public void Update(Document document)
		{
			Document = document;
			OnPropertyChanged("Document");
		}
	}
}