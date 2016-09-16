using System.ServiceModel;
using LicenseService.Entities;

namespace LicenseService
{
	[ServiceContract]
	public interface ILicenseService
	{
		[OperationContract]
		string GetDemoLicense(string userKey);

		[OperationContract]
		string GetProductLicense(ServiceLicenseData license);

		[OperationContract]
		string TestConnection();
	}
}
