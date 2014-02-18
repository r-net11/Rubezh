using System;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.CheckBoxList;
using FiresecAPI;
using FiresecClient;

namespace SKDModule.ViewModels
{
	public class DocumentFilterViewModel : OrganizationFilterBaseViewModel<DocumentFilter>
	{
		public DocumentFilterViewModel(DocumentFilter filter):base(filter) { }
	}
}
