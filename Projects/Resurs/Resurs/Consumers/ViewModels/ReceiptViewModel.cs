using DevExpress.Xpf.Printing;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Resurs.Reports;
using Resurs.Reports.Templates;
using ResursAPI;

namespace Resurs.ViewModels
{
	public class ReceiptViewModel : DialogViewModel
	{
		public Consumer Consumer {get; private set;}
		public ReportPreviewModel Model { get; private set; }
		public ReceiptViewModel(Consumer consumer)
		{			
			FitPageSizeCommand = new RelayCommand<ZoomFitMode>(OnFitPageSize, CanFitPageSize);

			Consumer = consumer;
			Model = CreateModel();
			BuildReceipt();
		}
		public RelayCommand<ZoomFitMode> FitPageSizeCommand { get; private set; }
		private void OnFitPageSize(ZoomFitMode fitMode)
		{
			if (fitMode == ZoomFitMode.WholePage)
			{
				Model.ZoomMode = null;
				Model.SetZoom(100);
				return;
			}
			Model.ZoomMode = new ZoomFitModeItem(fitMode);
		}
		private bool CanFitPageSize(ZoomFitMode fitMode)
		{
			return Model != null && Model.PrintCommand.CanExecute(null);
		}
		ReportPreviewModel CreateModel()
		{
			var receipt = new ReceiptTemplate();
			return new ReportPreviewModel(receipt)
			{
				AutoShowParametersPanel = false,
				IsParametersPanelVisible = false,
				IsDocumentMapVisible = false
			};
		}
		void BuildReceipt()
		{
			Model.Report.CreateDocument();
		}

	}
}