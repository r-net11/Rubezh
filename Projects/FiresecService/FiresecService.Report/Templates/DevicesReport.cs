using System.Data;
using RubezhClient;
using FiresecService.Report.DataSources;
using System.Text;
using RubezhAPI.SKD.ReportFilters;
using System.Linq;

namespace FiresecService.Report.Templates
{
	public partial class DevicesReport : BaseReport
	{
		public DevicesReport()
		{
			InitializeComponent();
		}
		protected override bool ForcedLandscape
		{
			get { return true; }
		}
		protected override bool IsNotDataBase
		{
			get { return true; }
		}
		public override string ReportTitle
		{
			get { return "Список устройств"; }
		}
		public override void ApplyFilter(SKDReportFilter filter)
		{
			base.ApplyFilter(filter);
		}
		protected override DataSet CreateDataSet()
		{
			var filter = Filter as DevicesReportFilter;
			var devices = GKManager.Devices;
			var dataSet = new DevicesDataSet();
			if (filter != null && filter.SelectedDevices != null)
			{
				foreach (var selectedDevice in filter.SelectedDevices)
				{
					var device = devices.FirstOrDefault(x => x.UID == selectedDevice.UID);
					var dataRow = dataSet.Data.NewDataRow();
					var presentationName = new StringBuilder();
					presentationName.Append(' ', device.AllParents.Count * 5);
					if (device.AllChildren.Count > 0)
						presentationName.Append(" - ");
					else
						presentationName.Append("   ");
					presentationName.Append(device.PresentationName);
					dataRow.PresentationName = presentationName.ToString();
					dataRow.PresentationAddress = device.PresentationAddress;
					dataRow.PresentationZoneOrLogic = GKManager.GetPresentationZoneOrLogic(device);
					dataRow.Description = device.Description;
					dataSet.Data.Rows.Add(dataRow);
				}
			}
			return dataSet;
		}
	}
}