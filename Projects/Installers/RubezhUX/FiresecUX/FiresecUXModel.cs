using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System.Reflection;
using System.Diagnostics;

namespace FiresecUX
{
	public class FiresecUXModel
	{
		Version version;
		const string BurnBundleLayoutDirectoryVariable = "WixBundleLayoutDirectory";

		public FiresecUXModel(BootstrapperApplication bootstrapper)
		{
			Bootstrapper = bootstrapper;
			Telemetry = new List<KeyValuePair<string, string>>();
		}

		//public List<KeyValuePair<string, string>> Telemetry { get; private set; }
		public BootstrapperApplication Bootstrapper { get; private set; }
		public int Result { get; set; }
		public List<KeyValuePair<string, string>> Telemetry { get; private set; }


		public Command Command { get { return this.Bootstrapper.Command; } }
		public Engine Engine { get { return this.Bootstrapper.Engine; } }

		public Version Version
		{
			get
			{
				if (null == this.version)
				{
					Assembly assembly = Assembly.GetExecutingAssembly();
					FileVersionInfo fileVersion = FileVersionInfo.GetVersionInfo(assembly.Location);

					this.version = new Version(fileVersion.FileVersion);
				}

				return this.version;
			}
		}

		public string LayoutDirectory
		{
			get
			{
				if (!this.Engine.StringVariables.Contains(BurnBundleLayoutDirectoryVariable))
				{
					return null;
				}

				return Engine.StringVariables[BurnBundleLayoutDirectoryVariable];
			}

			set
			{
				Engine.StringVariables[BurnBundleLayoutDirectoryVariable] = value;
			}
		}
	}
}
