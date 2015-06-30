using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;

[assembly: AssemblyTitle("FiresecBootstrapperApplication")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("FiresecBootstrapperApplication")]
[assembly: AssemblyCopyright("Copyright ©  2014")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]

[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

// Identifies the class that derives from UserExperience and is the UX class that gets
// instantiated by the interop layer
[assembly: BootstrapperApplication(typeof(FiresecBootstrapperApplication.FiresecBootstrapperApplication))]

