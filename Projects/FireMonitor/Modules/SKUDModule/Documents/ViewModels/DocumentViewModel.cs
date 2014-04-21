using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;
using Infrastructure.Common.TreeList;

namespace SKDModule.ViewModels
{
	public class DocumentViewModel : TreeNodeViewModel<DocumentViewModel>
	{
		public Organisation Organization { get; private set; }
		public Document Document { get; private set; }

		public DocumentViewModel(Organisation organization)
		{
			Organization = organization;
			IsOrganisation = true;
			Name = organization.Name;
		}

		public DocumentViewModel(Document document)
		{
			Document = document;
			IsOrganisation = false;
			Name = document.Name;
			Description = document.Description;
		}

		public void Update(Document document)
		{
			Name = document.Name;
			Description = document.Description;
			OnPropertyChanged("Name");
			OnPropertyChanged("Description");
		}

		public bool IsOrganisation { get; private set; }

		public string Name { get; private set; }
		public string Description { get; private set; }
	}
}