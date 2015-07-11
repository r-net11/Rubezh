using Softing.Opc.Ua.Toolkit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationModule.ViewModels
{
    public class ConfigClass
	{
		[DebuggerStepThrough]
		public bool LoadApplicationConfiguration()
		{
			Application.Configuration.ApplicationName = "Softing OPC UA .NET Toolkit Tutorial Client";
			Application.Configuration.ProductUri = "http://industrial.softing.com/OpcUaNetToolkit/ClientTutorial";

			// security configuration
			string applicationFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"Softing\OpcUaNetToolkit\V1.27\src\Samples\Client");

			Application.Configuration.Security.ApplicationCertificateStore = Path.Combine(applicationFolder, @"pki\own");
			Application.Configuration.Security.ApplicationCertificateSubject = Application.Configuration.ApplicationName;
			Application.Configuration.Security.TrustedCertificateStore = Path.Combine(applicationFolder, @"pki\trusted");
			Application.Configuration.Security.TrustedIssuerCertificateStore = Path.Combine(applicationFolder, @"pki\issuer");
			Application.Configuration.Security.RejectedCertificateStore = Path.Combine(applicationFolder, @"pki\rejected");

			Application.CertificateValidation += Application_CertificateValidation;

			Application.Configuration.Validate();


			Application.UseUaValidationForHttps();

			// trace configuration
			Application.Configuration.Trace.LogFileName = @"Logs\TutorialClient.txt";
			Application.Configuration.Trace.LogFileMaxSize = 10;
			Application.Configuration.Trace.LogFileMaxRollBackups = 5;
			Application.Configuration.Trace.LogFileTracelevel = TraceLevels.Warning;
			//enable all masks
			Application.Configuration.Trace.LogFileTraceMask = 0x00FF00FF;
			Application.Configuration.Trace.Tracelevel = TraceLevels.Warning;
			//enable all masks
			Application.Configuration.Trace.TraceMask = 0x00FF00FF;

			return true;
		}

		private static void Application_CertificateValidation(object sender, CertificateValidationEventArgs e)
		{
			e.ValidationOption = CertificateValidationOption.AcceptOnce;
		}
	}
}