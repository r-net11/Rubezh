using Common;
using GKProcessor;
using Infrastructure.Common;
using Infrastructure.Common.Services;
using RubezhAPI;
using RubezhAPI.Automation;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhAPI.Models.Layouts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RubezhClient
{
	public partial class ClientManager
	{
		public static PlansConfiguration PlansConfiguration
		{
			get { return ConfigurationCash.PlansConfiguration; }
			set { ConfigurationCash.PlansConfiguration = value; }
		}

		public static SystemConfiguration SystemConfiguration { get; set; }
		public static SecurityConfiguration SecurityConfiguration { get; set; }
		public static LayoutsConfiguration LayoutsConfiguration { get; set; }

		public static void UpdateFiles()
		{
			try
			{
				FileHelper.Synchronize();
			}
			catch (Exception e)
			{
				Logger.Error(e, "ClientManager.UpdateFiles");
				LoadingErrorManager.Add(e);
			}
		}

		public static void CopyStream(Stream input, Stream output)
		{
			var buffer = new byte[8 * 1024];
			int length;
			while ((length = input.Read(buffer, 0, buffer.Length)) > 0)
			{
				output.Write(buffer, 0, length);
			}
			output.Close();
		}

		public static void GetConfiguration(string configurationFolderName)
		{
			try
			{
				var serverConfigDirectory = AppDataFolderHelper.GetServerAppDataPath("Config");
				var configDirectory = AppDataFolderHelper.GetLocalFolder(configurationFolderName);
				var contentDirectory = Path.Combine(configDirectory, "Content");
				if (Directory.Exists(configDirectory))
				{
					Directory.Delete(configDirectory, true);
				}
				Directory.CreateDirectory(configDirectory);
				Directory.CreateDirectory(contentDirectory);
				//if (ServiceFactoryBase.ContentService != null)
				//	ServiceFactoryBase.ContentService.Invalidate();

				if (ConnectionSettingsManager.IsRemote)
				{
					var configFileName = Path.Combine(configDirectory, "Config.fscp");
					var configFileStream = File.Create(configFileName);
					var stream = FiresecService.GetConfig();
					CopyStream(stream, configFileStream);
					LoadFromZipFile(configFileName);

					var result = FiresecService.GetSecurityConfiguration();
					if (!result.HasError && result.Result != null)
					{
						SecurityConfiguration = result.Result;
					}
				}
				else
				{
					foreach (var fileName in Directory.GetFiles(serverConfigDirectory))
					{
						var file = Path.GetFileName(fileName);
						File.Copy(fileName, Path.Combine(configDirectory, file), true);
					}
					foreach (var fileName in Directory.GetFiles(Path.Combine(serverConfigDirectory, "Content")))
					{
						var file = Path.GetFileName(fileName);
						File.Copy(fileName, Path.Combine(contentDirectory, file), true);
					}

					if (File.Exists(serverConfigDirectory + "\\..\\SecurityConfiguration.xml"))
					{
						File.Copy(serverConfigDirectory + "\\..\\SecurityConfiguration.xml", Path.Combine(configDirectory, "SecurityConfiguration.xml"), true);
					}

					LoadConfigFromDirectory(configDirectory);
					SecurityConfiguration = ZipSerializeHelper.DeSerialize<SecurityConfiguration>(Path.Combine(configDirectory, "SecurityConfiguration.xml"));
				}
				UpdateConfiguration();
			}
			catch (Exception e)
			{
				Logger.Error(e, "ClientManager.GetConfiguration");
				LoadingErrorManager.Add(e);
			}
		}

		public static void UpdateConfiguration()
		{
			try
			{
				if (LayoutsConfiguration == null)
					LayoutsConfiguration = new LayoutsConfiguration();
				LayoutsConfiguration.Update();
				//PlansConfiguration.Update();
				SystemConfiguration.UpdateConfiguration();
				GKDriversCreator.Create();
				GKManager.UpdateConfiguration();
				GKManager.CreateStates();
				UpdatePlansConfiguration();
			}
			catch (Exception e)
			{
				Logger.Error(e, "ClientManager.UpdateConfiguration");
				LoadingErrorManager.Add(e);
			}
		}

		public static void UpdatePlansConfiguration()
		{
			
		}

		public static void InvalidateContent()
		{
		}
	}
}