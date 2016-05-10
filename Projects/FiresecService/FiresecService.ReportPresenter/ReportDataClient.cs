using System;
using System.Collections.Generic;
using System.Data;
using System.ServiceModel;
using Common;
using FiresecService.Report;
using Infrastructure.Common;

namespace FiresecService.ReportPresenter
{
	public class ReportDataClient : IReportDataService
	{
		private static IReportDataService CreateClient()
		{
			string serverAddress = ConnectionSettingsManager.ReportDataServerAddress;
			var binding = BindingHelper.CreateBindingFromAddress(serverAddress);
			var endpointAddress = new EndpointAddress(new Uri(serverAddress));
			var channelFactory = new ChannelFactory<IReportDataService>(binding, endpointAddress);
			channelFactory.Open();
			return channelFactory.CreateChannel();
		}

		public List<CardData> GetCardsReport()
		{
			return this.client.Value.GetCardsReport();
		}

		public List<DepartmentData> GetDepartmentsReport()
		{
			return this.client.Value.GetDepartmentsReport();
		}

		public List<DeviceData> GetDevicesReport()
		{
			return this.client.Value.GetDevicesReport();
		}

		public List<DisciplineData> GetDisciplineReport()
		{
			return this.client.Value.GetDisciplineReport();
		}

		public List<DocumentData> GetDocumentsReport()
		{
			return this.client.Value.GetDocumentsReport();
		}

		public List<DoorData> GetDoorsReport()
		{
			return this.client.Value.GetDoorsReport();
		}

		public List<EmployeeAccessData> GetEmployeeAccessReport()
		{
			return this.client.Value.GetEmployeeAccessReport();
		}

		public List<EmployeeDoorsData> GetEmployeeDoorsReport()
		{
			return this.client.Value.GetEmployeeDoorsReport();
		}

		public EmployeeReportData GetEmployeeReport()
		{
			return this.client.Value.GetEmployeeReport();
		}

		public EmployeeRootReportData GetEmployeeRootReport()
		{
			return this.client.Value.GetEmployeeRootReport();
		}

		public List<EmployeeZonesData> GetEmployeeZonesReport()
		{
			return this.client.Value.GetEmployeeZonesReport();
		}

		public DataSet GetEmptyReport()
		{
			return this.client.Value.GetEmptyReport();
		}

		public List<EventsData> GetEventsReport()
		{
			return this.client.Value.GetEventsReport();
		}

		public List<PositionsData> GetPositionsReport()
		{
			return this.client.Value.GetPositionsReport();
		}

		public List<ReflectionData> GetReflectionReport()
		{
			return this.client.Value.GetReflectionReport();
		}

		public List<SchedulesData> GetSchedulesReport()
		{
			return this.client.Value.GetSchedulesReport();
		}

		public List<WorkingTimeData> GetWorkingTimeReport()
		{
			return this.client.Value.GetWorkingTimeReport();
		}

		#region Fields
		private readonly Lazy<IReportDataService> client = new Lazy<IReportDataService>(CreateClient);

		#endregion

	}
}
