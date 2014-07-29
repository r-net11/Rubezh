/************************************************************************

   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the New BSD
   License (BSD) as published at http://avalondock.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up AvalonDock in Extended WPF Toolkit Plus at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like facebook.com/datagrids

  **********************************************************************/

using System.Windows;
using System.Diagnostics;
using System;

namespace AvalonDock.MVVMTestApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			bool trace = true;
			BindingErrorListener.Listen(m => { if (trace) Console.WriteLine(m); });
		}
    }
	public class BindingErrorListener : TraceListener
	{
		private Action<string> _logAction;

		public static void Listen(Action<string> logAction)
		{
			PresentationTraceSources.DataBindingSource.Listeners.Add(new BindingErrorListener() { _logAction = logAction });
		}

		public override void Write(string message) { }
		public override void WriteLine(string message)
		{
			_logAction(message);
		}
	}
}
