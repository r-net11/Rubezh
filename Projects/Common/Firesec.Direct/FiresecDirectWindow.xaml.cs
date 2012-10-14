using System.Collections.Generic;
using System.IO;
using System.Windows;
using Firesec;
using FiresecAPI.Models;
using FiresecAPI;
using System.Runtime.Serialization;
using System;

namespace FiresecDirect
{
	public partial class FiresecDirectWindow : Window
	{
		public FiresecDirectWindow()
		{
			InitializeComponent();
			NativeFiresecClient = new Firesec.NativeFiresecClient();
		}

		Firesec.NativeFiresecClient NativeFiresecClient;

		void Window_Loaded(object sender, RoutedEventArgs e)
		{
			NativeFiresecClient.Connect("localhost", 211, "adm", "", false);
		}

		void OnSetNewConfig(object sender, RoutedEventArgs e)
		{
			NativeFiresecClient.SetNewConfig(textBox1.Text);
			//using (var reader = new StreamReader("SetNewConfig.xml"))
			//{
			//    textBox1.Text = reader.ReadToEnd();
			//}

			//byte[] bytes = Encoding.UTF8.GetBytes(textBox1.Text);
			//var memoryStream = new MemoryStream(bytes);
			//var serializer = new XmlSerializer(typeof(Firesec.CoreConfig.config));
			//Firesec.CoreConfig.config config = (Firesec.CoreConfig.config)serializer.Deserialize(memoryStream);
			//memoryStream.Close();

			//FiresecClient.SetNewConfig(config);
			//FiresecClient.DeviceWriteConfig(config, "0");
		}

		void OnGetCoreConfig(object sender, RoutedEventArgs e)
		{
			textBox1.Text = NativeFiresecClient.GetCoreConfig().Result;

			try
			{
				using (var fileStream = new FileStream("D:/CoreConfig.xml", FileMode.Create))
				using (var streamWriter = new StreamWriter(fileStream))
				{
					streamWriter.Write(textBox1.Text);
				}
			}
			catch { }
		}

		void OnGetPlans(object sender, RoutedEventArgs e)
		{
			string plans = NativeFiresecClient.GetPlans().Result;

			using (var fileStream = new FileStream("D:/Plan.xml", FileMode.Create))
			using (var streamWriter = new StreamWriter(fileStream))
			{
				streamWriter.Write(plans);
			}

			//textBox1.Text = plans;
		}

		void OnGetCoreState(object sender, RoutedEventArgs e)
		{
			textBox1.Text = NativeFiresecClient.GetCoreState().Result;
		}

		void OnGetMetaData(object sender, RoutedEventArgs e)
		{
			textBox1.Text = NativeFiresecClient.GetMetadata().Result;

			using (var fileStream = new FileStream("D:/Metadata.xml", FileMode.Create))
			using (var streamWriter = new StreamWriter(fileStream))
			{
				streamWriter.Write(textBox1.Text);
			}
		}

		void OnGetCoreDeviceParams(object sender, RoutedEventArgs e)
		{
			textBox1.Text = NativeFiresecClient.GetCoreDeviceParams().Result;
		}

		void OnReadEvents(object sender, RoutedEventArgs e)
		{
			textBox1.Text = NativeFiresecClient.ReadEvents(0, 100).Result;
		}

		void Button_Click_6(object sender, RoutedEventArgs e)
		{
		}

		void OnBoltOpen(object sender, RoutedEventArgs e)
		{
			NativeFiresecClient.ExecuteCommand("0\\0\\6\\13", "BoltOpen");
		}

		void OnBoltClose(object sender, RoutedEventArgs e)
		{
		}

		void OnBoltStop(object sender, RoutedEventArgs e)
		{
		}

		void OnBoltAutoOn(object sender, RoutedEventArgs e)
		{
		}

		void OnBoltAutoOff(object sender, RoutedEventArgs e)
		{
		}

		void OnAddToIgnoreList(object sender, RoutedEventArgs e)
		{
			NativeFiresecClient.AddToIgnoreList(new List<string>() { "0\\0\\0\\0" });
		}

		void OnRemoveFromIgnoreList(object sender, RoutedEventArgs e)
		{
		}

		void OnAddCustomMessage(object sender, RoutedEventArgs e)
		{
			NativeFiresecClient.AddUserMessage("message");
		}

		private void OnExecuteRuntimeDeviceMethod1(object sender, RoutedEventArgs e)
		{
			int reguestId = 0;
			var result = NativeFiresecClient.ExecuteRuntimeDeviceMethod(Execute1_devicePath.Text, Execute1_MethodName.Text, Execute1_AParams.Text, ref reguestId);
			if (result.HasError)
				MessageBox.Show("Error:" + result.Error);
			else
				MessageBox.Show("Result:" + result.Result);
		}

		private void OnExecuteRuntimeDeviceMethod2(object sender, RoutedEventArgs e)
		{
			int reguestId = 0;
			var result = NativeFiresecClient.ExecuteRuntimeDeviceMethod(Execute2_devicePath.Text, Execute2_MethodName.Text, Execute2_AParams.Text, ref reguestId);
			if (result.HasError)
				MessageBox.Show("Error:" + result.Error);
			else
				MessageBox.Show("Result:" + result.Result);
		}

		private void OnGetConfigurationParameters(object sender, RoutedEventArgs e)
		{
			var result = NativeFiresecClient.GetConfigurationParameters(Execute1_devicePath.Text, int.Parse(Execute1_AParams.Text));
			if (result.HasError)
				MessageBox.Show("Error:" + result.Error);
			else
				MessageBox.Show("Result:" + result.Result);
		}

		private void OnSetConfigurationParameters(object sender, RoutedEventArgs e)
		{
			var result = NativeFiresecClient.SetConfigurationParameters(Execute1_devicePath.Text, Execute1_AParams.Text);
			if (result.HasError)
				MessageBox.Show("Error:" + result.Error);
			else
				MessageBox.Show("Result:" + result.Result);
		}

		private void OnCreateDrivers(object sender, RoutedEventArgs e)
		{
			var firesecDriver = new FiresecDriver(0, "localhost", 211, "adm", "", false);
			var driversConfiguration = ConfigurationCash.DriversConfiguration;

			try
			{
				driversConfiguration.Version = new ConfigurationVersion() { MajorVersion = 1, MinorVersion = 1 };

				using (var memoryStream = new MemoryStream())
				{
					var dataContractSerializer = new DataContractSerializer(typeof(DriversConfiguration));
					dataContractSerializer.WriteObject(memoryStream, driversConfiguration);

					using (var fileStream = new FileStream("D:/DriversConfiguration.xml", FileMode.Create))
					{
						fileStream.Write(memoryStream.GetBuffer(), 0, (int)memoryStream.Position);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
	}
}