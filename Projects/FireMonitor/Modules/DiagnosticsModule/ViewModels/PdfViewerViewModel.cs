using Firesec.Imitator;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using MigraDoc.DocumentObjectModel.IO;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering.Forms;
using PdfSharp.Pdf.IO;

namespace DiagnosticsModule.ViewModels
{
    public class PdfViewerViewModel : DialogViewModel
    {
		public PdfViewerViewModel()
        {
			CreatePdfCommand = new RelayCommand(OnCreatePdfCommand);
        }

		public RelayCommand CreatePdfCommand { get; private set; }
		private void OnCreatePdfCommand()
        {
			//DocumentPreview dp;
			//var document = PdfReader.Open(@"d:\Downloads\MigraDoc\samples\Samples C#\Based on WPF\DocumentViewer\bin\Debug\test.pdf");
			////Document doc = SampleDocuments.CreateSample1();
			//Ddl = DdlWriter.WriteToString(document);
		}

		private string _ddl;
		public string Ddl
		{
			get { return _ddl; }
			set
			{
				_ddl = value;
				OnPropertyChanged(() => Ddl);
			}
		}
		
    }
}