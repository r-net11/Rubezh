using System.Collections.Generic;
using System.Data;
using System.ServiceModel;

namespace FiresecService.Report
{
    [ServiceContract()]
    public interface IReportDataService
    {
		[OperationContract()]List<CardData> GetCardsReport();
		[OperationContract()]List<DepartmentData> GetDepartmentsReport();
		[OperationContract()]List<DeviceData> GetDevicesReport();
		[OperationContract()]List<DisciplineData> GetDisciplineReport();
		[OperationContract()]List<DocumentData> GetDocumentsReport();
		[OperationContract()]List<DoorData> GetDoorsReport();
		[OperationContract()]List<EmployeeAccessData> GetEmployeeAccessReport();
		[OperationContract()]List<EmployeeDoorsData> GetEmployeeDoorsReport();
		[OperationContract()]EmployeeReportData GetEmployeeReport();
		[OperationContract()]EmployeeRootReportData GetEmployeeRootReport();
		[OperationContract()]List<EmployeeZonesData> GetEmployeeZonesReport();
		[OperationContract()]DataSet GetEmptyReport();
		[OperationContract()]List<EventsData> GetEventsReport();
		[OperationContract()]List<PositionsData> GetPositionsReport();
		[OperationContract()]List<ReflectionData> GetReflectionReport();
		[OperationContract()]List<SchedulesData> GetSchedulesReport();
		[OperationContract()]List<WorkingTimeData> GetWorkingTimeReport();
	}
}
