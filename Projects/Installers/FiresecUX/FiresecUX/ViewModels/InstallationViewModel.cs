using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;
using Infrastructure.Common.Windows;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using WinForms = System.Windows.Forms;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace FiresecUX.ViewModels
{
	public enum InstallationState
	{
		Initializing,
		DetectedAbsent,
		DetectedPresent,
		DetectedNewer,
		Applying,
		Applied,
		Failed,
	}

	public enum Error
	{
		UserCancelled = 1223,
	}

	internal class Hresult
	{
		public static bool Succeeded(int status)
		{
			return status >= 0;
		}
	}

	public class InstallationViewModel : BaseViewModel
	{
		FiresecUXViewModel _root;

		public InstallationViewModel(FiresecUXViewModel root)
		{
			_root = root;

			FiresecUX.Model.Bootstrapper.DetectBegin += DetectBegin;
			FiresecUX.Model.Bootstrapper.DetectRelatedBundle += DetectedRelatedBundle;
			FiresecUX.Model.Bootstrapper.DetectPackageComplete += this.DetectedPackage;
			FiresecUX.Model.Bootstrapper.DetectComplete += this.DetectComplete;
			FiresecUX.Model.Bootstrapper.PlanPackageBegin += this.PlanPackageBegin;
			FiresecUX.Model.Bootstrapper.PlanComplete += this.PlanComplete;
			FiresecUX.Model.Bootstrapper.ApplyBegin += this.ApplyBegin;
			FiresecUX.Model.Bootstrapper.CacheAcquireBegin += this.CacheAcquireBegin;
			FiresecUX.Model.Bootstrapper.CacheAcquireComplete += this.CacheAcquireComplete;
			FiresecUX.Model.Bootstrapper.ExecutePackageBegin += this.ExecutePackageBegin;
			FiresecUX.Model.Bootstrapper.ExecutePackageComplete += this.ExecutePackageComplete;
			FiresecUX.Model.Bootstrapper.Error += this.ExecuteError;
			FiresecUX.Model.Bootstrapper.ResolveSource += this.ResolveSource;
			FiresecUX.Model.Bootstrapper.ApplyComplete += this.ApplyComplete;
		}

		private Dictionary<string, int> _downloadRetries;
		LaunchAction _plannedAction;
		bool _planAttempted;
		IntPtr _hwnd;
		DateTime _cachePackageStart;
		DateTime _executePackageStart;

		bool _downgrade;
		public bool Downgrade
		{
			get { return _downgrade; }
			set
			{
				if (_downgrade != value)
				{
					_downgrade = value;
					base.OnPropertyChanged("Downgrade");
				}
			}
		}

		string _message;
		public string Message
		{
			get { return _message; }
			set
			{
				_message = value;
				base.OnPropertyChanged("Message");
			}
		}

		RelayCommand InstallCommand { get; set; }

		void Plan(LaunchAction action)
		{
			_planAttempted = true;
			_plannedAction = action;
			_hwnd = (FiresecUX.View == null) ? IntPtr.Zero : new WindowInteropHelper(FiresecUX.View).Handle;

			_root.Canceled = false;
			FiresecUX.Model.Engine.Plan(_plannedAction);
		}

		void PlanLayout()
		{
			// Either default or set the layout directory
			if (String.IsNullOrEmpty(FiresecUX.Model.Command.LayoutDirectory))
			{
				FiresecUX.Model.LayoutDirectory = System.IO.Directory.GetCurrentDirectory();

				// Ask the user for layout folder if one wasn't provided and we're in full UI mode
				if (FiresecUX.Model.Command.Display == Display.Full)
				{
					FiresecUX.Dispatcher.Invoke((Action)delegate()
					{
						WinForms.FolderBrowserDialog browserDialog = new WinForms.FolderBrowserDialog();
						browserDialog.RootFolder = Environment.SpecialFolder.MyComputer;

						// Default to the current directory.
						browserDialog.SelectedPath = FiresecUX.Model.LayoutDirectory;
						WinForms.DialogResult result = browserDialog.ShowDialog();

						if (WinForms.DialogResult.OK == result)
						{
							FiresecUX.Model.LayoutDirectory = browserDialog.SelectedPath;
							this.Plan(FiresecUX.Model.Command.Action);
						}
						else
						{
							FiresecUX.View.Close();
						}
					}
					);
				}
			}
			else
			{
				FiresecUX.Model.LayoutDirectory = FiresecUX.Model.Command.LayoutDirectory;

				FiresecUX.Dispatcher.Invoke((Action)delegate()
				{
					this.Plan(FiresecUX.Model.Command.Action);
				}
				);
			}
		}

		void DetectBegin(object sender, DetectBeginEventArgs e)
		{
			_root.State = InstallationState.Initializing;
			_planAttempted = false;
		}

		void DetectedRelatedBundle(object sender, DetectRelatedBundleEventArgs e)
		{
			if (e.Operation == RelatedOperation.Downgrade)
			{
				this.Downgrade = true;
			}
		}

		void DetectedPackage(object sender, DetectPackageCompleteEventArgs e)
		{
			if (e.PackageId.Equals("Firesec", StringComparison.Ordinal))
			{ _root.State = (e.State == PackageState.Present) ? InstallationState.DetectedPresent : InstallationState.DetectedAbsent; }
		}

		void DetectComplete(object sender, DetectCompleteEventArgs e)
		{
			if (LaunchAction.Uninstall == FiresecUX.Model.Command.Action)
			{
				FiresecUX.Model.Engine.Log(LogLevel.Verbose, "Invoking automatic plan for uninstall");
				FiresecUX.Dispatcher.Invoke((Action)delegate()
				{
					Plan(LaunchAction.Uninstall);
				}
				);
			}
			else if (Hresult.Succeeded(e.Status))
			{
				if (Downgrade == true)
				{
					// TODO: What behavior do we want for downgrade?
					_root.State = InstallationState.DetectedNewer;
				}

				if (LaunchAction.Layout == FiresecUX.Model.Command.Action)
				{
					PlanLayout();
				}
				else if (FiresecUX.Model.Command.Display != Display.Full)
				{
					// If we're not waiting for the user to click install, dispatch plan with the default action.
					FiresecUX.Model.Engine.Log(LogLevel.Verbose, "Invoking automatic plan for non-interactive mode.");
					FiresecUX.Dispatcher.Invoke((Action)delegate()
					{
						this.Plan(FiresecUX.Model.Command.Action);
					}
					);
				}
			}
			else
			{
				_root.State = InstallationState.Failed;
			}
		}

		void PlanPackageBegin(object sender, PlanPackageBeginEventArgs e)
		{
			if (FiresecUX.Model.Engine.StringVariables.Contains("MbaNetfxPackageId") && e.PackageId.Equals(FiresecUX.Model.Engine.StringVariables["MbaNetfxPackageId"],
				StringComparison.Ordinal))
			{
				e.State = RequestState.None;
			}
		}

		void PlanComplete(object sender, PlanCompleteEventArgs e)
		{
			if (Hresult.Succeeded(e.Status))
			{
				_root.PreApplyState = _root.State;
				_root.State = InstallationState.Applying;
				FiresecUX.Model.Engine.Apply(_hwnd);
			}
			else
			{
				_root.State = InstallationState.Failed;
			}
		}

		void ApplyBegin(object sender, ApplyBeginEventArgs e)
		{
			_downloadRetries.Clear();
		}

		void CacheAcquireBegin(object sender, CacheAcquireBeginEventArgs e)
		{
			_cachePackageStart = DateTime.Now;
		}

		void CacheAcquireComplete(object sender, CacheAcquireCompleteEventArgs e)
		{
			this.AddPackageTelemetry("Cache", e.PackageOrContainerId ?? String.Empty, DateTime.Now.Subtract(_cachePackageStart).TotalMilliseconds, e.Status);
		}

		void ExecutePackageBegin(object sender, ExecutePackageBeginEventArgs e)
		{
			_executePackageStart = e.ShouldExecute ? DateTime.Now : DateTime.MinValue;
		}

		void ExecutePackageComplete(object sender, ExecutePackageCompleteEventArgs e)
		{
			if (DateTime.MinValue < _executePackageStart)
			{
				this.AddPackageTelemetry("Execute", e.PackageId ?? String.Empty, DateTime.Now.Subtract(_executePackageStart).TotalMilliseconds, e.Status);
				_executePackageStart = DateTime.MinValue;
			}
		}

		void ExecuteError(object sender, ErrorEventArgs e)
		{
			lock (this)
			{
				if (!_root.Canceled)
				{
					// If the error is a cancel coming from the engine during apply we want to go back to the preapply state.
					if (InstallationState.Applying == _root.State && (int)Error.UserCancelled == e.ErrorCode)
					{
						_root.State = _root.PreApplyState;
					}
					else
					{
						this.Message = e.ErrorMessage;

						FiresecUX.View.Dispatcher.Invoke((Action)delegate()
						{
							MessageBox.Show(FiresecUX.View, e.ErrorMessage, "WiX Toolset", MessageBoxButton.OK, MessageBoxImage.Error);
						}
							);
					}
				}

				e.Result = _root.Canceled ? Result.Cancel : Result.Ok;
			}
		}

		void ResolveSource(object sender, ResolveSourceEventArgs e)
		{
			int retries = 0;

			_downloadRetries.TryGetValue(e.PackageOrContainerId, out retries);
			_downloadRetries[e.PackageOrContainerId] = retries + 1;

			e.Result = retries < 3 && !String.IsNullOrEmpty(e.DownloadSource) ? Result.Download : Result.Ok;
		}

		void ApplyComplete(object sender, ApplyCompleteEventArgs e)
		{
			FiresecUX.Model.Result = e.Status; // remember the final result of the apply.

			// If we're not in Full UI mode, we need to alert the dispatcher to stop and close the window for passive.
			if (Microsoft.Tools.WindowsInstallerXml.Bootstrapper.Display.Full != FiresecUX.Model.Command.Display)
			{
				// If its passive, send a message to the window to close.
				if (Microsoft.Tools.WindowsInstallerXml.Bootstrapper.Display.Passive == FiresecUX.Model.Command.Display)
				{
					FiresecUX.Model.Engine.Log(LogLevel.Verbose, "Automatically closing the window for non-interactive install");
					FiresecUX.Dispatcher.BeginInvoke((Action)delegate()
					{
						FiresecUX.View.Close();
					}
					);
				}
				else
				{
					FiresecUX.Dispatcher.InvokeShutdown();
				}
			}

			// Set the state to applied or failed unless the state has already been set back to the preapply state
			// which means we need to show the UI as it was before the apply started.
			if (_root.State != _root.PreApplyState)
			{
				_root.State = Hresult.Succeeded(e.Status) ? InstallationState.Applied : InstallationState.Failed;
			}
		}

		void AddPackageTelemetry(string prefix, string id, double time, int result)
		{
			lock (this)
			{
				string key = String.Format("{0}Time_{1}", prefix, id);
				string value = time.ToString();
				FiresecUX.Model.Telemetry.Add(new KeyValuePair<string, string>(key, value));

				key = String.Format("{0}Result_{1}", prefix, id);
				value = String.Concat("0x", result.ToString("x"));
				FiresecUX.Model.Telemetry.Add(new KeyValuePair<string, string>(key, value));
			}
		}
	}
}
