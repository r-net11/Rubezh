using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System.Windows.Threading;
using FiresecUX.ViewModels;

namespace FiresecUX
{
	public class FiresecUX : BootstrapperApplication
	{
		public FiresecUX()
		{
		}

		public static MainView View { get; private set; }
		public static FiresecUXModel Model { get; private set; }
		public static Dispatcher Dispatcher { get; private set; }

		protected override void Run()
		{
			FiresecUX.Model = new FiresecUXModel(this);
			FiresecUX.Dispatcher = Dispatcher.CurrentDispatcher;
			var firesecUXViewModel = new FiresecUXViewModel();
			// Create a Window to show UI.

			if (FiresecUX.Model.Command.Display == Display.Passive || FiresecUX.Model.Command.Display == Display.Full)
			{
				Engine.Log(LogLevel.Verbose, "Creating UI");
				FiresecUX.View = new MainView();
				FiresecUX.View.DataContext = firesecUXViewModel;
				FiresecUX.View.Show();
			}

			Dispatcher.Run();

			Engine.Quit(FiresecUX.Model.Result);
		}
	}
}
