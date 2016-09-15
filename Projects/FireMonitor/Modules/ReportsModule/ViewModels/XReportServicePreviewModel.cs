using Common;
using DevExpress.DocumentServices.ServiceModel;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.DocumentServices.ServiceModel.ServiceOperations;
using DevExpress.Xpf.Printing;
using Infrastructure.Common.Windows;
using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Windows.Threading;

namespace ReportsModule.ViewModels
{
	public class XReportServicePreviewModel : ReportServicePreviewModel
	{
		private const int ClientPingPeriod = 300;
		private readonly DispatcherTimer _holdTimer;

		public XReportServicePreviewModel()
		{
			_holdTimer = new DispatcherTimer {Interval = TimeSpan.FromSeconds(ClientPingPeriod), IsEnabled = false};
			_holdTimer.Tick += PingClient;
		}

		public void Build(object args)
		{
			CreateDocumentInternal(args);
		}

		public string SelectedPage
		{
			get { return CurrentPageNumber.ToString(); }
			set
			{
				int intValue;
				if (int.TryParse(value, out intValue) && intValue >= 1 && intValue <= PageCount)
					CurrentPageNumber = intValue;
				RaisePropertyChanged(() => SelectedPage);
			}
		}

		public event EventHandler<SimpleFaultEventArgs> UnhandledError;

		protected override void OnCurrentPageIndexChanged()
		{
			try
			{
				base.OnCurrentPageIndexChanged();
				RaisePropertyChanged(() => SelectedPage);
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
				ApplicationService.BeginInvoke(() =>
				{
					if (UnhandledError != null)
						UnhandledError(this, new SimpleFaultEventArgs(ex));
				});
				Clear();
			}
		}
		protected override void RefreshPageOnReportBuildCompleted()
		{
			base.RefreshPageOnReportBuildCompleted();
			HoldClient(true);
		}
		protected override void ClearDocumentOnServer()
		{
			try
			{
				base.ClearDocumentOnServer();
			}
			catch (CommunicationException cex)
			{
				Logger.Error(cex);
				DocumentId = null;
				ResetClient();
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
			}
			finally
			{
				HoldClient(false);
			}
		}

		protected override CreateDocumentOperation ConstructCreateDocumentOperation(ReportBuildArgs buildArgs)
		{
			var operation = base.ConstructCreateDocumentOperation(buildArgs);
			return operation;
		}

		private ReadOnlyCollection<double> _zoomValues;
		protected override ReadOnlyCollection<double> ZoomValues
		{
			get { return _zoomValues ?? (_zoomValues = new ReadOnlyCollection<double>(new[] { 10.0, 25.0, 50.0, 75.0, 100.0, 150.0, 200.0, 300.0, 400.0, 500.0 })); }
		}

		private void ResetClient()
		{
			var clientField = typeof(ReportServicePreviewModel).GetField("client", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			if (clientField != null)
				clientField.SetValue(this, null);
		}

		private void HoldClient(bool isHold)
		{
			if (isHold)
				_holdTimer.Start();
			else
				_holdTimer.Stop();
		}
		private void PingClient(object sender, EventArgs e)
		{
			try
			{
				Client.GetBuildStatusAsync(DocumentId, null);
			}
			catch (Exception ex) //TODO: Need to specity the type of exception and throw it
			{
				Logger.Error(ex);
			}
		}
	}
}
