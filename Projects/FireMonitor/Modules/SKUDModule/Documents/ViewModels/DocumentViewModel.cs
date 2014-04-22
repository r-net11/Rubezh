using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;

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