using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Windows;
using DevicesModule.ViewModels;
using DiagnosticsModule.Views;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.BalloonTrayTip;
using Infrastructure.Common.Mail;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Ionic.Zip;
using DiagnosticsModule.Mime;

namespace DiagnosticsModule.ViewModels
{
	public class DiagnosticsViewModel : ViewPartViewModel
	{
		public DiagnosticsViewModel()
		{
			ShowDriversCommand = new RelayCommand(OnShowDrivers);
			ShowXDriversCommand = new RelayCommand(OnShowXDrivers);
			ShowTreeCommand = new RelayCommand(OnShowTree);
			TreeListViewTestCommand = new RelayCommand(OnTreeListViewTest);
			Test1Command = new RelayCommand(OnTest1);
			Test2Command = new RelayCommand(OnTest2);
			Test3Command = new RelayCommand(OnTest3);
			Test4Command = new RelayCommand(OnTest4);
			Test5Command = new RelayCommand(OnTest5);
			Test6Command = new RelayCommand(OnTest6);
			Test7Command = new RelayCommand(OnTest7);
			Test8Command = new RelayCommand(OnTest8);
			Test9Command = new RelayCommand(OnTest9);
			Test10Command = new RelayCommand(OnTest10);
			Test11Command = new RelayCommand(OnTest11);
			BalloonTestCommand = new RelayCommand(OnBalloonTest);
			PlanDuplicateTestCommand = new RelayCommand(OnPlanDuplicateTest);
			MailCommand = new RelayCommand(OnMail);
		}

		public void StopThreads()
		{
			IsThreadStoping = true;
		}

		bool IsThreadStoping = false;

		string _text;

		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;
				OnPropertyChanged("Text");
			}
		}

		public RelayCommand ShowDriversCommand { get; private set; }

		private void OnShowDrivers()
		{
			var driversView = new DriversView();
			driversView.ShowDialog();
		}

		public RelayCommand ShowXDriversCommand { get; private set; }

		private void OnShowXDrivers()
		{
			var driversView = new XDriversView();
			driversView.ShowDialog();
		}

		public RelayCommand TreeListViewTestCommand { get; private set; }
		private void OnTreeListViewTest()
		{
			var treeViewTestViewModel = new TreeViewTestViewModel();
			DialogService.ShowModalWindow(treeViewTestViewModel);
		}

		public RelayCommand ShowTreeCommand { get; private set; }

		private void OnShowTree()
		{
			var devicesTreeViewModel = new DevicesTreeViewModel();
			DialogService.ShowModalWindow(devicesTreeViewModel);
		}

		int counter = 0;

		public RelayCommand Test1Command { get; private set; }
		private void OnTest1()
		{
			//var text = "ZCMPeNpdUEtOwzAQ3fcUUfbgOCiCSJPpDg5AF3RpOSaxlDqW7RJyPE7AFdhUVILegckH2rCa99G8+cD6dddEL8p53Zoi5tdJHCkj21Kbqog7bcq281c8zXi8xhX4vXsWUvllB0Ll2r31c0Vgi0r63Pe/LfRWoWyE91oCGxl0ugw18iQBNkGola7qgBkpMwQpbKAcfDt8Rsfvw+kI7FeiHXS5GaLSYf6MR/UxKPtE0ZM+sT9juzC2tLMRdtM+kIAc2AUDYfS9EzvlB+NMoBG9cpEuizjN89vkJr/LUrrSkIvvXx8nYCOE4ITxVjh6dI80dMGBjTFU56edkcfVD8cHmz8=";
			//byte[] data = System.Text.UnicodeEncoding.UTF8.GetBytes(text);
			//Base64Encoder myEncoder = new Base64Encoder(data);
			//StringBuilder sb = new StringBuilder();
			//sb.Append(myEncoder.GetEncoded());
			//text = sb.ToString();

			var text = "ZCMPeNpdUEtOwzAQ3fcUUfbgOCiCSJPpDg5AF3RpOSaxlDqW7RJyPE7AFdhUVILegckH2rCa99G8+cD6dddEL8p53Zoi5tdJHCkj21Kbqog7bcq281c8zXi8xhX4vXsWUvllB0Ll2r31c0Vgi0r63Pe/LfRWoWyE91oCGxl0ugw18iQBNkGola7qgBkpMwQpbKAcfDt8Rsfvw+kI7FeiHXS5GaLSYf6MR/UxKPtE0ZM+sT9juzC2tLMRdtM+kIAc2AUDYfS9EzvlB+NMoBG9cpEuizjN89vkJr/LUrrSkIvvXx8nYCOE4ITxVjh6dI80dMGBjTFU56edkcfVD8cHmz8=";
			char[] data = text.ToCharArray();
			Base64Decoder myDecoder = new Base64Decoder(data);
			StringBuilder sb = new StringBuilder();
			byte[] temp = myDecoder.GetDecoded();
			using (var fileStream = new FileStream("E:/xxx.zip", FileMode.Create))
			{
				fileStream.Write(temp, 0, temp.Length);
			}
			sb.Append(System.Text.UTF8Encoding.UTF8.GetChars(temp));
			text = sb.ToString();
		}

		public RelayCommand Test2Command { get; private set; }
		private void OnTest2()
		{
			var thread = new Thread(new ThreadStart(() =>
			{
				while (true)
				{
					if (IsThreadStoping)
						break;

					FiresecManager.FiresecDriver.SetNewConfig(FiresecManager.FiresecConfiguration.DeviceConfiguration);
					//FiresecManager.FiresecService.NotifyClientsOnConfigurationChanged();
					Thread.Sleep(TimeSpan.FromSeconds(5));
					Trace.WriteLine("SetNewConfig Count=" + counter.ToString() + " " + DateTime.Now.ToString());
					counter++;
				}
			}));
			thread.IsBackground = true;
			thread.Start();
		}

		public RelayCommand Test3Command { get; private set; }

		private void OnTest3()
		{
			var viewModel = new SVGTestViewModel();
			DialogService.ShowModalWindow(viewModel);
		}

		public RelayCommand Test4Command { get; private set; }

		private void OnTest4()
		{
		}

		public RelayCommand Test5Command { get; private set; }

		private void OnTest5()
		{
			var thread = new Thread(new ThreadStart(() =>
			{
				while (true)
				{
					if (IsThreadStoping)
						break;

					var journalRecords = new List<JournalRecord>();
					var journalRecord = new JournalRecord()
					{
						DeviceTime = DateTime.Now,
						SystemTime = DateTime.Now,
						Description = "TestEvent"
					};
					journalRecords.Add(journalRecord);
					FiresecManager.FiresecService.AddJournalRecords(journalRecords);
					Thread.Sleep(TimeSpan.FromSeconds(1));
					Trace.WriteLine("SetNewConfig Count=" + counter.ToString() + " " + DateTime.Now.ToString());
					counter++;
				}
			}));
			thread.IsBackground = true;
			thread.Start();
		}

		public RelayCommand Test6Command { get; private set; }

		private void OnTest6()
		{
			var thread = new Thread(new ThreadStart(() =>
			{
				int count = 0;
				while (true)
				{
					//if (Firesec.NativeFiresecClient.TasksCount > 10)
					//    continue;
					Thread.Sleep(TimeSpan.FromMilliseconds(1000));

					//var safeFiresecService = new SafeFiresecService("net.pipe://127.0.0.1/FiresecService/");
					//safeFiresecService.Connect(new ClientCredentials() { ClientType = ClientType.Administrator, ClientUID = Guid.NewGuid(), UserName = "adm", Password = "" }, true);
					//safeFiresecService.StartShortPoll(false);
					//safeFiresecService.ShortPoll();
					//safeFiresecService.Dispose();
					//Trace.WriteLine("Count = " + count++.ToString());

					FiresecManager.FiresecDriver.AddUserMessage("Test Message " + count++.ToString());
					if (count % 1000 == 0)
					{
						Trace.WriteLine("Count = " + count.ToString());
					}
				}
			}));
			thread.IsBackground = true;
			thread.Start();
		}

		public RelayCommand Test8Command { get; private set; }

		private void OnTest8()
		{
			FiresecManager.FiresecDriver.AddUserMessage("Single Test Message");
		}

		public RelayCommand Test7Command { get; private set; }

		private void OnTest7()
		{
			var thread = new Thread(new ThreadStart(() =>
			{
				int count = 0;
				while (true)
				{
					if (IsThreadStoping)
						break;

					FiresecManager.FiresecService.Test("Hello " + count++.ToString());//.ShortPoll(FiresecServiceFactory.UID);
					Thread.Sleep(1000);
				}
			}));
			thread.IsBackground = true;
			thread.Start();
		}

		public RelayCommand Test9Command { get; private set; }

		private void OnTest9()
		{
			FiresecManager.DeviceLibraryConfiguration = null;
			FiresecManager.DeviceLibraryConfiguration = GetConfig(FiresecManager.DeviceLibraryConfiguration, "DeviceLibraryConfiguration.xml");
			ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);
		}

		private T GetConfig<T>(T configuration, string fileName)
			where T : VersionedConfiguration, new()
		{
			var stream = FiresecManager.FiresecService.GetConfig();
			using (Stream file = File.Create("Configuration\\config.fscp"))
			{
				CopyStream(stream, file);
				file.Close();
				var unzip = ZipFile.Read("Configuration\\config.fscp", new ReadOptions { Encoding = Encoding.GetEncoding("cp866") });
				var xmlstream = new MemoryStream();
				var entry = unzip[fileName];
				if (entry != null)
				{
					entry.Extract(xmlstream);
					xmlstream.Position = 0;
					configuration = ZipSerializeHelper.DeSerialize<T>(xmlstream);
				}
				return configuration;
			}
		}

		public RelayCommand Test10Command { get; private set; }

		private void OnTest10()
		{
			//DiagnosticsModule.stopWatch.Reset();
			//DiagnosticsModule.stopWatch.Start();
			//var deviceIconTestViewModel = new DeviceIconTestViewModel();
			//DialogService.ShowModalWindow(deviceIconTestViewModel);
		}

		public RelayCommand Test11Command { get; private set; }

		private void OnTest11()
		{
			//ZoneTestViewModel.Stopwatch.Restart();
			//var zoneTestViewModel = new ZoneTestViewModel();
			//DialogService.ShowModalWindow(zoneTestViewModel);
		}

		public static void CopyStream(Stream input, Stream output)
		{
			byte[] buffer = new byte[8 * 1024];
			int len;
			while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
			{
				output.Write(buffer, 0, len);
			}
		}

		public RelayCommand BalloonTestCommand { get; private set; }

		private void OnBalloonTest()
		{
			BalloonHelper.Show("Предупреждение", "Это текст предупреждения");
		}

		public RelayCommand PlanDuplicateTestCommand { get; private set; }

		private void OnPlanDuplicateTest()
		{
			using (new WaitWrapper())
			{
				DataContractSerializer serializer = new DataContractSerializer(typeof(Plan));
				var Count = FiresecManager.PlansConfiguration.Plans.Count;
				for (int i = 0; i < Count; i++)
				{
					Plan plan = null;
					using (var stream = new MemoryStream())
					{
						serializer.WriteObject(stream, FiresecManager.PlansConfiguration.Plans[i]);
						stream.Seek(0, SeekOrigin.Begin);
						plan = (Plan)serializer.ReadObject(stream);
					}
					if (plan != null)
						FiresecManager.PlansConfiguration.Plans.Add(plan);
				}

				var appType = Application.Current.GetType();
				var bootstrapperProp = appType.GetField("_bootstrapper", BindingFlags.Instance | BindingFlags.NonPublic);
				var bootstrapper = bootstrapperProp.GetValue(Application.Current);
				var modulesProp = bootstrapper.GetType().BaseType.GetField("_modules", BindingFlags.Instance | BindingFlags.NonPublic);
				var modules = (List<IModule>)modulesProp.GetValue(bootstrapper);
				foreach (IModule module in modules)
					if (module.Name == "Графические планы")
						module.Initialize();
			}
			MessageBox.Show("Всего планов: " + FiresecManager.PlansConfiguration.Plans.Count);
		}

		public RelayCommand MailCommand { get; private set; }

		private void OnMail()
		{
			MailHelper.Send(FiresecManager.SystemConfiguration.EmailData.EmailSettings, "obychevma@rubezh.ru",
							@"This message was automatically sent by Firesec-2 application",
							"Testing message from Firesec-2");
		}
	}
}