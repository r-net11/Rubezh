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
		public ReceiptTemplate Receipt { get; private set;}
		public ReportPreviewModel Model { get; set; }
		public ReceiptViewModel(ReceiptTemplate receipt)
		{			
			FitPageSizeCommand = new RelayCommand<ZoomFitMode>(OnFitPageSize, CanFitPageSize);

			Receipt = receipt;
			Model = CreateModel();
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
			Receipt.CreateDocument();
			return new ReportPreviewModel(Receipt)
			{
				AutoShowParametersPanel = false,
				IsParametersPanelVisible = false,
				IsDocumentMapVisible = false
			};
		}
	}
}