using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WixSharp;

class Script
{
	static public void Main(string[] args)
	{
		Project project =
			new Project("A.C.Tech Key Generator",
				new Dir(@"%ProgramFiles%\A.C.Tech Key Generator\A.C.Tech Key Generator",
					new File(@"bin\Release\licence.dll"),
					new File(@"bin\Release\GenerateKeyApplication.exe",
						new FileShortcut("GenerateKeyApplication", "INSTALLDIR"), //INSTALLDIR is the ID of "%ProgramFiles%\My Company\My Product"
                        new FileShortcut("GenerateKeyApplication", @"%Desktop%") { IconFile = @"Icons\key.ico" }),
				//	new File(@"bin\Release\GenerateKeyApplication.exe.config"),
			//		new File(@"bin\Release\GenerateKeyApplication.pdb"),
					//new File(@"bin\Release\GenerateKeyApplication.vshost.exe"),
					//new File(@"bin\Release\GenerateKeyApplication.vshost.exe.config"),
				//	new File(@"bin\Release\GenerateKeyApplication.vshost.exe.manifest"),
					//new File(@"bin\Release\BootstrapperCore.dll"),
					//new File(@"bin\Release\BootstrapperCore.xml"),
					new File(@"bin\Release\System.Windows.Controls.Input.Toolkit.dll"),
					new File(@"bin\Release\System.Windows.Controls.Layout.Toolkit.dll"),
					new File(@"bin\Release\WPFToolkit.dll"),
					new File(@"bin\Release\Xceed.Wpf.AvalonDock.dll"),
					new File(@"bin\Release\Xceed.Wpf.AvalonDock.Themes.Aero.dll"),
					new File(@"bin\Release\Xceed.Wpf.AvalonDock.Themes.Metro.dll"),
					new File(@"bin\Release\Xceed.Wpf.AvalonDock.Themes.VS2010.dll"),
					new File(@"bin\Release\Xceed.Wpf.DataGrid.dll"),
					new File(@"bin\Release\Xceed.Wpf.Toolkit.dll"),
					new ExeFileShortcut("Uninstall A.C.Tech Key Generator", "[System64Folder]msiexec.exe", "/x [ProductCode]")),

					new Dir(@"%ProgramMenu%\A.C.Tech Key Generator\A.C.Tech Key Generator",
						new ExeFileShortcut("Samples", @"[" + @"%ProgramFiles%\A.C.Tech Key Generator\A.C.Tech Key Generator\Samples".ToDirID() + "]", ""),
						new ExeFileShortcut("Uninstall A.C.Tech Key Generator", "[System64Folder]msiexec.exe", "/x [ProductCode]")));

		project.GUID = new Guid("419cc434-fb04-417b-8794-e66f1c1e3d0a");
		project.UI = WUI.WixUI_ProgressOnly;
		project.OutFileName = "A.C.TechProductKeyGenInstaller";
        project.PreserveTempFiles = true;

		Compiler.BuildMsi(project);
	}
}
