using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Media;
using Common;

namespace Infrastructure.Common
{
	public class RegistrySettingsHelper
	{
		static string FileName = AppDataFolderHelper.GetRegistryDataConfigurationFileName();
		static object locker = new object();

		public static string GetString(string name)
		{
			var registryData = GetRegistryData(name);
			if (registryData != null)
			{
				return registryData.StringValue;
			}
			return null;
		}

		public static int GetInt(string name)
		{
			var registryData = GetRegistryData(name);
			if (registryData != null)
			{
				return registryData.IntValue;
			}
			return 0;
		}

		public static double GetDouble(string name)
		{
			var registryData = GetRegistryData(name);
			if (registryData != null)
			{
				return registryData.DoubleValue;
			}
			return 0;
		}

		public static bool GetBool(string name)
		{
			return GetBool(name, false);
		}
		public static bool GetBool(string name, bool defaultValue)
		{
			var registryData = GetRegistryData(name);
			if (registryData != null)
			{
				return registryData.BoolValue;
			}
			return defaultValue;
		}

		public static List<string> GetStrings(string name)
		{
			var registryData = GetRegistryData(name);
			if (registryData != null)
			{
				return registryData.StringsValue;
			}
			return null;
		}

		public static WindowRect GetWindowRect(string name)
		{
			var registryData = GetRegistryData(name);
			if (registryData != null)
			{
				return registryData.WindowRectValue;
			}
			return null;
		}

		public static Color GetColor(string name)
		{
			var registryData = GetRegistryData(name);
			if (registryData != null)
			{
				return registryData.ColorValue;
			}
			return Colors.White;
		}

		public static void SetString(string name, string stringValue)
		{
			var registryData = new RegistryData()
			{
				Name = name,
				StringValue = stringValue
			};
			SetRegistryData(registryData);
		}

		public static void SetInt(string name, int intValue)
		{
			var registryData = new RegistryData()
			{
				Name = name,
				IntValue = intValue
			};
			SetRegistryData(registryData);
		}

		public static void SetDouble(string name, double doubleValue)
		{
			var registryData = new RegistryData()
			{
				Name = name,
				DoubleValue = doubleValue
			};
			SetRegistryData(registryData);
		}

		public static void SetBool(string name, bool boolValue)
		{
			var registryData = new RegistryData()
			{
				Name = name,
				BoolValue = boolValue
			};
			SetRegistryData(registryData);
		}

		public static void SetStrings(string name, List<string> stringsValue)
		{
			var registryData = new RegistryData()
			{
				Name = name,
				StringsValue = stringsValue
			};
			SetRegistryData(registryData);
		}

		public static void SetWindowRect(string name, WindowRect windowRect)
		{
			var registryData = new RegistryData()
			{
				Name = name,
				WindowRectValue = windowRect
			};
			SetRegistryData(registryData);
		}

		public static void SetColor(string name, Color color)
		{
			var registryData = new RegistryData()
			{
				Name = name,
				ColorValue = color
			};
			SetRegistryData(registryData);
		}

		static RegistryData GetRegistryData(string name)
		{
			try
			{
				var registryDataConfiguration = GetRegistryDataConfiguration();
				var registryData = registryDataConfiguration.RegistryDataCollection.FirstOrDefault(x => x.Name == name);
				return registryData;
			}
			catch (Exception e)
			{
				Logger.Error(e, "RegistrySettingsHelper.GetRegistryData " + name);
				return null;
			}
		}

		static void SetRegistryData(RegistryData newRegistryData)
		{
			try
			{
				var registryDataConfiguration = GetRegistryDataConfiguration();
				var registryData = registryDataConfiguration.RegistryDataCollection.FirstOrDefault(x => x.Name == newRegistryData.Name);
				if (registryData == null)
				{
					registryData = new RegistryData()
					{
						Name = newRegistryData.Name
					};
					registryDataConfiguration.RegistryDataCollection.Add(registryData);
				}
				registryData.StringValue = newRegistryData.StringValue;
				registryData.IntValue = newRegistryData.IntValue;
				registryData.DoubleValue = newRegistryData.DoubleValue;
				registryData.BoolValue = newRegistryData.BoolValue;
				registryData.StringsValue = newRegistryData.StringsValue;
				registryData.WindowRectValue = newRegistryData.WindowRectValue;
				registryData.ColorValue = newRegistryData.ColorValue;
				SetRegistryDataConfiguration(registryDataConfiguration);
			}
			catch (Exception e)
			{
				Logger.Error(e, "RegistrySettingsHelper.SetRegistryData " + newRegistryData.Name);
			}
		}

		static RegistryDataConfiguration GetRegistryDataConfiguration()
		{
			lock (locker)
			{
				//using (var mutex = new Mutex(true, "RegistryDataConfiguration"))
				//{
				//	mutex.WaitOne(TimeSpan.FromSeconds(1));
				for (int i = 0; i < 3; i++)
				{
					var tempFileName = FileName + "." + Guid.NewGuid().ToString();
					try
					{
						var registryDataConfiguration = new RegistryDataConfiguration();
						if (File.Exists(FileName))
						{
							File.Copy(FileName, tempFileName);
							using (var fileStream = new FileStream(tempFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
							{
								var dataContractSerializer = new DataContractSerializer(typeof(RegistryDataConfiguration));
								registryDataConfiguration = (RegistryDataConfiguration)dataContractSerializer.ReadObject(fileStream);
							}
						}
						return registryDataConfiguration;
					}
					catch (Exception e)
					{
						Logger.Error(e, "RegistrySettingsHelper.GetRegistryDataConfiguration ");
					}
					finally
					{
						if (File.Exists(tempFileName))
						{
							File.Delete(tempFileName);
						}
						//mutex.ReleaseMutex();
					}
				}
				return new RegistryDataConfiguration();
				//}
			}
		}

		static void SetRegistryDataConfiguration(RegistryDataConfiguration registryDataConfiguration)
		{
			lock (locker)
			{
				//EventWaitHandle eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, "");
				//eventWaitHandle.
				//using (var mutex = new Mutex(false, "RegistryDataConfiguration"))
				//{
				//	mutex.WaitOne(TimeSpan.FromSeconds(1));
				for (int i = 0; i < 3; i++)
				{
					var tempFileName = FileName + "." + Guid.NewGuid().ToString();
					try
					{
						var dataContractSerializer = new DataContractSerializer(typeof(RegistryDataConfiguration));
						using (var fileStream = new FileStream(tempFileName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
						{
							dataContractSerializer.WriteObject(fileStream, registryDataConfiguration);
						}
						File.Copy(tempFileName, FileName, true);
						return;
					}
					catch (Exception e)
					{
						Logger.Error(e, "RegistrySettingsHelper.SetRegistryDataConfiguration");
					}
					finally
					{
						if (File.Exists(tempFileName))
						{
							File.Delete(tempFileName);
						}
						//mutex.ReleaseMutex();
					}
				}
				//}
			}
		}
	}

	[DataContract]
	public class RegistryDataConfiguration
	{
		public RegistryDataConfiguration()
		{
			RegistryDataCollection = new List<RegistryData>();
		}

		[DataMember]
		public List<RegistryData> RegistryDataCollection { get; set; }
	}

	[DataContract]
	public class RegistryData
	{
		public RegistryData()
		{
			StringsValue = new List<string>();
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string StringValue { get; set; }

		[DataMember]
		public int IntValue { get; set; }

		[DataMember]
		public double DoubleValue { get; set; }

		[DataMember]
		public bool BoolValue { get; set; }

		[DataMember]
		public List<string> StringsValue { get; set; }

		[DataMember]
		public WindowRect WindowRectValue { get; set; }

		[DataMember]
		public Color ColorValue { get; set; }
	}

	[DataContract]
	public class WindowRect
	{
		[DataMember]
		public double Left { get; set; }

		[DataMember]
		public double Top { get; set; }

		[DataMember]
		public double Width { get; set; }

		[DataMember]
		public double Height { get; set; }
	}
}